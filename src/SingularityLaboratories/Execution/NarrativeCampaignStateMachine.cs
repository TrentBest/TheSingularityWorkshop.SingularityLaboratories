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
        string campaignId,
        string campaignVersion,
        string playerRole,
        string startingLocation,
        NarrativeCampaignOutcome outcome,
        IReadOnlyList<NarrativeEvidenceRecord> evidence,
        IReadOnlyList<NarrativeCampaignEvent> events)
    {
        AttemptId = attemptId;
        CampaignId = campaignId;
        CampaignVersion = campaignVersion;
        PlayerRole = playerRole;
        StartingLocation = startingLocation;
        Outcome = outcome;
        Evidence = Array.AsReadOnly(evidence.ToArray());
        Events = Array.AsReadOnly(events.ToArray());
    }

    public Guid AttemptId { get; }
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
            definition.Id,
            definition.Version,
            definition.FixedPlayerRole,
            definition.StartingLocation,
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

        var missingOrUnverified = definition.RequiredSafeEscapeEvidence
            .Where(required => attempt.Evidence.LastOrDefault(record => record.Kind == required)?.State
                != NarrativeEvidenceState.VerifiedCurrent)
            .ToArray();

        if (missingOrUnverified.Length > 0)
        {
            var details = $"Safe escape denied for route '{routeReference.Trim()}'; missing or unverified evidence: {string.Join(", ", missingOrUnverified)}.";
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
            !string.Equals(attempt.CampaignVersion, definition.Version, StringComparison.Ordinal))
            throw new ArgumentException("The attempt belongs to a different campaign identity or version.", nameof(attempt));
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
            attempt.CampaignId,
            attempt.CampaignVersion,
            attempt.PlayerRole,
            attempt.StartingLocation,
            outcome,
            evidence,
            events);
}
