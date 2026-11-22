namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Review state for a version-bound scientific experiment assessment.</summary>
/// <remarks>
/// Review state is separate from <see cref="ScientificValidationLevel"/>. An approval record
/// documents a review decision; it does not silently upgrade the experiment's declared evidence level.
/// </remarks>
public enum ExperimentValidationState
{
    NotAssessed = 0,
    InReview = 1,
    Accepted = 2,
    Rejected = 3,
    Superseded = 4
}

/// <summary>Qualitative hazard classification declared by the experiment author or reviewer.</summary>
public enum ExperimentRiskLevel
{
    Unspecified = 0,
    Low = 1,
    Moderate = 2,
    High = 3,
    Critical = 4
}

/// <summary>Safety disclosures that travel with a versioned experiment definition.</summary>
public sealed record ExperimentSafetyMetadata
{
    public ExperimentSafetyMetadata(
        ExperimentRiskLevel riskLevel,
        IEnumerable<string> hazards,
        IEnumerable<string> precautions,
        IEnumerable<string> prohibitedUses)
    {
        if (!Enum.IsDefined(riskLevel) || riskLevel == ExperimentRiskLevel.Unspecified)
            throw new ArgumentOutOfRangeException(nameof(riskLevel), "A declared risk level is required.");
        RiskLevel = riskLevel;
        Hazards = Snapshot(hazards, nameof(hazards));
        Precautions = Snapshot(precautions, nameof(precautions));
        ProhibitedUses = Snapshot(prohibitedUses, nameof(prohibitedUses));
        if (Hazards.Count == 0)
            throw new ArgumentException("At least one hazard or explicit no-known-hazard statement is required.", nameof(hazards));
        if (Precautions.Count == 0)
            throw new ArgumentException("At least one precaution is required.", nameof(precautions));
        if (ProhibitedUses.Count == 0)
            throw new ArgumentException("At least one prohibited-use boundary is required.", nameof(prohibitedUses));
    }

    public ExperimentRiskLevel RiskLevel { get; }
    public IReadOnlyList<string> Hazards { get; }
    public IReadOnlyList<string> Precautions { get; }
    public IReadOnlyList<string> ProhibitedUses { get; }

    private static IReadOnlyList<string> Snapshot(IEnumerable<string> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        var snapshot = values.Select(value =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
            return value.Trim();
        }).ToArray();
        if (snapshot.Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Safety disclosures must be unique.", parameterName);
        return Array.AsReadOnly(snapshot);
    }
}

/// <summary>A traceable item of evidence considered during scientific validation review.</summary>
public sealed record ExperimentValidationEvidence
{
    public ExperimentValidationEvidence(
        string id,
        string description,
        DateTimeOffset recordedAtUtc,
        string? source = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (recordedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Evidence timestamps must be UTC.", nameof(recordedAtUtc));
        if (source is not null && string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("An evidence source must be non-empty when supplied.", nameof(source));
        Id = id.Trim();
        Description = description.Trim();
        RecordedAtUtc = recordedAtUtc;
        Source = source?.Trim();
    }

    public string Id { get; }
    public string Description { get; }
    public DateTimeOffset RecordedAtUtc { get; }
    public string? Source { get; }
}

/// <summary>
/// Immutable validation decision and evidence snapshot for one exact experiment and model version.
/// </summary>
/// <remarks>
/// Acceptance requires a named reviewer and at least one evidence item. This record does not
/// execute tests, certify safety, or change the experiment's declared scientific validation level.
/// </remarks>
public sealed record ExperimentValidationAssessment
{
    public ExperimentValidationAssessment(
        string experimentId,
        string experimentVersion,
        string modelId,
        string modelVersion,
        ExperimentValidationState state,
        DateTimeOffset assessedAtUtc,
        IEnumerable<ExperimentValidationEvidence> evidence,
        string? reviewer = null,
        string? decisionNotes = null)
    {
        ExperimentId = Required(experimentId, nameof(experimentId));
        ExperimentVersion = Required(experimentVersion, nameof(experimentVersion));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));
        if (!Enum.IsDefined(state))
            throw new ArgumentOutOfRangeException(nameof(state));
        if (assessedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Assessment timestamps must be UTC.", nameof(assessedAtUtc));
        if (reviewer is not null && string.IsNullOrWhiteSpace(reviewer))
            throw new ArgumentException("A reviewer must be non-empty when supplied.", nameof(reviewer));
        if (decisionNotes is not null && string.IsNullOrWhiteSpace(decisionNotes))
            throw new ArgumentException("Decision notes must be non-empty when supplied.", nameof(decisionNotes));

        ArgumentNullException.ThrowIfNull(evidence);
        var snapshot = evidence.ToArray();
        if (snapshot.Any(item => item is null))
            throw new ArgumentException("Evidence cannot contain null entries.", nameof(evidence));
        if (snapshot.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Evidence identifiers must be unique.", nameof(evidence));
        if (state == ExperimentValidationState.Accepted && (snapshot.Length == 0 || reviewer is null))
            throw new ArgumentException("An accepted assessment requires evidence and a named reviewer.");

        State = state;
        AssessedAtUtc = assessedAtUtc;
        Evidence = Array.AsReadOnly(snapshot);
        Reviewer = reviewer?.Trim();
        DecisionNotes = decisionNotes?.Trim();
    }

    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public ExperimentValidationState State { get; }
    public DateTimeOffset AssessedAtUtc { get; }
    public IReadOnlyList<ExperimentValidationEvidence> Evidence { get; }
    public string? Reviewer { get; }
    public string? DecisionNotes { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
