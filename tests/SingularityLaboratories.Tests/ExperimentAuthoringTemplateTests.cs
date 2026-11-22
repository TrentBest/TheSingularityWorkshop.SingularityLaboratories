using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentAuthoringTemplateTests
{
    private static ExperimentDefinition CreateExperiment(string id = "thermal-coupling", string version = "1.0.0") =>
        new(id, version, "Electrical to thermal coupling", "physics.thermal", "2.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);

    private static ExperimentParameterSchema CreateSchema(string id = "thermal-coupling", string version = "1.0.0") =>
        new(id, version, 1,
        [
            new ExperimentParameterDefinition("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number, minimum: 0),
            new ExperimentParameterDefinition("resistanceOhm", "Resistance", "Load resistance.", ExperimentParameterType.Number, minimum: 0.001)
        ]);

    private static ExperimentConfiguration CreateConfiguration(
        string id = "thermal-coupling",
        string version = "1.0.0",
        int schemaVersion = 1,
        string payload = "{\"voltageV\":12,\"resistanceOhm\":6}") =>
        new(id, version, schemaVersion, "application/json", payload);

    [Test]
    public void Constructor_AcceptsMatchingDefinitionSchemaAndValidDefault()
    {
        var experiment = CreateExperiment();
        var schema = CreateSchema();
        var configuration = CreateConfiguration();

        var template = new ExperimentAuthoringTemplate(
            "intro-electrical-thermal", "1.0.0", "Introductory electrical heating",
            experiment, schema, configuration);

        Assert.That(template.Experiment, Is.SameAs(experiment));
        Assert.That(template.ParameterSchema, Is.SameAs(schema));
        Assert.That(template.DefaultConfiguration, Is.SameAs(configuration));
    }

    [Test]
    public void Constructor_RejectsSchemaForDifferentExperimentVersion()
    {
        Assert.Throws<ArgumentException>(() => new ExperimentAuthoringTemplate(
            "template", "1.0.0", "Template",
            CreateExperiment(version: "2.0.0"), CreateSchema(), CreateConfiguration()));
    }

    [Test]
    public void Constructor_RejectsInvalidDefaultConfiguration()
    {
        Assert.Throws<ArgumentException>(() => new ExperimentAuthoringTemplate(
            "template", "1.0.0", "Template",
            CreateExperiment(), CreateSchema(),
            CreateConfiguration(payload: "{\"voltageV\":-1,\"resistanceOhm\":6}")));
    }

    [Test]
    public void CreateConfiguration_ValidatesAndPreservesTemplateIdentityAndSchemaVersion()
    {
        var template = new ExperimentAuthoringTemplate(
            "template", "1.0.0", "Template",
            CreateExperiment(), CreateSchema(), CreateConfiguration());

        var configuration = template.CreateConfiguration("{\"voltageV\":24,\"resistanceOhm\":12}");

        Assert.That(configuration.ExperimentId, Is.EqualTo("thermal-coupling"));
        Assert.That(configuration.ExperimentVersion, Is.EqualTo("1.0.0"));
        Assert.That(configuration.SchemaVersion, Is.EqualTo(1));
        Assert.That(configuration.ContentType, Is.EqualTo("application/json"));
        Assert.That(configuration.Payload, Does.Contain("\"voltageV\":24"));
    }

    [Test]
    public void CreateConfiguration_RejectsUnknownOrOutOfRangeParameters()
    {
        var template = new ExperimentAuthoringTemplate(
            "template", "1.0.0", "Template",
            CreateExperiment(), CreateSchema(), CreateConfiguration());

        Assert.Throws<ArgumentException>(() => template.CreateConfiguration(
            "{\"voltageV\":12,\"resistanceOhm\":6,\"hidden\":true}"));
        Assert.Throws<ArgumentException>(() => template.CreateConfiguration(
            "{\"voltageV\":12,\"resistanceOhm\":0}"));
    }
}
