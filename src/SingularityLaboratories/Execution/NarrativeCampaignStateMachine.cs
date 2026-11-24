using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>Event kinds in the append-only narrative campaign attempt history.</summary>
public enum NarrativeCampaignEventKind
{
    AttemptStarted = 1,
    EvidenceRecorded = 2,
    PlayerDeath = 3,
    SafeEscapeDenied = 4,
    SafeEscapeVerified = 5
}

/// <summary>One immutable event in a narrative campaign attempt.</summary>
public sealed record NarrativeCampaignEvent(
    int Sequence,
    NarrativeCampaignEventKind Kind,
    DateTimeOffset OccurredAtUtc,
    string Details);

/// <summary>
/// Immutable attempt snapshot. Restarting creates another attempt rather than rewriting this one.
/// </summary>
public sealed record NarrativeCampaignAttempt
{
    internal NarrativeCampaignAttempt(
        Guid attemptId,
        NarrativeCampaignDefinition definition,
        NarrativeCampaignOutcome outcome,
        IReadOnlyList<NarrativeEvidenceRecord> evidence,
        IReadOnlyList<NarrativeCampaignEvent> events)
    {
        AttemptId = attemptId;
        Definition = definition;
        CampaignId = definition.Id;
        CampaignVersion = definition.Version;
        PlayerRole = definition.FixedPlayerRole;
        StartingLocation = definition.StartingLocation;
        Outcome = outcome;
        Evidence = Array.AsReadOnly(evidence.ToArray());
        Events = Array.AsReadOnly(events.ToArray());
    }

    /// <summary>Restores a persisted immutable attempt after validating its event and outcome invariants.</summary>
    public static NarrativeCampaignAttempt Restore(
        Guid attemptId,
        NarrativeCampaignDefinition definition,
        NarrativeCampaignOutcome outcome,
        IReadOnlyList<NarrativeEvidenceRecord> evidence,
        IReadOnlyList<NarrativeCampaignEvent> events)
    {
        if (attemptId == Guid.Empty)
            throw new ArgumentException("An attempt identifier is required.", nameof(attemptId));
        ArgumentNullException.ThrowIfNull(definition);
        if (!Enum.IsDefined(outcome))
            throw new ArgumentOutOfRangeException(nameof(outcome));
        ArgumentNullException.ThrowIfNull(evidence);
        ArgumentNullException.ThrowIfNull(events);

        var evidenceSnapshot = evidence.ToArray();
        if (evidenceSnapshot.Any(item => item is null ||
            !definition.RequiredSafeEscapeEvidence.Contains(item.Kind)))
            throw new ArgumentException("Persisted evidence must contain only required, non-null campaign evidence.", nameof(evidence));

        var eventSnapshot = events.ToArray();
        if (eventSnapshot.Length == 0 || eventSnapshot.Any(item => item is null))
            throw new ArgumentException("A persisted attempt must contain its start event and non-null event history.", nameof(events));

        for (var index = 0; index < eventSnapshot.Length; index++)
        {
            var item = eventSnapshot[index];
            if (item.Sequence != index + 1 ||
                !Enum.IsDefined(item.Kind) ||
                item.OccurredAtUtc.Offset != TimeSpan.Zero ||
                string.IsNullOrWhiteSpace(item.Details))
                throw new ArgumentException("Persisted campaign events must have contiguous sequence, valid kind, UTC timestamp, and details.", nameof(events));
            if ((index == 0) != (item.Kind == NarrativeCampaignEventKind.AttemptStarted))
                throw new ArgumentException("The attempt-start event must appear exactly once as the first event.", nameof(events));
            var terminalEvent = item.Kind is NarrativeCampaignEventKind.PlayerDeath or NarrativeCampaignEventKind.SafeEscapeVerified;
            if (terminalEvent && index != eventSnapshot.Length - 1)
                throw new ArgumentException("No event may follow a terminal campaign outcome.", nameof(events));
        }

        if (eventSnapshot.Count(item => item.Kind == NarrativeCampaignEventKind.EvidenceRecorded) != evidenceSnapshot.Length)
            throw new ArgumentException("Evidence records and evidence-recorded events must have matching counts.", nameof(evidence));

        var finalKind = eventSnapshot[^1].Kind;
        var outcomeMatchesHistory = outcome switch
        {
            NarrativeCampaignOutcome.Active =>
                finalKind is not NarrativeCampaignEventKind.PlayerDeath and not NarrativeCampaignEventKind.SafeEscapeVerified,
            NarrativeCampaignOutcome.PlayerDeath => finalKind == NarrativeCampaignEventKind.PlayerDeath,
            NarrativeCampaignOutcome.VerifiedSafeEscape => finalKind == NarrativeCampaignEventKind.SafeEscapeVerified,
            _ => false
        };
        if (!outcomeMatchesHistory)
            throw new ArgumentException("The persisted outcome must agree with the final event.", nameof(outcome));

        if (outcome == NarrativeCampaignOutcome.VerifiedSafeEscape)
        {
            var latest = definition.RequiredSafeEscapeEvidence
                .Select(kind => evidenceSnapshot.LastOrDefault(item => item.Kind == kind))
                .ToArray();
            var allCurrent = latest.All(item => item?.State == NarrativeEvidenceState.VerifiedCurrent);
            var independentSources = latest.Where(item => item is not null)
                .Select(item => item!.SourceId).Distinct(StringComparer.Ordinal).Count() == latest.Length;
            var egress = latest.SingleOrDefault(item => item?.Kind == NarrativeEvidenceKind.EgressRecord);
            var routeMatches = egress is not null &&
                eventSnapshot[^1].Details.EndsWith($"route '{egress.EvidenceReference}'.", StringComparison.Ordinal);
            if (!allCurrent || !independentSources || !routeMatches)
                throw new ArgumentException("A persisted safe-escape outcome must satisfy the current evidence, independent-source, and egress-route gates.", nameof(outcome));
        }

        return new NarrativeCampaignAttempt(attemptId, definition, outcome, evidenceSnapshot, eventSnapshot);
    }

