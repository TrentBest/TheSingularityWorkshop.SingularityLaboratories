using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentDefaultConfigurationCompatibilityTests
{
    private static ExperimentDefinition CreateExperiment(string version = "1.0.0") =>
        new("thermal-coupling", version, "Electrical to thermal coupling", "physics.thermal", "2.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);

    private static ExperimentParameterSchema CreateSchema(
        IEnumerable<ExperimentParameterDefinition> parameters, int version = 1) =>
        new("thermal-coupling", "1.0.0", version, parameters);

    private static ExperimentAuthoringTemplate CreateTemplate(
        string templateVersion,
        string experimentVersion,
        ExperimentParameterSchema schema,
        string payload,
        string contentType = "application/json") =>
        new("thermal-intro", templateVersion, "Introductory thermal experiment",
            CreateExperiment(experimentVersion), schema,
            new ExperimentConfiguration("thermal-coupling", experimentVersion,
                schema.SchemaVersion, contentType, payload));

    private static ExperimentParameterDefinition Voltage(bool required = true, double maximum = 100) =>
        new("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number,
            required: required, minimum: 0, maximum: maximum);

    private static ExperimentParameterDefinition Resistance() =>
        new("resistanceOhm", "Resistance", "Load resistance.", ExperimentParameterType.Number,
            minimum: 0.001);

    [Test]
    public void Assess_AcceptsSourceDefaultWhenTargetSchemaStillAcceptsIt()
    {
        var source = CreateTemplate("1.0.0", "1.0.0",
            CreateSchema([Voltage(), Resistance()]), "{\"voltageV\":12,\"resistanceOhm\":6}");
        var target = CreateTemplate("2.0.0", "1.0.0",
            CreateSchema([Voltage(), Resistance(),
                new ExperimentParameterDefinition("label", "Label", "Optional note.",
                    ExperimentParameterType.String, required: false)]),
            "{\"voltageV\":12,\"resistanceOhm\":6}");

        var assessment = ExperimentDefaultConfigurationCompatibility.Assess(source, target);

        Assert.That(assessment.IsCompatible, Is.True);
        Assert.That(assessment.FailureReason, Is.Null);
        Assert.That(assessment.SourceTemplateVersion, Is.EqualTo("1.0.0"));
        Assert.That(assessment.TargetTemplateVersion, Is.EqualTo("2.0.0"));
    }

    [Test]
    public void Assess_RejectsSourceDefaultWhenTargetAddsRequiredParameter()
    {
        var source = CreateTemplate("1.0.0", "1.0.0",
            CreateSchema([Voltage(), Resistance()]), "{\"voltageV\":12,\"resistanceOhm\":6}");
        var target = CreateTemplate("2.0.0", "1.0.0",
            CreateSchema([Voltage(), Resistance(),
                new ExperimentParameterDefinition("ambientC", "Ambient temperature",
                    "Ambient temperature.", ExperimentParameterType.Number)]),
            "{\"voltageV\":12,\"resistanceOhm\":6,\"ambientC\":20}");

        var assessment = ExperimentDefaultConfigurationCompatibility.Assess(source, target);

        Assert.That(assessment.IsCompatible, Is.False);
        Assert.That(assessment.FailureReason, Does.Contain("ambientC"));
    }

    [Test]
    public void Assess_RejectsSourceDefaultWhenTargetNarrowsParameterRange()
    {
        var source = CreateTemplate("1.0.0", "1.0.0",
            CreateSchema([Voltage(maximum: 200), Resistance()]), "{\"voltageV\":120,\"resistanceOhm\":6}");
        var target = CreateTemplate("2.0.0", "1.0.0",
            CreateSchema([
                new ExperimentParameterDefinition("voltageV", "Voltage", "Applied voltage.",
                    ExperimentParameterType.Number, minimum: 0, maximum: 100),
                Resistance()
            ]), "{\"voltageV\":12,\"resistanceOhm\":6}");

        var assessment = ExperimentDefaultConfigurationCompatibility.Assess(source, target);

        Assert.That(assessment.IsCompatible, Is.False);
        Assert.That(assessment.FailureReason, Does.Contain("outside"));
    }

    [Test]
    public void Assess_RejectsDifferentContentTypeAndExperimentIdentity()
    {
        var schema = CreateSchema([Voltage(), Resistance()]);
        var source = CreateTemplate("1.0.0", "1.0.0", schema,
            "{\"voltageV\":12,\"resistanceOhm\":6}", "text/plain");
        var target = CreateTemplate("2.0.0", "1.0.0", schema,
            "{\"voltageV\":12,\"resistanceOhm\":6}", "text/plain");

        var contentTypeAssessment = ExperimentDefaultConfigurationCompatibility.Assess(source, target);
        Assert.That(contentTypeAssessment.IsCompatible, Is.False);
        Assert.That(contentTypeAssessment.FailureReason, Does.Contain("application/json"));

        var otherExperiment = new ExperimentAuthoringTemplate("other-template", "1.0.0", "Other",
            new ExperimentDefinition("other-experiment", "1.0.0", "Other", "physics.other", "1.0.0",
                ScientificValidationLevel.EducationalModel, ["Assumption."], ["Limitation."]),
            new ExperimentParameterSchema("other-experiment", "1.0.0", 1,
                [new ExperimentParameterDefinition("value", "Value", "Value.", ExperimentParameterType.Number)]),
            new ExperimentConfiguration("other-experiment", "1.0.0", 1, "application/json", "{\"value\":1}"));

        var identityAssessment = ExperimentDefaultConfigurationCompatibility.Assess(source, otherExperiment);
        Assert.That(identityAssessment.IsCompatible, Is.False);
        Assert.That(identityAssessment.FailureReason, Does.Contain("different experiment"));
    }

    [Test]
    public void Assess_RejectsNullTemplates()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ExperimentDefaultConfigurationCompatibility.Assess(null!, CreateTemplate("2.0.0", "1.0.0",
                CreateSchema([Voltage(), Resistance()]), "{\"voltageV\":12,\"resistanceOhm\":6}")));
    }
}
