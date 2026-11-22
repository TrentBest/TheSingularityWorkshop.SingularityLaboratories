namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Host-owned admission rules applied before an experiment model is invoked.
/// These rules do not certify safety or change scientific validation level.
/// </summary>
public sealed record ExperimentExecutionPolicy
{
    public ExperimentExecutionPolicy(
        bool requireAcceptedValidationAssessment = false,
        ExperimentRiskLevel? maximumRiskLevel = null)
    {
        if (maximumRiskLevel is not null &&
            (!Enum.IsDefined(maximumRiskLevel.Value) || maximumRiskLevel == ExperimentRiskLevel.Unspecified))
            throw new ArgumentOutOfRangeException(nameof(maximumRiskLevel));

        RequireAcceptedValidationAssessment = requireAcceptedValidationAssessment;
        MaximumRiskLevel = maximumRiskLevel;
    }

    public bool RequireAcceptedValidationAssessment { get; }
    public ExperimentRiskLevel? MaximumRiskLevel { get; }

    /// <summary>Evaluates all configured admission rules and returns every denial reason.</summary>
    public ExperimentExecutionAdmissionResult Evaluate(ExperimentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var reasons = new List<ExperimentExecutionDenialReason>();

        if (RequireAcceptedValidationAssessment &&
            definition.ValidationAssessment?.State != ExperimentValidationState.Accepted)
            reasons.Add(ExperimentExecutionDenialReason.ValidationAssessmentNotAccepted);

        if (MaximumRiskLevel is not null)
        {
            var safety = definition.SafetyMetadata;
            if (safety is null)
                reasons.Add(ExperimentExecutionDenialReason.SafetyMetadataMissing);
            else if (safety.RiskLevel > MaximumRiskLevel.Value)
                reasons.Add(ExperimentExecutionDenialReason.RiskAboveMaximum);
        }

        return new ExperimentExecutionAdmissionResult(reasons);
    }

    /// <summary>Rejects a definition that does not satisfy this host's declared admission rules.</summary>
    public void EnsureCanExecute(ExperimentDefinition definition) => Evaluate(definition).EnsureAllowed();
}
