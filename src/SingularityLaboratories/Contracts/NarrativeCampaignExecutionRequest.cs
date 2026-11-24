namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>One deterministic transition requested by a narrative campaign workflow.</summary>
public enum NarrativeCampaignCommandKind
{
    RecordEvidence = 1,
    PlayerDeath = 2,
    AttemptSafeEscape = 3
}

/// <summary>Immutable, timestamped command in a narrative campaign execution request.</summary>
public sealed record NarrativeCampaignCommand
{
    public NarrativeCampaignCommand(
        NarrativeCampaignCommandKind kind,
        DateTimeOffset occurredAtUtc,
        NarrativeEvidenceRecord? evidence = null,
        string? value = null)
    {
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind));
        if (occurredAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Narrative command timestamps must be UTC.", nameof(occurredAtUtc));

        switch (kind)
        {
            case NarrativeCampaignCommandKind.RecordEvidence when evidence is null || value is not null:
                throw new ArgumentException("An evidence command requires an evidence record and no value.");
            case NarrativeCampaignCommandKind.PlayerDeath or NarrativeCampaignCommandKind.AttemptSafeEscape
                when evidence is not null || string.IsNullOrWhiteSpace(value):
                throw new ArgumentException("A death or escape command requires a non-empty value and no evidence record.");
        }

        Kind = kind;
        OccurredAtUtc = occurredAtUtc;
        Evidence = evidence;
        Value = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public NarrativeCampaignCommandKind Kind { get; }
    public DateTimeOffset OccurredAtUtc { get; }
    public NarrativeEvidenceRecord? Evidence { get; }
    public string? Value { get; }

    public static NarrativeCampaignCommand RecordEvidence(
        NarrativeEvidenceRecord evidence,
        DateTimeOffset recordedAtUtc) =>
        new(NarrativeCampaignCommandKind.RecordEvidence, recordedAtUtc, evidence);

    public static NarrativeCampaignCommand RecordPlayerDeath(string cause, DateTimeOffset occurredAtUtc) =>
        new(NarrativeCampaignCommandKind.PlayerDeath, occurredAtUtc, value: cause);

    public static NarrativeCampaignCommand AttemptSafeEscape(string routeReference, DateTimeOffset occurredAtUtc) =>
        new(NarrativeCampaignCommandKind.AttemptSafeEscape, occurredAtUtc, value: routeReference);
}

/// <summary>
/// Complete deterministic input for one narrative attempt workflow. The command sequence is
/// immutable and may contain at most one terminal command, which must be last.
/// </summary>
public sealed record NarrativeCampaignExecutionRequest
{
    public NarrativeCampaignExecutionRequest(
        NarrativeCampaignDefinition definition,
        Guid attemptId,
        DateTimeOffset startedAtUtc,
        IReadOnlyList<NarrativeCampaignCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if (attemptId == Guid.Empty)
            throw new ArgumentException("An attempt identity is required.", nameof(attemptId));
        if (startedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Narrative attempt start timestamps must be UTC.", nameof(startedAtUtc));
        ArgumentNullException.ThrowIfNull(commands);

        var snapshot = commands.ToArray();
        if (snapshot.Any(command => command is null))
            throw new ArgumentException("Narrative command sequences cannot contain null entries.", nameof(commands));
        if (snapshot.Take(snapshot.Length - 1).Any(command =>
            command.Kind is NarrativeCampaignCommandKind.PlayerDeath or NarrativeCampaignCommandKind.AttemptSafeEscape))
            throw new ArgumentException("A terminal narrative command must be the final command.", nameof(commands));

        Definition = definition;
        AttemptId = attemptId;
        StartedAtUtc = startedAtUtc;
        Commands = Array.AsReadOnly(snapshot);
    }

    public NarrativeCampaignDefinition Definition { get; }
    public Guid AttemptId { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public IReadOnlyList<NarrativeCampaignCommand> Commands { get; }
}
