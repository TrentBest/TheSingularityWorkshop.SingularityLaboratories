using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentConfigurationCompatibilityTests
{
    private static ExperimentDefinition CreateExperiment(string version = "2.0.0") =>
        new("thermal-coupling", version, "Electrical to thermal coupling", "physics.thermal", "2.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);

    private static ExperimentParameterSchema CreateSchema(
        IEnumerable<ExperimentParameterDefinition> parameters, int version = 2) =>
        new("thermal-coupling", "2.0.0", version, parameters);

    private static ExperimentAuthoringTemplate CreateTarget(
        IEnumerable<ExperimentParameterDefinition> parameters,
        string defaultPayload = "{\"voltageV\":12,\"resistanceOhm\":6}",
        string contentType = "application/json")
    {
        var schema = CreateSchema(parameters);
        return new ExperimentAuthoringTemplate(
            "thermal-template", "3.0.0", "Thermal experiment",
            CreateExperiment(), schema,
            new ExperimentConfiguration("thermal-coupling", "2.0.0",
                schema.SchemaVersion, contentType, defaultPayload));
    }

    private static ExperimentParameterDefinition Voltage(double maximum = 100) =>
        new("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number,
            minimum: 0, maximum: maximum);

    private static ExperimentParameterDefinition Resistance() =>
        new("resistanceOhm", "Resistance", "Load resistance.", ExperimentParameterType.Number,
            minimum: 0.001);

    [Test]
    public void Assess_AcceptsSavedConfigurationFromEarlierExperimentAndSchemaVersions()
    {
        var source = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "application/json",
            "{\"voltageV\":12,\"resistanceOhm\":6}");
        var target = CreateTarget([
            Voltage(), Resistance(),
            new ExperimentParameterDefinition("label", "Label", "Optional note.",
                ExperimentParameterType.String, required: false)
        ]);

        var assessment = ExperimentConfigurationCompatibility.Assess(source, target);

        Assert.That(assessment.IsCompatible, Is.True);
        Assert.That(assessment.FailureReason, Is.Null);
        Assert.That(assessment.SourceExperimentVersion, Is.EqualTo("1.0.0"));
        Assert.That(assessment.SourceSchemaVersion, Is.EqualTo(1));
        Assert.That(assessment.TargetExperimentVersion, Is.EqualTo("2.0.0"));
        Assert.That(assessment.TargetSchemaVersion, Is.EqualTo(2));
        Assert.That(assessment.TargetTemplateVersion, Is.EqualTo("3.0.0"));
    }

    [Test]
    public void Assess_RejectsSavedConfigurationWhenTargetAddsRequiredParameter()
    {
        var source = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "application/json",
            "{\"voltageV\":12,\"resistanceOhm\":6}");
        var target = CreateTarget([
            Voltage(), Resistance(),
            new ExperimentParameterDefinition("ambientC", "Ambient temperature",
                "Ambient temperature.", ExperimentParameterType.Number)
        ], "{\"voltageV\":12,\"resistanceOhm\":6,\"ambientC\":20}");

        var assessment = ExperimentConfigurationCompatibility.Assess(source, target);

        Assert.That(assessment.IsCompatible, Is.False);
        Assert.That(assessment.FailureReason, Does.Contain("ambientC"));
    }

    [Test]
    public void Assess_RejectsSavedConfigurationOutsideTargetRange()
    {
        var source = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "application/json",
            "{\"voltageV\":120,\"resistanceOhm\":6}");
        var target = CreateTarget([Voltage(), Resistance()]);

        var assessment = ExperimentConfigurationCompatibility.Assess(source, target);

        Assert.That(assessment.IsCompatible, Is.False);
        Assert.That(assessment.FailureReason, Does.Contain("outside"));
    }

    [Test]
    public void Assess_RejectsDifferentExperimentIdentityAndContentType()
    {
        var target = CreateTarget([Voltage(), Resistance()]);
        var differentExperiment = new ExperimentConfiguration(
            "other-experiment", "1.0.0", 1, "application/json", "{\"voltageV\":12}");
        var wrongIdentity = ExperimentConfigurationCompatibility.Assess(differentExperiment, target);
        Assert.That(wrongIdentity.IsCompatible, Is.False);
        Assert.That(wrongIdentity.FailureReason, Does.Contain("different experiment"));

        var wrongContentType = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "text/plain", "voltage=12");
        var contentType = ExperimentConfigurationCompatibility.Assess(wrongContentType, target);
        Assert.That(contentType.IsCompatible, Is.False);
        Assert.That(contentType.FailureReason, Does.Contain("different content types"));
    }

    [Test]
    public void Assess_RejectsMalformedJsonAndNonJsonContent()
    {
        var target = CreateTarget([Voltage(), Resistance()]);
        var malformed = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "application/json", "{");
        var malformedAssessment = ExperimentConfigurationCompatibility.Assess(malformed, target);
        Assert.That(malformedAssessment.IsCompatible, Is.False);
        Assert.That(malformedAssessment.FailureReason, Does.Contain("not valid JSON"));

        var unsupported = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "application/xml", "<config />");
        var unsupportedAssessment = ExperimentConfigurationCompatibility.Assess(unsupported, target);
        Assert.That(unsupportedAssessment.IsCompatible, Is.False);
        Assert.That(unsupportedAssessment.FailureReason, Does.Contain("different content types"));
    }

    [Test]
    public void Assess_RejectsNullArguments()
    {
        var target = CreateTarget([Voltage(), Resistance()]);
        var source = new ExperimentConfiguration(
            "thermal-coupling", "1.0.0", 1, "application/json",
            "{\"voltageV\":12,\"resistanceOhm\":6}");

        Assert.Throws<ArgumentNullException>(() =>
            ExperimentConfigurationCompatibility.Assess(null!, target));
        Assert.Throws<ArgumentNullException>(() =>
            ExperimentConfigurationCompatibility.Assess(source, null!));
    }
}
