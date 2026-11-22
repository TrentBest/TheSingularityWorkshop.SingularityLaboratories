using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentAuthoringTemplateSerializerTests
{
    private static ExperimentAuthoringTemplate CreateTemplate()
    {
        var experiment = new ExperimentDefinition(
            "thermal-coupling", "1.0.0", "Electrical to thermal coupling", "physics.thermal", "2.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."],
            new ExperimentSafetyMetadata(
                ExperimentRiskLevel.Low,
                ["Hot surfaces may occur in physical demonstrations."],
                ["Use simulation-only defaults."],
                ["Do not treat this educational model as appliance certification."]));
        var schema = new ExperimentParameterSchema(
            experiment.Id, experiment.Version, 1,
            [
                new ExperimentParameterDefinition("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number, minimum: 0),
                new ExperimentParameterDefinition("resistanceOhm", "Resistance", "Load resistance.", ExperimentParameterType.Number, minimum: 0.001)
            ]);
        var configuration = new ExperimentConfiguration(
            experiment.Id, experiment.Version, 1, "application/json",
            "{\"voltageV\":12,\"resistanceOhm\":6}");
        return new ExperimentAuthoringTemplate("thermal-template", "1.0.0", "Introductory thermal experiment", experiment, schema, configuration);
    }

    [Test]
    public void ExportImport_RoundTripsVersionedTemplateAndSafetyMetadata()
    {
        var original = CreateTemplate();

        var json = ExperimentAuthoringTemplateSerializer.ExportToJson(original);
        var imported = ExperimentAuthoringTemplateSerializer.ImportFromJson(json);

        Assert.That(imported.Id, Is.EqualTo(original.Id));
        Assert.That(imported.Version, Is.EqualTo(original.Version));
        Assert.That(imported.Experiment.ModelId, Is.EqualTo(original.Experiment.ModelId));
        Assert.That(imported.Experiment.SafetyMetadata!.RiskLevel, Is.EqualTo(ExperimentRiskLevel.Low));
        Assert.That(imported.ParameterSchema.Parameters.Select(item => item.Id),
            Is.EqualTo(original.ParameterSchema.Parameters.Select(item => item.Id)));
        Assert.That(imported.DefaultConfiguration.Payload, Is.EqualTo(original.DefaultConfiguration.Payload));
    }

    [Test]
    public void ExportToJson_IsDeterministicForTheSameTemplate()
    {
        var template = CreateTemplate();

        Assert.That(
            ExperimentAuthoringTemplateSerializer.ExportToJson(template),
            Is.EqualTo(ExperimentAuthoringTemplateSerializer.ExportToJson(template)));
    }

    [Test]
    public void ImportFromJson_RejectsTamperedPayload()
    {
        var json = ExperimentAuthoringTemplateSerializer.ExportToJson(CreateTemplate());
        var tampered = json.Replace("thermal-template", "altered-template", StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(() =>
            ExperimentAuthoringTemplateSerializer.ImportFromJson(tampered));
    }

    [Test]
    public void ImportFromJson_RejectsUnsupportedFormatVersion()
    {
        var json = ExperimentAuthoringTemplateSerializer.ExportToJson(CreateTemplate())
            .Replace("\"formatVersion\": 1", "\"formatVersion\": 2", StringComparison.Ordinal);

        Assert.Throws<NotSupportedException>(() =>
            ExperimentAuthoringTemplateSerializer.ImportFromJson(json));
    }
}
