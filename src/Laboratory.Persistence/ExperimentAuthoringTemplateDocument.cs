using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Persistence-only DTO. It deliberately avoids serializing domain constructors directly:
/// domain constructors accept enumerable inputs while exposing immutable read-only collections.
/// </summary>
internal sealed record ExperimentAuthoringTemplateDocument
{
    public required string Id { get; init; }
    public required string Version { get; init; }
    public required string Name { get; init; }
    public required ExperimentDefinitionDocument Experiment { get; init; }
    public required ParameterSchemaDocument ParameterSchema { get; init; }
    public required ConfigurationDocument DefaultConfiguration { get; init; }

    public static ExperimentAuthoringTemplateDocument From(ExperimentAuthoringTemplate template) => new()
    {
        Id = template.Id,
        Version = template.Version,
        Name = template.Name,
        Experiment = ExperimentDefinitionDocument.From(template.Experiment),
        ParameterSchema = ParameterSchemaDocument.From(template.ParameterSchema),
        DefaultConfiguration = new ConfigurationDocument
        {
            ExperimentId = template.DefaultConfiguration.ExperimentId,
            ExperimentVersion = template.DefaultConfiguration.ExperimentVersion,
            SchemaVersion = template.DefaultConfiguration.SchemaVersion,
            ContentType = template.DefaultConfiguration.ContentType,
            Payload = template.DefaultConfiguration.Payload
        }
    };

    public ExperimentAuthoringTemplate ToTemplate()
    {
        var experiment = Experiment.ToDefinition();
        var schema = ParameterSchema.ToSchema();
        var configuration = new ExperimentConfiguration(
            DefaultConfiguration.ExperimentId,
            DefaultConfiguration.ExperimentVersion,
            DefaultConfiguration.SchemaVersion,
            DefaultConfiguration.ContentType,
            DefaultConfiguration.Payload);
        return new ExperimentAuthoringTemplate(Id, Version, Name, experiment, schema, configuration);
    }
}

internal sealed record ExperimentDefinitionDocument
{
    public required string Id { get; init; }
    public required string Version { get; init; }
    public required string Name { get; init; }
    public required string ModelId { get; init; }
    public required string ModelVersion { get; init; }
    public required ScientificValidationLevel ValidationLevel { get; init; }
    public required string[] Assumptions { get; init; }
    public required string[] Limitations { get; init; }
    public SafetyMetadataDocument? SafetyMetadata { get; init; }
    public ValidationAssessmentDocument? ValidationAssessment { get; init; }

    public static ExperimentDefinitionDocument From(ExperimentDefinition value) => new()
    {
        Id = value.Id, Version = value.Version, Name = value.Name, ModelId = value.ModelId,
        ModelVersion = value.ModelVersion, ValidationLevel = value.ValidationLevel,
        Assumptions = value.Assumptions.ToArray(), Limitations = value.Limitations.ToArray(),
        SafetyMetadata = value.SafetyMetadata is null ? null : SafetyMetadataDocument.From(value.SafetyMetadata),
        ValidationAssessment = value.ValidationAssessment is null ? null : ValidationAssessmentDocument.From(value.ValidationAssessment)
    };

    public ExperimentDefinition ToDefinition() => new(
        Id, Version, Name, ModelId, ModelVersion, ValidationLevel, Assumptions, Limitations,
        SafetyMetadata?.ToMetadata(), ValidationAssessment?.ToAssessment());
}

internal sealed record SafetyMetadataDocument
{
    public required ExperimentRiskLevel RiskLevel { get; init; }
    public required string[] Hazards { get; init; }
    public required string[] Precautions { get; init; }
    public required string[] ProhibitedUses { get; init; }

    public static SafetyMetadataDocument From(ExperimentSafetyMetadata value) => new()
    {
        RiskLevel = value.RiskLevel, Hazards = value.Hazards.ToArray(),
        Precautions = value.Precautions.ToArray(), ProhibitedUses = value.ProhibitedUses.ToArray()
    };

