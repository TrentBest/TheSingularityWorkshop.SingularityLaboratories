namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Immutable provenance and result envelope for one completed experiment run.</summary>
public sealed record ExperimentRunRecord
{
    public ExperimentRunRecord(
        Guid runId,
        string laboratoryId,
        string laboratoryVersion,
        string experimentId,
        string experimentVersion,
        string modelId,
        string modelVersion,
        int configurationSchemaVersion,
        string configurationContentType,
        string configurationPayload,
        ScientificValidationLevel validationLevel,
        DateTimeOffset startedAtUtc,
        DateTimeOffset completedAtUtc,
        string resultContentType,
        string resultPayload,
        IReadOnlyList<string> assumptions,
        IReadOnlyList<string> limitations,
        Guid? replayedFromRunId = null)
    {
        if (runId == Guid.Empty) throw new ArgumentException("A run identity is required.", nameof(runId));
        if (replayedFromRunId == Guid.Empty || replayedFromRunId == runId)
            throw new ArgumentException("A replay source must be a different, non-empty run identity.", nameof(replayedFromRunId));
        LaboratoryId = Required(laboratoryId, nameof(laboratoryId));
        LaboratoryVersion = Required(laboratoryVersion, nameof(laboratoryVersion));
        ExperimentId = Required(experimentId, nameof(experimentId));
        ExperimentVersion = Required(experimentVersion, nameof(experimentVersion));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));
        ConfigurationContentType = Required(configurationContentType, nameof(configurationContentType));
        ArgumentNullException.ThrowIfNull(configurationPayload);
        ArgumentOutOfRangeException.ThrowIfLessThan(configurationSchemaVersion, 1);
        if (!Enum.IsDefined(validationLevel)) throw new ArgumentOutOfRangeException(nameof(validationLevel));
        if (startedAtUtc.Offset != TimeSpan.Zero || completedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Run timestamps must be UTC.");
        if (completedAtUtc < startedAtUtc)
            throw new ArgumentException("Completion cannot precede run start.");
        ResultContentType = Required(resultContentType, nameof(resultContentType));
        ArgumentNullException.ThrowIfNull(resultPayload);

        RunId = runId;
        ReplayedFromRunId = replayedFromRunId;
        ConfigurationSchemaVersion = configurationSchemaVersion;
        ConfigurationPayload = configurationPayload;
        ValidationLevel = validationLevel;
        StartedAtUtc = startedAtUtc;
        CompletedAtUtc = completedAtUtc;
        ResultPayload = resultPayload;
        Assumptions = Snapshot(assumptions, nameof(assumptions));
        Limitations = Snapshot(limitations, nameof(limitations));
        if (Assumptions.Count == 0 || Limitations.Count == 0)
            throw new ArgumentException("Run provenance must preserve assumptions and limitations.");
    }

    public Guid RunId { get; }
    public Guid? ReplayedFromRunId { get; }
    public string LaboratoryId { get; }
    public string LaboratoryVersion { get; }
    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public int ConfigurationSchemaVersion { get; }
    public string ConfigurationContentType { get; }
    public string ConfigurationPayload { get; }
    public ScientificValidationLevel ValidationLevel { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public DateTimeOffset CompletedAtUtc { get; }
    public string ResultContentType { get; }
    public string ResultPayload { get; }
    public IReadOnlyList<string> Assumptions { get; }
    public IReadOnlyList<string> Limitations { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }

    private static IReadOnlyList<string> Snapshot(IEnumerable<string> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        return Array.AsReadOnly(values.Select(value => Required(value, parameterName)).ToArray());
    }
}
