namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Declares a versioned experiment capability and the model identity it invokes.</summary>
public sealed record ExperimentDefinition
{
    public ExperimentDefinition(
        string id,
        string version,
        string name,
        string modelId,
        string modelVersion,
        ScientificValidationLevel validationLevel,
        IEnumerable<string> assumptions,
        IEnumerable<string> limitations)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Name = Required(name, nameof(name));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));
        ValidationLevel = validationLevel;
        Assumptions = Snapshot(assumptions, nameof(assumptions));
        Limitations = Snapshot(limitations, nameof(limitations));
        if (Assumptions.Count == 0)
            throw new ArgumentException("At least one model assumption must be disclosed.", nameof(assumptions));
        if (Limitations.Count == 0)
            throw new ArgumentException("At least one model limitation must be disclosed.", nameof(limitations));
        if (!Enum.IsDefined(validationLevel))
            throw new ArgumentOutOfRangeException(nameof(validationLevel));
    }

    public string Id { get; }
    public string Version { get; }
    public string Name { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public ScientificValidationLevel ValidationLevel { get; }
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
        var snapshot = values.Select(value => Required(value, parameterName)).ToArray();
        if (snapshot.Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Disclosure entries must be unique.", parameterName);
        return Array.AsReadOnly(snapshot);
    }
}

/// <summary>Scientific evidence level; commercial tier and visual quality do not alter this value.</summary>
public enum ScientificValidationLevel
{
    ConceptDemonstration = 0,
    EducationalModel = 1,
    ValidatedModel = 2,
    ResearchReviewedModel = 3
}
