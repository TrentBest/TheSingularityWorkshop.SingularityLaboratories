namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// A reusable, versioned starting point for authoring an experiment. A template binds
/// one experiment definition to its parameter schema and a validated default configuration.
/// </summary>
public sealed record ExperimentAuthoringTemplate
{
    public ExperimentAuthoringTemplate(
        string id,
        string version,
        string name,
        ExperimentDefinition experiment,
        ExperimentParameterSchema parameterSchema,
        ExperimentConfiguration defaultConfiguration)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Name = Required(name, nameof(name));
        ArgumentNullException.ThrowIfNull(experiment);
        ArgumentNullException.ThrowIfNull(parameterSchema);
        ArgumentNullException.ThrowIfNull(defaultConfiguration);

        if (!StringComparer.Ordinal.Equals(experiment.Id, parameterSchema.ExperimentId) ||
            !StringComparer.Ordinal.Equals(experiment.Version, parameterSchema.ExperimentVersion))
            throw new ArgumentException("The parameter schema must target the template's exact experiment definition.", nameof(parameterSchema));

        if (!StringComparer.Ordinal.Equals(experiment.Id, defaultConfiguration.ExperimentId) ||
            !StringComparer.Ordinal.Equals(experiment.Version, defaultConfiguration.ExperimentVersion))
            throw new ArgumentException("The default configuration must target the template's exact experiment definition.", nameof(defaultConfiguration));

        if (parameterSchema.SchemaVersion != defaultConfiguration.SchemaVersion)
            throw new ArgumentException("The default configuration and parameter schema versions must match.", nameof(defaultConfiguration));

        parameterSchema.ValidateJson(defaultConfiguration.Payload);

        Experiment = experiment;
        ParameterSchema = parameterSchema;
        DefaultConfiguration = defaultConfiguration;
    }

    public string Id { get; }
    public string Version { get; }
    public string Name { get; }
    public ExperimentDefinition Experiment { get; }
    public ExperimentParameterSchema ParameterSchema { get; }
    public ExperimentConfiguration DefaultConfiguration { get; }

    /// <summary>Creates a new immutable configuration after validating it against this template.</summary>
    public ExperimentConfiguration CreateConfiguration(string payload)
    {
        ParameterSchema.ValidateJson(payload);
        return new ExperimentConfiguration(
            Experiment.Id,
            Experiment.Version,
            ParameterSchema.SchemaVersion,
            DefaultConfiguration.ContentType,
            payload);
    }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