    public Guid AttemptId { get; }
    public NarrativeCampaignDefinition Definition { get; }
    public string CampaignId { get; }
    public string CampaignVersion { get; }
    public string PlayerRole { get; }
    public string StartingLocation { get; }
    public NarrativeCampaignOutcome Outcome { get; }
    public IReadOnlyList<NarrativeEvidenceRecord> Evidence { get; }
    public IReadOnlyList<NarrativeCampaignEvent> Events { get; }
}

/// <summary>
/// Pure domain transition service for fixed-opening narrative campaigns.
/// Safe escape is awarded only when every required evidence category is current and verified.
/// </summary>
/// <remarks>
/// This state machine records fictional game outcomes only. It does not control a facility,
/// execute a scientific model, or determine real-world safety.
/// </remarks>
public sealed class NarrativeCampaignStateMachine
{
    public NarrativeCampaignAttempt Start(
        NarrativeCampaignDefinition definition,
        Guid attemptId,
        DateTimeOffset startedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (attemptId == Guid.Empty)
            throw new ArgumentException("An attempt identifier is required.", nameof(attemptId));

        var started = new NarrativeCampaignEvent(
            1,
            NarrativeCampaignEventKind.AttemptStarted,
            startedAtUtc.ToUniversalTime(),
            $"Fixed role '{definition.FixedPlayerRole}' begins at '{definition.StartingLocation}'.");
        return new NarrativeCampaignAttempt(
            attemptId,
            definition,
            NarrativeCampaignOutcome.Active,
            [],
            [started]);
    }

    public NarrativeCampaignAttempt RecordEvidence(
        NarrativeCampaignDefinition definition,
        NarrativeCampaignAttempt attempt,
        NarrativeEvidenceRecord evidence,
        DateTimeOffset recordedAtUtc)
    {
        ValidateActiveAttempt(definition, attempt);
        ArgumentNullException.ThrowIfNull(evidence);
        if (!definition.RequiredSafeEscapeEvidence.Contains(evidence.Kind))
            throw new ArgumentException("The evidence category is not part of this campaign's safe-escape gate.", nameof(evidence));
        // Re-inspection is allowed: retain every source observation, and let the latest record
        // for a category determine whether that category currently satisfies the gate.
        var evidenceSnapshot = attempt.Evidence.Append(evidence).ToArray();
        var eventSnapshot = AppendEvent(
            attempt,
            NarrativeCampaignEventKind.EvidenceRecorded,
            recordedAtUtc,
            $"{evidence.Kind}: {evidence.State} from {evidence.SourceId} ({evidence.EvidenceReference}); source observed {evidence.ObservedAtUtc:O}.");

        return Copy(attempt, NarrativeCampaignOutcome.Active, evidenceSnapshot, eventSnapshot);
    }

