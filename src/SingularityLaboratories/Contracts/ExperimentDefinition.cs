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
        IEnumerable<string> limitations,
        ExperimentSafetyMetadata? safetyMetadata = null,
        ExperimentValidationAssessment? validationAssessment = null,
        IEnumerable<ExperimentPhysicsDependency>? physicsDependencies = null,
        IEnumerable<ExperimentMicroBundleRequirement>? microBundleRequirements = null)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Name = Required(name, nameof(name));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));
        ValidationLevel = validationLevel;
        Assumptions = Snapshot(assumptions, nameof(assumptions));
        Limitations = Snapshot(limitations, nameof(limitations));
        SafetyMetadata = safetyMetadata;
        ValidationAssessment = validationAssessment;
        PhysicsDependencies = Snapshot(physicsDependencies ?? [], nameof(physicsDependencies));
        MicroBundleRequirements = Snapshot(microBundleRequirements ?? [], nameof(microBundleRequirements));
        if (validationAssessment is not null &&
            (validationAssessment.ExperimentId != Id ||
             validationAssessment.ExperimentVersion != Version ||
             validationAssessment.ModelId != ModelId ||
             validationAssessment.ModelVersion != ModelVersion))
            throw new ArgumentException(
                "A validation assessment must target this exact experiment and model version.",
                nameof(validationAssessment));
        if (Assumptions.Count == 0)
            throw new ArgumentException("At least one model assumption must be disclosed.", nameof(assumptions));
        if (Limitations.Count == 0)
            throw new ArgumentException("At least one model limitation must be disclosed.", nameof(limitations));
        if (!Enum.IsDefined(validationLevel))
            throw new ArgumentOutOfRangeException(nameof(validationLevel));
        if (PhysicsDependencies.Select(x => x.CapabilityId).Distinct(StringComparer.Ordinal).Count() != PhysicsDependencies.Count)
            throw new ArgumentException("Physics capability identifiers must be unique within an experiment.", nameof(physicsDependencies));
        if (MicroBundleRequirements.Select(x => (x.BundleId, x.Version, x.ProviderId)).Distinct().Count() != MicroBundleRequirements.Count)
            throw new ArgumentException("MicroBundle requirements must be unique.", nameof(microBundleRequirements));
    }

    public string Id { get; }
    public string Version { get; }
    public string Name { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public ScientificValidationLevel ValidationLevel { get; }
    public IReadOnlyList<string> Assumptions { get; }
    public IReadOnlyList<string> Limitations { get; }
    public ExperimentSafetyMetadata? SafetyMetadata { get; }
    public ExperimentValidationAssessment? ValidationAssessment { get; }
    /// <summary>Physics capabilities declared by the experiment; declarations are not execution evidence.</summary>
    public IReadOnlyList<ExperimentPhysicsDependency> PhysicsDependencies { get; }
    /// <summary>MicroBundle capabilities required by the experiment; loaded or executing state is reported separately.</summary>
    public IReadOnlyList<ExperimentMicroBundleRequirement> MicroBundleRequirements { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }

    private static IReadOnlyList<T> Snapshot<T>(IEnumerable<T> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        var snapshot = values.ToArray();
        if (snapshot.Any(value => value is null))
            throw new ArgumentException("Entries cannot contain null values.", parameterName);
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
