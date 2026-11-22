namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Assesses whether the source template's default configuration is accepted by the target template's schema.
/// This is a narrow compatibility check, not a migration or proof that every historical configuration is compatible.
/// </summary>
public sealed record ExperimentDefaultConfigurationCompatibility
{
    private ExperimentDefaultConfigurationCompatibility(
        string sourceTemplateId,
        string sourceTemplateVersion,
        string targetTemplateId,
        string targetTemplateVersion,
        bool isCompatible,
        string? failureReason)
    {
        SourceTemplateId = sourceTemplateId;
        SourceTemplateVersion = sourceTemplateVersion;
        TargetTemplateId = targetTemplateId;
        TargetTemplateVersion = targetTemplateVersion;
        IsCompatible = isCompatible;
        FailureReason = failureReason;
    }

    public string SourceTemplateId { get; }
    public string SourceTemplateVersion { get; }
    public string TargetTemplateId { get; }
    public string TargetTemplateVersion { get; }

    /// <summary>True only when the source default's content type and payload pass the target schema.</summary>
    public bool IsCompatible { get; }

    /// <summary>A concise reason when the source default cannot be checked against the target schema.</summary>
    public string? FailureReason { get; }

    public static ExperimentDefaultConfigurationCompatibility Assess(
        ExperimentAuthoringTemplate source,
        ExperimentAuthoringTemplate target)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(target);

        string? failure = null;
        var configuration = source.DefaultConfiguration;

        if (!StringComparer.Ordinal.Equals(configuration.ExperimentId, target.Experiment.Id))
            failure = "The source configuration belongs to a different experiment.";
        else if (!StringComparer.Ordinal.Equals(configuration.ContentType, target.DefaultConfiguration.ContentType))
            failure = "The source and target default configurations use different content types.";
        else if (!StringComparer.Ordinal.Equals(configuration.ContentType, "application/json"))
            failure = "Only application/json configurations can be checked against the parameter schema.";
        else
        {
            try
            {
                target.ParameterSchema.ValidateJson(configuration.Payload);
            }
            catch (ArgumentException exception)
            {
                failure = exception.Message;
            }
            catch (System.Text.Json.JsonException exception)
            {
                failure = $"The source configuration is not valid JSON: {exception.Message}";
            }
        }

        return new ExperimentDefaultConfigurationCompatibility(
            source.Id, source.Version, target.Id, target.Version,
            failure is null, failure);
    }
}
