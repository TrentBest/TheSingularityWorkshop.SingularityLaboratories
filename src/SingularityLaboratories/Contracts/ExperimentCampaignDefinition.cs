namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Scientific purpose of a scenario within an experiment campaign.</summary>
public enum ExperimentScenarioKind
{
    Baseline = 1,
    Nominal = 2,
    Boundary = 3,
    Sensitivity = 4,
    Tolerance = 5,
    Disturbance = 6,
    Failure = 7,
    Adversarial = 8
}

/// <summary>
/// One explicitly reasoned scenario and its immutable, version-bound experiment configuration.
/// </summary>
/// <remarks>
/// The scenario contract does not generate parameter values, execute a model, or imply that a
/// model supports the described condition. Model-domain validation remains a separate gate.
/// </remarks>
public sealed record ExperimentCampaignScenario
{
    public ExperimentCampaignScenario(
        string id,
        string name,
        ExperimentScenarioKind kind,
        string rationale,
        ExperimentConfiguration configuration)
    {
        Id = Required(id, nameof(id));
        Name = Required(name, nameof(name));
        if (!Enum.IsDefined(kind))
            throw new ArgumentOutOfRangeException(nameof(kind));
        Kind = kind;
        Rationale = Required(rationale, nameof(rationale));
        ArgumentNullException.ThrowIfNull(configuration);
        Configuration = configuration;
    }

    public string Id { get; }
    public string Name { get; }
    public ExperimentScenarioKind Kind { get; }
    public string Rationale { get; }
    public ExperimentConfiguration Configuration { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>
/// Immutable plan for a set of related experiment runs answering one scientific or engineering
/// question. Every scenario is bound to the same exact experiment version.
/// </summary>
/// <remarks>
/// A campaign definition is not a run, reservation, safety approval, or scientific certification.
/// It records intent and scenario configurations; execution and result provenance remain separate.
/// </remarks>
public sealed record ExperimentCampaignDefinition
{
    public ExperimentCampaignDefinition(
        string id,
        string version,
        string objective,
        string experimentId,
        string experimentVersion,
        string modelId,
        string modelVersion,
        IReadOnlyList<string> acceptanceCriteria,
        IReadOnlyList<ExperimentCampaignScenario> scenarios)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Objective = Required(objective, nameof(objective));
        ExperimentId = Required(experimentId, nameof(experimentId));
        ExperimentVersion = Required(experimentVersion, nameof(experimentVersion));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));

        AcceptanceCriteria = Snapshot(acceptanceCriteria, nameof(acceptanceCriteria));
        if (AcceptanceCriteria.Count == 0)
            throw new ArgumentException("At least one measurable acceptance criterion is required.", nameof(acceptanceCriteria));

        ArgumentNullException.ThrowIfNull(scenarios);
        var scenarioSnapshot = scenarios.ToArray();
        if (scenarioSnapshot.Length < 2)
            throw new ArgumentException("A campaign requires a baseline and at least one additional scenario.", nameof(scenarios));
        if (scenarioSnapshot.Any(scenario => scenario is null))
            throw new ArgumentException("Scenarios cannot contain null entries.", nameof(scenarios));
        if (scenarioSnapshot.Select(scenario => scenario.Id).Distinct(StringComparer.Ordinal).Count() != scenarioSnapshot.Length)
            throw new ArgumentException("Scenario identifiers must be unique within a campaign.", nameof(scenarios));
        if (scenarioSnapshot.Count(scenario => scenario.Kind == ExperimentScenarioKind.Baseline) != 1)
            throw new ArgumentException("A campaign requires exactly one baseline scenario.", nameof(scenarios));
        if (scenarioSnapshot.Any(scenario =>
            !string.Equals(scenario.Configuration.ExperimentId, ExperimentId, StringComparison.Ordinal) ||
            !string.Equals(scenario.Configuration.ExperimentVersion, ExperimentVersion, StringComparison.Ordinal)))
        {
            throw new ArgumentException("Every scenario configuration must target the campaign's exact experiment identity and version.", nameof(scenarios));
        }

        Scenarios = Array.AsReadOnly(scenarioSnapshot);
    }

    public string Id { get; }
    public string Version { get; }
    public string Objective { get; }
    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public IReadOnlyList<string> AcceptanceCriteria { get; }
    public IReadOnlyList<ExperimentCampaignScenario> Scenarios { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }

    private static IReadOnlyList<string> Snapshot(IEnumerable<string> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        var snapshot = values.Select(value => Required(value, parameterName)).ToArray();
        if (snapshot.Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Acceptance criteria must be unique.", parameterName);
        return Array.AsReadOnly(snapshot);
    }
}
