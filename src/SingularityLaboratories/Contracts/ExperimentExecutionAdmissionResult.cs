namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Stable machine-readable reason why a host execution policy denied admission.</summary>
public enum ExperimentExecutionDenialReason
{
    ValidationAssessmentNotAccepted = 1,
    SafetyMetadataMissing = 2,
    RiskAboveMaximum = 3
}

/// <summary>Immutable result of evaluating host admission rules for one experiment definition.</summary>
/// <remarks>
/// Admission describes whether a host's configured rules allow invocation. It is not a safety
/// certification, scientific validation, or guarantee that execution will succeed.
</remarks>
public sealed record ExperimentExecutionAdmissionResult
{
    public ExperimentExecutionAdmissionResult(IEnumerable<ExperimentExecutionDenialReason> denialReasons)
    {
        ArgumentNullException.ThrowIfNull(denialReasons);
        var snapshot = denialReasons.ToArray();
        if (snapshot.Any(reason => !Enum.IsDefined(reason)))
            throw new ArgumentOutOfRangeException(nameof(denialReasons), "Admission denial reasons must be defined.");
        if (snapshot.Distinct().Count() != snapshot.Length)
            throw new ArgumentException("Admission denial reasons must be unique.", nameof(denialReasons));

        DenialReasons = Array.AsReadOnly(snapshot);
        IsAllowed = snapshot.Length == 0;
    }

    public bool IsAllowed { get; }
    public IReadOnlyList<ExperimentExecutionDenialReason> DenialReasons { get; }

    /// <summary>Throws when the host policy denied admission.</summary>
    public void EnsureAllowed()
    {
        if (!IsAllowed)
            throw new InvalidOperationException(
                $"The host execution policy denied this experiment: {string.Join(", ", DenialReasons)}.");
    }
}