    public NarrativeCampaignAttempt RecordPlayerDeath(
        NarrativeCampaignDefinition definition,
        NarrativeCampaignAttempt attempt,
        string cause,
        DateTimeOffset occurredAtUtc)
    {
        ValidateActiveAttempt(definition, attempt);
        ArgumentException.ThrowIfNullOrWhiteSpace(cause);
        var events = AppendEvent(
            attempt,
            NarrativeCampaignEventKind.PlayerDeath,
            occurredAtUtc,
            $"Player death: {cause.Trim()}.");
        return Copy(attempt, NarrativeCampaignOutcome.PlayerDeath, attempt.Evidence, events);
    }

    public NarrativeCampaignAttempt AttemptSafeEscape(
        NarrativeCampaignDefinition definition,
        NarrativeCampaignAttempt attempt,
        string routeReference,
        DateTimeOffset occurredAtUtc)
    {
        ValidateActiveAttempt(definition, attempt);
        ArgumentException.ThrowIfNullOrWhiteSpace(routeReference);

        var latestEvidence = definition.RequiredSafeEscapeEvidence
            .Select(required => new
            {
                Kind = required,
                Record = attempt.Evidence.LastOrDefault(record => record.Kind == required)
            })
            .ToArray();
        var missingOrUnverified = latestEvidence
            .Where(item => item.Record?.State != NarrativeEvidenceState.VerifiedCurrent)
            .Select(item => item.Kind)
            .ToArray();
        var duplicatedSources = latestEvidence
            .Where(item => item.Record?.State == NarrativeEvidenceState.VerifiedCurrent)
            .GroupBy(item => item.Record!.SourceId, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();
        var egressRecord = latestEvidence
            .Single(item => item.Kind == NarrativeEvidenceKind.EgressRecord)
            .Record;
        var routeMatchesEvidence = string.Equals(
            egressRecord?.EvidenceReference,
            routeReference.Trim(),
            StringComparison.Ordinal);

        if (missingOrUnverified.Length > 0 || duplicatedSources.Length > 0 || !routeMatchesEvidence)
        {
            var details = $"Safe escape denied for route '{routeReference.Trim()}'; " +
                $"missing or unverified evidence: {string.Join(", ", missingOrUnverified)}; " +
                $"duplicated evidence sources: {string.Join(", ", duplicatedSources)}; " +
                $"egress record route match: {routeMatchesEvidence}.";
            var deniedEvents = AppendEvent(
                attempt,
                NarrativeCampaignEventKind.SafeEscapeDenied,
                occurredAtUtc,
                details);
            return Copy(attempt, NarrativeCampaignOutcome.Active, attempt.Evidence, deniedEvents);
        }

        var verifiedEvents = AppendEvent(
            attempt,
            NarrativeCampaignEventKind.SafeEscapeVerified,
            occurredAtUtc,
            $"Fictional safe-escape gate satisfied for route '{routeReference.Trim()}'.");
        return Copy(attempt, NarrativeCampaignOutcome.VerifiedSafeEscape, attempt.Evidence, verifiedEvents);
    }

    private static void ValidateActiveAttempt(
        NarrativeCampaignDefinition definition,
        NarrativeCampaignAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(attempt);
        if (!string.Equals(attempt.CampaignId, definition.Id, StringComparison.Ordinal) ||
            !string.Equals(attempt.CampaignVersion, definition.Version, StringComparison.Ordinal) ||
            !string.Equals(attempt.Definition.FixedPlayerRole, definition.FixedPlayerRole, StringComparison.Ordinal) ||
            !string.Equals(attempt.Definition.StartingLocation, definition.StartingLocation, StringComparison.Ordinal) ||
            !attempt.Definition.RequiredSafeEscapeEvidence.SequenceEqual(definition.RequiredSafeEscapeEvidence))
            throw new ArgumentException("The attempt is not bound to this exact campaign definition and version.", nameof(attempt));
        if (attempt.Outcome != NarrativeCampaignOutcome.Active)
            throw new InvalidOperationException("A terminal campaign attempt cannot accept further transitions.");
    }

    private static IReadOnlyList<NarrativeCampaignEvent> AppendEvent(
        NarrativeCampaignAttempt attempt,
        NarrativeCampaignEventKind kind,
        DateTimeOffset occurredAtUtc,
        string details) =>
        attempt.Events.Append(new NarrativeCampaignEvent(
            attempt.Events.Count + 1,
            kind,
            occurredAtUtc.ToUniversalTime(),
            details)).ToArray();

    private static NarrativeCampaignAttempt Copy(
        NarrativeCampaignAttempt attempt,
        NarrativeCampaignOutcome outcome,
        IReadOnlyList<NarrativeEvidenceRecord> evidence,
        IReadOnlyList<NarrativeCampaignEvent> events) =>
        new(
            attempt.AttemptId,
            attempt.Definition,
            outcome,
            evidence,
            events);
}
