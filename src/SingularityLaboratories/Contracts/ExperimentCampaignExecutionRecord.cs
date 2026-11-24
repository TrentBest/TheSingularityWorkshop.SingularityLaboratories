namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Aggregate outcome of executing one immutable experiment campaign definition.</summary>
public enum ExperimentCampaignExecutionStatus
{
    Completed = 1,
    PartiallyCompleted = 2,
    Failed = 3,
    Cancelled = 4
}

/// <summary>Outcome of one scenario in a campaign execution.</summary>
public enum ExperimentCampaignScenarioStatus
{
    Completed = 1,
    Failed = 2,
    Cancelled = 3,
    NotRun = 4
}

/// <summary>Immutable execution evidence for one campaign scenario.</summary>
public sealed record ExperimentCampaignScenarioResult
{
    public ExperimentCampaignScenarioResult(
        string scenarioId,
        ExperimentCampaignScenarioStatus status,
        ExperimentRunRecord? run,
        string? failureType = null,
        string? failureMessage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scenarioId);
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));
        if (status == ExperimentCampaignScenarioStatus.Completed && run is null)
            throw new ArgumentException("A completed scenario must contain its experiment run.", nameof(run));
        if (status != ExperimentCampaignScenarioStatus.Completed && run is not null)
            throw new ArgumentException("Only a completed scenario can contain a successful experiment run.", nameof(run));
        if (status == ExperimentCampaignScenarioStatus.Failed &&
            (string.IsNullOrWhiteSpace(failureType) || string.IsNullOrWhiteSpace(failureMessage)))
            throw new ArgumentException("A failed scenario must preserve its failure type and message.");
        if (status != ExperimentCampaignScenarioStatus.Failed &&
            (failureType is not null || failureMessage is not null))
            throw new ArgumentException("Failure details are only valid for failed scenarios.");

        ScenarioId = scenarioId.Trim();
        Status = status;
        Run = run;
        FailureType = failureType;
        FailureMessage = failureMessage;
    }

    public string ScenarioId { get; }
    public ExperimentCampaignScenarioStatus Status { get; }
    public ExperimentRunRecord? Run { get; }
    public string? FailureType { get; }
    public string? FailureMessage { get; }
}

/// <summary>
/// Immutable campaign execution envelope. Successful scenarios retain their complete run records;
/// failed and unstarted scenarios remain explicit rather than disappearing from the evidence.
/// </summary>
public sealed record ExperimentCampaignExecutionRecord
{
    public ExperimentCampaignExecutionRecord(
        Guid executionId,
        string campaignId,
        string campaignVersion,
        string laboratoryId,
        string laboratoryVersion,
        string experimentId,
        string experimentVersion,
        string modelId,
        string modelVersion,
        DateTimeOffset startedAtUtc,
        DateTimeOffset completedAtUtc,
        ExperimentCampaignExecutionStatus status,
        IEnumerable<ExperimentCampaignScenarioResult> scenarios)
    {
        if (executionId == Guid.Empty)
            throw new ArgumentException("An execution identity is required.", nameof(executionId));
        CampaignId = Required(campaignId, nameof(campaignId));
        CampaignVersion = Required(campaignVersion, nameof(campaignVersion));
        LaboratoryId = Required(laboratoryId, nameof(laboratoryId));
        LaboratoryVersion = Required(laboratoryVersion, nameof(laboratoryVersion));
        ExperimentId = Required(experimentId, nameof(experimentId));
        ExperimentVersion = Required(experimentVersion, nameof(experimentVersion));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));
        if (startedAtUtc.Offset != TimeSpan.Zero || completedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Campaign execution timestamps must be UTC.");
        if (completedAtUtc < startedAtUtc)
            throw new ArgumentException("Campaign completion cannot precede its start.");
        if (!Enum.IsDefined(status))
            throw new ArgumentOutOfRangeException(nameof(status));
        ArgumentNullException.ThrowIfNull(scenarios);
        var snapshot = scenarios.ToArray();
        if (snapshot.Length == 0 || snapshot.Any(scenario => scenario is null))
            throw new ArgumentException("A campaign execution requires non-null scenario results.", nameof(scenarios));
        if (snapshot.Select(scenario => scenario.ScenarioId).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Scenario result identifiers must be unique.", nameof(scenarios));
        if (snapshot.Where(scenario => scenario.Run is not null).Any(scenario =>
            !string.Equals(scenario.Run!.LaboratoryId, LaboratoryId, StringComparison.Ordinal) ||
            !string.Equals(scenario.Run.LaboratoryVersion, LaboratoryVersion, StringComparison.Ordinal) ||
            !string.Equals(scenario.Run.ExperimentId, ExperimentId, StringComparison.Ordinal) ||
            !string.Equals(scenario.Run.ExperimentVersion, ExperimentVersion, StringComparison.Ordinal) ||
            !string.Equals(scenario.Run.ModelId, ModelId, StringComparison.Ordinal) ||
            !string.Equals(scenario.Run.ModelVersion, ModelVersion, StringComparison.Ordinal)))
            throw new ArgumentException("Every successful scenario run must match the campaign execution's exact laboratory, experiment, and model versions.", nameof(scenarios));
        var completed = snapshot.Count(scenario => scenario.Status == ExperimentCampaignScenarioStatus.Completed);
        var failed = snapshot.Count(scenario => scenario.Status == ExperimentCampaignScenarioStatus.Failed);
        var cancelled = snapshot.Count(scenario => scenario.Status == ExperimentCampaignScenarioStatus.Cancelled);
        var notRun = snapshot.Count(scenario => scenario.Status == ExperimentCampaignScenarioStatus.NotRun);
        var expectedStatus = cancelled > 0
            ? ExperimentCampaignExecutionStatus.Cancelled
            : failed == 0 && notRun == 0
                ? ExperimentCampaignExecutionStatus.Completed
                : completed > 0
                    ? ExperimentCampaignExecutionStatus.PartiallyCompleted
                    : ExperimentCampaignExecutionStatus.Failed;
        if (status != expectedStatus)
            throw new ArgumentException("The aggregate status does not match the scenario outcomes.", nameof(status));

        ExecutionId = executionId;
        StartedAtUtc = startedAtUtc;
        CompletedAtUtc = completedAtUtc;
        Status = status;
        Scenarios = Array.AsReadOnly(snapshot);
    }

    public Guid ExecutionId { get; }
    public string CampaignId { get; }
    public string CampaignVersion { get; }
    public string LaboratoryId { get; }
    public string LaboratoryVersion { get; }
    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public DateTimeOffset CompletedAtUtc { get; }
    public ExperimentCampaignExecutionStatus Status { get; }
    public IReadOnlyList<ExperimentCampaignScenarioResult> Scenarios { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
