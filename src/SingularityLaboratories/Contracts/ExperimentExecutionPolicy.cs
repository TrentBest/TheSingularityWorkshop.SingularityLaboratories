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

    /// <summary>Rejects a definition that does not satisfy this host's declared admission rules.</summary>
    public void EnsureCanExecute(ExperimentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        if (RequireAcceptedValidationAssessment &&
            definition.ValidationAssessment?.State != ExperimentValidationState.Accepted)
            throw new InvalidOperationException(
                "The host requires an accepted validation assessment for this exact experiment and model version.");

        if (MaximumRiskLevel is not null)
        {
            var safety = definition.SafetyMetadata ??
                throw new InvalidOperationException("The host requires safety metadata to enforce its risk ceiling.");

            if (safety.RiskLevel > MaximumRiskLevel.Value)
                throw new InvalidOperationException(
                    $"The declared experiment risk level '{safety.RiskLevel}' exceeds the host maximum '{MaximumRiskLevel.Value}'.");
        }
    }
}