    public ExperimentSafetyMetadata ToMetadata() => new(RiskLevel, Hazards, Precautions, ProhibitedUses);
}

internal sealed record ValidationAssessmentDocument
{
    public required string ExperimentId { get; init; }
    public required string ExperimentVersion { get; init; }
    public required string ModelId { get; init; }
    public required string ModelVersion { get; init; }
    public required ExperimentValidationState State { get; init; }
    public required DateTimeOffset AssessedAtUtc { get; init; }
    public required EvidenceDocument[] Evidence { get; init; }
    public string? Reviewer { get; init; }
    public string? DecisionNotes { get; init; }

    public static ValidationAssessmentDocument From(ExperimentValidationAssessment value) => new()
    {
        ExperimentId = value.ExperimentId, ExperimentVersion = value.ExperimentVersion,
        ModelId = value.ModelId, ModelVersion = value.ModelVersion, State = value.State,
        AssessedAtUtc = value.AssessedAtUtc, Evidence = value.Evidence.Select(EvidenceDocument.From).ToArray(),
        Reviewer = value.Reviewer, DecisionNotes = value.DecisionNotes
    };

    public ExperimentValidationAssessment ToAssessment() => new(
        ExperimentId, ExperimentVersion, ModelId, ModelVersion, State, AssessedAtUtc,
        Evidence.Select(item => item.ToEvidence()).ToArray(), Reviewer, DecisionNotes);
}

internal sealed record EvidenceDocument
{
    public required string Id { get; init; }
    public required string Description { get; init; }
    public required DateTimeOffset RecordedAtUtc { get; init; }
    public string? Source { get; init; }

    public static EvidenceDocument From(ExperimentValidationEvidence value) => new()
    {
        Id = value.Id, Description = value.Description, RecordedAtUtc = value.RecordedAtUtc, Source = value.Source
    };

    public ExperimentValidationEvidence ToEvidence() => new(Id, Description, RecordedAtUtc, Source);
}

internal sealed record ParameterSchemaDocument
{
    public required string ExperimentId { get; init; }
    public required string ExperimentVersion { get; init; }
    public required int SchemaVersion { get; init; }
    public required ParameterDefinitionDocument[] Parameters { get; init; }

    public static ParameterSchemaDocument From(ExperimentParameterSchema value) => new()
    {
        ExperimentId = value.ExperimentId, ExperimentVersion = value.ExperimentVersion,
        SchemaVersion = value.SchemaVersion, Parameters = value.Parameters.Select(ParameterDefinitionDocument.From).ToArray()
    };

    public ExperimentParameterSchema ToSchema() => new(
        ExperimentId, ExperimentVersion, SchemaVersion, Parameters.Select(item => item.ToDefinition()).ToArray());
}

internal sealed record ParameterDefinitionDocument
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ExperimentParameterType Type { get; init; }
    public required bool Required { get; init; }
    public string? UnitSymbol { get; init; }
    public double? Minimum { get; init; }
    public double? Maximum { get; init; }
    public required string[] AllowedValues { get; init; }

    public static ParameterDefinitionDocument From(ExperimentParameterDefinition value) => new()
    {
        Id = value.Id, Name = value.Name, Description = value.Description, Type = value.Type,
        Required = value.Required, UnitSymbol = value.UnitSymbol, Minimum = value.Minimum,
        Maximum = value.Maximum, AllowedValues = value.AllowedValues.ToArray()
    };

    public ExperimentParameterDefinition ToDefinition() => new(
        Id, Name, Description, Type, Required, UnitSymbol, Minimum, Maximum, AllowedValues);
}

internal sealed record ConfigurationDocument
{
    public required string ExperimentId { get; init; }
    public required string ExperimentVersion { get; init; }
    public required int SchemaVersion { get; init; }
    public required string ContentType { get; init; }
    public required string Payload { get; init; }
}
