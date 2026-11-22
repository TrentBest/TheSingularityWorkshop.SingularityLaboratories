using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentAuthoringTemplateComparisonTests
{
    private static ExperimentDefinition CreateExperiment(string version = "1.0.0", string modelVersion = "2.0.0") =>
        new("thermal-coupling", version, "Electrical to thermal coupling", "physics.thermal", modelVersion,
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);

    private static ExperimentParameterDefinition VoltageParameter(double minimum = 0) =>
        new("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number, minimum: minimum);

    private static ExperimentParameterSchema CreateSchema(
        IEnumerable<ExperimentParameterDefinition>? parameters = null, int version = 1) =>
        new("thermal-coupling", "1.0.0", version,
            parameters ?? [VoltageParameter(), new ExperimentParameterDefinition(
                "resistanceOhm", "Resistance", "Load resistance.", ExperimentParameterType.Number, minimum: 0.001)]);

    private static ExperimentAuthoringTemplate CreateTemplate(
        string templateVersion = "1.0.0",
        string experimentVersion = "1.0.0",
        string modelVersion = "2.0.0",
        ExperimentParameterSchema? schema = null,
        string payload = "{\"voltageV\":12,\"resistanceOhm\":6}") =>
        new("thermal-intro", templateVersion, "Introductory thermal experiment",
            CreateExperiment(experimentVersion, modelVersion),
            schema ?? CreateSchema(),
            new ExperimentConfiguration("thermal-coupling", experimentVersion,
                schema?.SchemaVersion ?? 1, "application/json", payload));

    [Test]
    public void Compare_ReturnsIdenticalWhenTemplatesMatch()
    {
        var template = CreateTemplate();

        var comparison = ExperimentAuthoringTemplateComparison.Compare(template, CreateTemplate());

        Assert.That(comparison.IsIdentical, Is.True);
        Assert.That(comparison.Differences, Is.Empty);
    }

    [Test]
    public void Compare_ReportsTemplateExperimentAndDefaultChanges()
    {
        var before = CreateTemplate();
        var after = CreateTemplate(templateVersion: "2.0.0", experimentVersion: "1.0.0",
            modelVersion: "2.1.0", payload: "{\"voltageV\":24,\"resistanceOhm\":6}");

        var comparison = ExperimentAuthoringTemplateComparison.Compare(before, after);

        Assert.That(comparison.IsIdentical, Is.False);
        Assert.That(comparison.Differences.Select(item => item.Path),
            Does.Contain("template.version")
                .And.Contain("experiment.modelVersion")
                .And.Contain("defaultConfiguration.payload"));
    }

    [Test]
    public void Compare_ReportsAddedRemovedAndChangedParametersInStableOrder()
    {
        var beforeSchema = CreateSchema();
        var afterSchema = CreateSchema(
        [
            VoltageParameter(minimum: 1),
            new ExperimentParameterDefinition("currentA", "Current", "Measured current.",
                ExperimentParameterType.Number, required: false, minimum: 0)
        ]);

        var before = CreateTemplate(schema: beforeSchema);
        var after = CreateTemplate(schema: afterSchema,
            payload: "{\"voltageV\":12,\"currentA\":2}");

        var comparison = ExperimentAuthoringTemplateComparison.Compare(before, after);
        var parameterChanges = comparison.Differences
            .Where(item => item.Path.StartsWith("schema.parameters.", StringComparison.Ordinal))
            .ToArray();

        Assert.That(parameterChanges.Select(item => item.Path),
            Is.EqualTo([
                "schema.parameters.currentA",
                "schema.parameters.resistanceOhm",
                "schema.parameters.voltageV"
            ]));
        Assert.That(parameterChanges.Select(item => item.Kind),
            Is.EqualTo([
                ExperimentAuthoringTemplateDifferenceKind.Added,
                ExperimentAuthoringTemplateDifferenceKind.Removed,
                ExperimentAuthoringTemplateDifferenceKind.Changed
            ]));
    }

    [Test]
    public void Compare_RejectsNullTemplates()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ExperimentAuthoringTemplateComparison.Compare(null!, CreateTemplate()));
        Assert.Throws<ArgumentNullException>(() =>
            ExperimentAuthoringTemplateComparison.Compare(CreateTemplate(), null!));
    }
}
