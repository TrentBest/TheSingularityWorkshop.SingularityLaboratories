using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentValidationAssessmentTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 11, 22, 18, 0, 0, TimeSpan.Zero);

    private static ExperimentValidationEvidence Evidence(string id = "test-run") =>
        new(id, "Thermal output matched the documented analytical result.", Timestamp,
            "https://example.invalid/evidence/test-run");

    [Test]
    public void AcceptedAssessment_RequiresReviewerAndEvidence()
    {
        Assert.Throws<ArgumentException>(() => new ExperimentValidationAssessment(
            "thermal-coupling", "1.0.0", "physics.thermal", "2.0.0",
            ExperimentValidationState.Accepted, Timestamp, [Evidence()]));

        Assert.Throws<ArgumentException>(() => new ExperimentValidationAssessment(
            "thermal-coupling", "1.0.0", "physics.thermal", "2.0.0",
            ExperimentValidationState.Accepted, Timestamp, [], "reviewer"));
    }

    [Test]
    public void Assessment_PreservesExactExperimentAndModelVersionsAndEvidence()
    {
        var evidence = Evidence();
        var assessment = new ExperimentValidationAssessment(
            "thermal-coupling", "1.0.0", "physics.thermal", "2.0.0",
            ExperimentValidationState.Accepted, Timestamp, [evidence], "reviewer",
            "Reviewed against the documented analytical fixture.");

        Assert.That(assessment.ExperimentId, Is.EqualTo("thermal-coupling"));
        Assert.That(assessment.ExperimentVersion, Is.EqualTo("1.0.0"));
        Assert.That(assessment.ModelId, Is.EqualTo("physics.thermal"));
        Assert.That(assessment.ModelVersion, Is.EqualTo("2.0.0"));
        Assert.That(assessment.Evidence.Single(), Is.SameAs(evidence));
        Assert.That(assessment.Reviewer, Is.EqualTo("reviewer"));
    }

    [Test]
    public void Assessment_RejectsDuplicateEvidenceIdentifiers()
    {
        Assert.Throws<ArgumentException>(() => new ExperimentValidationAssessment(
            "thermal-coupling", "1.0.0", "physics.thermal", "2.0.0",
            ExperimentValidationState.InReview, Timestamp,
            [Evidence("duplicate"), Evidence("duplicate")]));
    }

    [Test]
    public void Evidence_RequiresUtcTimestamp()
    {
        Assert.Throws<ArgumentException>(() => new ExperimentValidationEvidence(
            "test", "Evidence description.", new DateTimeOffset(2026, 11, 22, 10, 0, 0, TimeSpan.FromHours(-8))));
    }

    [Test]
    public void SafetyMetadata_RequiresExplicitRiskAndBoundaries()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ExperimentSafetyMetadata(
            ExperimentRiskLevel.Unspecified, ["No known hazards."], ["Follow instructions."], ["Unsupervised use."]));

        Assert.Throws<ArgumentException>(() => new ExperimentSafetyMetadata(
            ExperimentRiskLevel.Low, [], ["Follow instructions."], ["Unsupervised use."]));
    }

    [Test]
    public void SafetyMetadata_SnapshotsHazardsPrecautionsAndProhibitedUses()
    {
        var hazards = new List<string> { "Hot components." };
        var safety = new ExperimentSafetyMetadata(
            ExperimentRiskLevel.Moderate, hazards, ["Allow cooling before handling."], ["Use with live mains voltage."]);
        hazards.Add("Changed after construction.");

        Assert.That(safety.Hazards, Is.EqualTo(new[] { "Hot components." }));
        Assert.That(safety.Precautions, Has.Count.EqualTo(1));
        Assert.That(safety.ProhibitedUses, Has.Count.EqualTo(1));
    }
}
