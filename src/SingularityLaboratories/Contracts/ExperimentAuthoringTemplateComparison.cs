using System.Text.Json;

namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Describes a deterministic, host-neutral comparison between two immutable authoring-template versions.
/// </summary>
public sealed record ExperimentAuthoringTemplateComparison
{
    private ExperimentAuthoringTemplateComparison(
        string beforeTemplateId,
        string beforeTemplateVersion,
        string afterTemplateId,
        string afterTemplateVersion,
        IReadOnlyList<ExperimentAuthoringTemplateDifference> differences)
    {
        BeforeTemplateId = beforeTemplateId;
        BeforeTemplateVersion = beforeTemplateVersion;
        AfterTemplateId = afterTemplateId;
        AfterTemplateVersion = afterTemplateVersion;
        Differences = differences;
    }

    public string BeforeTemplateId { get; }
    public string BeforeTemplateVersion { get; }
    public string AfterTemplateId { get; }
    public string AfterTemplateVersion { get; }
    public IReadOnlyList<ExperimentAuthoringTemplateDifference> Differences { get; }
    public bool IsIdentical => Differences.Count == 0;

    /// <summary>Compares template metadata, experiment provenance, disclosures, schema, parameters, and defaults.</summary>
    public static ExperimentAuthoringTemplateComparison Compare(
        ExperimentAuthoringTemplate before,
        ExperimentAuthoringTemplate after)
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);

        var differences = new List<ExperimentAuthoringTemplateDifference>();
        Add("template.id", before.Id, after.Id);
        Add("template.version", before.Version, after.Version);
        Add("template.name", before.Name, after.Name);

        var left = before.Experiment;
        var right = after.Experiment;
        Add("experiment.id", left.Id, right.Id);
        Add("experiment.version", left.Version, right.Version);
        Add("experiment.name", left.Name, right.Name);
        Add("experiment.modelId", left.ModelId, right.ModelId);
        Add("experiment.modelVersion", left.ModelVersion, right.ModelVersion);
        Add("experiment.validationLevel", left.ValidationLevel, right.ValidationLevel);
        Add("experiment.assumptions", left.Assumptions, right.Assumptions);
        Add("experiment.limitations", left.Limitations, right.Limitations);
        Add("experiment.safetyMetadata", left.SafetyMetadata, right.SafetyMetadata);
        Add("experiment.validationAssessment", left.ValidationAssessment, right.ValidationAssessment);

        Add("schema.version", before.ParameterSchema.SchemaVersion, after.ParameterSchema.SchemaVersion);
        var beforeParameters = before.ParameterSchema.Parameters.ToDictionary(parameter => parameter.Id, StringComparer.Ordinal);
        var afterParameters = after.ParameterSchema.Parameters.ToDictionary(parameter => parameter.Id, StringComparer.Ordinal);
        foreach (var id in beforeParameters.Keys.Union(afterParameters.Keys, StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal))
        {
            var path = $"schema.parameters.{id}";
            var hasBefore = beforeParameters.TryGetValue(id, out var oldParameter);
            var hasAfter = afterParameters.TryGetValue(id, out var newParameter);
            if (!hasBefore)
                differences.Add(new ExperimentAuthoringTemplateDifference(path, ExperimentAuthoringTemplateDifferenceKind.Added, null, Serialize(newParameter)));
            else if (!hasAfter)
                differences.Add(new ExperimentAuthoringTemplateDifference(path, ExperimentAuthoringTemplateDifferenceKind.Removed, Serialize(oldParameter), null));
            else
                Add(path, oldParameter, newParameter);
        }

        Add("defaultConfiguration.contentType", before.DefaultConfiguration.ContentType, after.DefaultConfiguration.ContentType);
        Add("defaultConfiguration.payload", before.DefaultConfiguration.Payload, after.DefaultConfiguration.Payload);

        return new ExperimentAuthoringTemplateComparison(
            before.Id, before.Version, after.Id, after.Version,
            Array.AsReadOnly(differences.ToArray()));

        void Add<T>(string path, T oldValue, T newValue)
        {
            var oldJson = Serialize(oldValue);
            var newJson = Serialize(newValue);
            if (!StringComparer.Ordinal.Equals(oldJson, newJson))
                differences.Add(new ExperimentAuthoringTemplateDifference(
                    path, ExperimentAuthoringTemplateDifferenceKind.Changed, oldJson, newJson));
        }
    }

    private static string? Serialize<T>(T value) =>
        value is null ? null : JsonSerializer.Serialize(value);
}

/// <summary>One stable-path change found between two authoring-template versions.</summary>
public sealed record ExperimentAuthoringTemplateDifference(
    string Path,
    ExperimentAuthoringTemplateDifferenceKind Kind,
    string? Before,
    string? After);

/// <summary>How a value differs between two authoring-template versions.</summary>
public enum ExperimentAuthoringTemplateDifferenceKind
{
    Added = 0,
    Removed = 1,
    Changed = 2
}
