namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Assesses whether one saved experiment configuration is accepted by a target authoring template.
/// This is validation against the target input schema, not a migration or proof of scientific equivalence.
/// </summary>
public sealed record ExperimentConfigurationCompatibility
{
    private ExperimentConfigurationCompatibility(
        string sourceExperimentId,
        string sourceExperimentVersion,
        int sourceSchemaVersion,
        string targetTemplateId,
        string targetTemplateVersion,
        string targetExperimentId,
        string targetExperimentVersion,
        int targetSchemaVersion,
        bool isCompatible,
        string? failureReason)
    {
        SourceExperimentId = sourceExperimentId;
        SourceExperimentVersion = sourceExperimentVersion;
        SourceSchemaVersion = sourceSchemaVersion;
        TargetTemplateId = targetTemplateId;
        TargetTemplateVersion = targetTemplateVersion;
        TargetExperimentId = targetExperimentId;
        TargetExperimentVersion = targetExperimentVersion;
        TargetSchemaVersion = targetSchemaVersion;
        IsCompatible = isCompatible;
        FailureReason = failureReason;
    }

    public string SourceExperimentId { get; }
    public string SourceExperimentVersion { get; }
    public int SourceSchemaVersion { get; }
    public string TargetTemplateId { get; }
    public string TargetTemplateVersion { get; }
    public string TargetExperimentId { get; }
    public string TargetExperimentVersion { get; }
    public int TargetSchemaVersion { get; }

    /// <summary>True only when the saved payload passes the target template's declared input schema.</summary>
    public bool IsCompatible { get; }

    /// <summary>A concise reason when the saved configuration is not accepted by the target template.</summary>
    public string? FailureReason { get; }

    /// <summary>
    /// Assesses one immutable saved configuration against a target template. Experiment identity must match;
    /// experiment and schema versions are retained as provenance but are not required to be equal when the
    /// target schema accepts the payload.
    /// </summary>
    public static ExperimentConfigurationCompatibility Assess(
        ExperimentConfiguration source,
        ExperimentAuthoringTemplate target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        string? failure = null;
        if (!StringComparer.Ordinal.Equals(source.ExperimentId, target.Experiment.Id))
            failure = "The saved configuration belongs to a different experiment.";
        else if (!StringComparer.Ordinal.Equals(source.ContentType, target.DefaultConfiguration.ContentType))
            failure = "The saved configuration and target template use different content types.";
        else if (!StringComparer.Ordinal.Equals(source.ContentType, "application/json"))
            failure = "Only application/json configurations can be checked against the parameter schema.";
        else
        {
            try
            {
                target.ParameterSchema.ValidateJson(source.Payload);
            }
            catch (ArgumentException exception)
            {
                failure = exception.Message;
            }
            catch (System.Text.Json.JsonException exception)
            {
                failure = $"The saved configuration is not valid JSON: {exception.Message}";
            }
        }

        return new ExperimentConfigurationCompatibility(
            source.ExperimentId,
            source.ExperimentVersion,
            source.SchemaVersion,
            target.Id,
            target.Version,
            target.Experiment.Id,
            target.Experiment.Version,
            target.ParameterSchema.SchemaVersion,
            failure is null,
            failure);
    }
}
