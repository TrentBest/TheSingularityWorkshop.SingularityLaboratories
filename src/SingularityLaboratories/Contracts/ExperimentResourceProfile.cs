namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Declares conservative resource estimates for one exact model version. Estimates are
/// advisory until a host supplies a budget; they are not measurements of actual consumption.
/// </summary>
public sealed record ExperimentResourceProfile
{
    public ExperimentResourceProfile(
        string modelId,
        string modelVersion,
        TimeSpan estimatedExecutionTime,
        long estimatedPeakMemoryBytes,
        long estimatedComputeUnits)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelVersion);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(estimatedExecutionTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimatedPeakMemoryBytes);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(estimatedComputeUnits);

        ModelId = modelId.Trim();
        ModelVersion = modelVersion.Trim();
        EstimatedExecutionTime = estimatedExecutionTime;
        EstimatedPeakMemoryBytes = estimatedPeakMemoryBytes;
        EstimatedComputeUnits = estimatedComputeUnits;
    }

    public string ModelId { get; }
    public string ModelVersion { get; }
    public TimeSpan EstimatedExecutionTime { get; }
    public long EstimatedPeakMemoryBytes { get; }
    public long EstimatedComputeUnits { get; }

    public void EnsureWithin(ExecutionResourceBudget budget)
    {
        ArgumentNullException.ThrowIfNull(budget);
        if (EstimatedExecutionTime > budget.MaximumExecutionTime ||
            EstimatedPeakMemoryBytes > budget.MaximumMemoryBytes ||
            EstimatedComputeUnits > budget.MaximumComputeUnits)
        {
            throw new InvalidOperationException("The experiment resource estimate exceeds the supplied execution budget.");
        }
    }
}

/// <summary>A host-owned ceiling for admitting an experiment execution.</summary>
public sealed record ExecutionResourceBudget
{
    public ExecutionResourceBudget(
        TimeSpan maximumExecutionTime,
        long maximumMemoryBytes,
        long maximumComputeUnits)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maximumExecutionTime, TimeSpan.Zero);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumMemoryBytes);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumComputeUnits);

        MaximumExecutionTime = maximumExecutionTime;
        MaximumMemoryBytes = maximumMemoryBytes;
        MaximumComputeUnits = maximumComputeUnits;
    }

    public TimeSpan MaximumExecutionTime { get; }
    public long MaximumMemoryBytes { get; }
    public long MaximumComputeUnits { get; }
}
