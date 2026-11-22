using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentDefinitionValidationTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 11, 22, 18, 0, 0, TimeSpan.Zero);

    private static ExperimentDefinition Definition(
        ExperimentSafetyMetadata? safety = null,
        ExperimentValidationAssessment? assessment = null) =>
        new("thermal-coupling", "1.0.0", "Thermal Coupling", "physics.thermal", "2.0.0",
            ScientificValidationLevel.EducationalModel, ["Idealized boundary conditions."],
            ["Does not model turbulent flow."], safety, assessment);

    private static ExperimentSafetyMetadata Safety() =>
        new(ExperimentRiskLevel.Low, ["No known hazards under stated conditions."],
            ["Follow the published procedure."], ["Use outside the stated temperature range."]);

    private static ExperimentValidationAssessment Assessment(
        string experimentId = "thermal-coupling",
        string experimentVersion = "1.0.0",
        string modelId = "physics.thermal",
        string modelVersion = "2.0.0") =>
        new(experimentId, experimentVersion, modelId, modelVersion,
            ExperimentValidationState.InReview, Timestamp, []);

    [Test]
    public void Definition_ExposesSafetyAndAssessmentForItsExactVersion()
    {
        var safety = Safety();
        var assessment = Assessment();

        var definition = Definition(safety, assessment);

        Assert.That(definition.SafetyMetadata, Is.SameAs(safety));
        Assert.That(definition.ValidationAssessment, Is.SameAs(assessment));
        Assert.That(definition.ValidationLevel, Is.EqualTo(ScientificValidationLevel.EducationalModel));
    }

    [TestCase("other-experiment", "1.0.0", "physics.thermal", "2.0.0")]
    [TestCase("thermal-coupling", "2.0.0", "physics.thermal", "2.0.0")]
    [TestCase("thermal-coupling", "1.0.0", "other-model", "2.0.0")]
    [TestCase("thermal-coupling", "1.0.0", "physics.thermal", "3.0.0")]
    public void Definition_RejectsAssessmentForDifferentExperimentOrModelVersion(
        string experimentId, string experimentVersion, string modelId, string modelVersion)
    {
        Assert.Throws<ArgumentException>(() =>
            Definition(assessment: Assessment(experimentId, experimentVersion, modelId, modelVersion)));
    }

    [Test]
    public void Definition_AllowsExistingCallersWithoutReviewMetadata()
    {
        var definition = Definition();

        Assert.That(definition.SafetyMetadata, Is.Null);
        Assert.That(definition.ValidationAssessment, Is.Null);
    }
}
