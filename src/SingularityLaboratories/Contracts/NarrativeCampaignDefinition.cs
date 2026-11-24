namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Evidence categories required by a narrative laboratory campaign.</summary>
public enum NarrativeEvidenceKind
{
    IncidentTimeline = 1,
    FacilityState = 2,
    EgressRecord = 3
}

/// <summary>How strongly an inspected source supports the fictional campaign's current state.</summary>
public enum NarrativeEvidenceState
{
    Observed = 1,
    VerifiedCurrent = 2,
    Stale = 3,
    Contradicted = 4
}

/// <summary>Terminal disposition of one narrative campaign attempt.</summary>
public enum NarrativeCampaignOutcome
{
    Active = 1,
    PlayerDeath = 2,
    VerifiedSafeEscape = 3
}

/// <summary>Immutable evidence reference recorded during a narrative campaign attempt.</summary>
public sealed record NarrativeEvidenceRecord
{
    public NarrativeEvidenceRecord(
        NarrativeEvidenceKind kind,
        string sourceId,
        string evidenceReference,
        NarrativeEvidenceState state,
        DateTimeOffset observedAtUtc)
    {
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind));
        if (!Enum.IsDefined(state))
            throw new ArgumentOutOfRangeException(nameof(state));
        SourceId = Required(sourceId, nameof(sourceId));
        EvidenceReference = Required(evidenceReference, nameof(evidenceReference));
        Kind = kind;
        State = state;
        ObservedAtUtc = observedAtUtc.ToUniversalTime();
    }

    public NarrativeEvidenceKind Kind { get; }
    public string SourceId { get; }
    public string EvidenceReference { get; }
    public NarrativeEvidenceState State { get; }
    public DateTimeOffset ObservedAtUtc { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>
/// Fixed-opening narrative campaign definition, independent of any game or UI host.
/// </summary>
/// <remarks>
/// This contract describes fictional narrative state. It is not a biological model, facility
/// control system, real-world safety procedure, or scientific experiment campaign.
/// </remarks>
public sealed record NarrativeCampaignDefinition
{
    public NarrativeCampaignDefinition(
        string id,
        string version,
        string fixedPlayerRole,
        string startingLocation,
        IReadOnlyList<NarrativeEvidenceKind> requiredSafeEscapeEvidence)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        FixedPlayerRole = Required(fixedPlayerRole, nameof(fixedPlayerRole));
        StartingLocation = Required(startingLocation, nameof(startingLocation));

        ArgumentNullException.ThrowIfNull(requiredSafeEscapeEvidence);
        var evidence = requiredSafeEscapeEvidence.ToArray();
        if (evidence.Length == 0)
            throw new ArgumentException("A safe-escape gate must require at least one evidence category.", nameof(requiredSafeEscapeEvidence));
        if (evidence.Any(kind => !Enum.IsDefined(kind)))
            throw new ArgumentOutOfRangeException(nameof(requiredSafeEscapeEvidence));
        if (evidence.Distinct().Count() != evidence.Length)
            throw new ArgumentException("Safe-escape evidence categories must be unique.", nameof(requiredSafeEscapeEvidence));

        RequiredSafeEscapeEvidence = Array.AsReadOnly(evidence);
    }

    public string Id { get; }
    public string Version { get; }
    public string FixedPlayerRole { get; }
    public string StartingLocation { get; }
    public IReadOnlyList<NarrativeEvidenceKind> RequiredSafeEscapeEvidence { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
