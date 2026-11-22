using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentExecutionAdmissionResultTests
{
    [Test]
    public void Evaluate_ReturnsEveryApplicableDenialReason()
    {
        var definition = CreateDefinition();
        var policy = new ExperimentExecutionPolicy(
            requireAcceptedValidationAssessment: true,
            maximumRiskLevel: ExperimentRiskLevel.Moderate);

        var result = policy.Evaluate(definition);

        Assert.That(result.IsAllowed, Is.False);
        Assert.That(result.DenialReasons, Is.EquivalentTo(new[]
        {
            ExperimentExecutionDenialReason.ValidationAssessmentNotAccepted,
            ExperimentExecutionDenialReason.SafetyMetadataMissing
        }));
        Assert.Throws<InvalidOperationException>(() => result.EnsureAllowed());
    }

    [Test]
    public void Evaluate_ReportsRiskCeilingViolationWithoutThrowing()
    {
        var definition = CreateDefinition(ExperimentRiskLevel.Critical);
        var result = new ExperimentExecutionPolicy(maximumRiskLevel: ExperimentRiskLevel.High)
            .Evaluate(definition);

        Assert.That(result.IsAllowed, Is.False);
        Assert.That(result.DenialReasons, Is.EqualTo(new[]
        {
            ExperimentExecutionDenialReason.RiskAboveMaximum
        }));
    }

    [Test]
    public void Evaluate_ReturnsAllowedResultWhenNoRulesAreConfigured()
    {
        var result = new ExperimentExecutionPolicy().Evaluate(CreateDefinition());

        Assert.That(result.IsAllowed, Is.True);
        Assert.That(result.DenialReasons, Is.Empty);
        Assert.DoesNotThrow(() => result.EnsureAllowed());
    }

    [Test]
    public void Constructor_RejectsDuplicateDenialReasons()
    {
        Assert.Throws<ArgumentException>(() => new ExperimentExecutionAdmissionResult(new[]
        {
            ExperimentExecutionDenialReason.SafetyMetadataMissing,
            ExperimentExecutionDenialReason.SafetyMetadataMissing
        }));
    }

    private static ExperimentDefinition CreateDefinition(ExperimentRiskLevel? risk = null)
    {
        ExperimentSafetyMetadata? safety = risk is null ? null : new ExperimentSafetyMetadata(
            risk.Value,
            ["Declared test hazard."],
            ["Use the documented precautions."],
            ["Use outside the documented conditions."]);

        return new ExperimentDefinition(
            "admission-test",
            "1.0.0",
            "Admission test",
            "test-model",
            "1.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Test assumption."],
            ["Test limitation."],
            safety);
    }
}
