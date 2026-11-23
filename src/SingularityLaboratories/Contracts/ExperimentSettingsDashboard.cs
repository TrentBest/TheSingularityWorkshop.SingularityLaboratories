namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Observed lifecycle state of a MicroBundle capability for one experiment run.</summary>
public enum MicroBundleExecutionStatus
{
    NotObserved = 0,
    Resolved = 1,
    Loaded = 2,
    Executing = 3,
    Completed = 4,
    Failed = 5,
    Skipped = 6
}

/// <summary>Host-reported evidence about a specific MicroBundle capability during a run.</summary>
public sealed record MicroBundleExecutionObservation
{
    public MicroBundleExecutionObservation(
        ulong bundleId,
        string version,
        string providerId,
        MicroBundleExecutionStatus status,
        DateTimeOffset observedAtUtc,
        string? detail = null)
    {
        if (bundleId == 0) throw new ArgumentOutOfRangeException(nameof(bundleId));
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        if (observedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Observation timestamps must be UTC.", nameof(observedAtUtc));
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        BundleId = bundleId;
        Version = version.Trim();
        ProviderId = providerId.Trim();
        Status = status;
        ObservedAtUtc = observedAtUtc;
        Detail = string.IsNullOrWhiteSpace(detail) ? null : detail.Trim();
    }

    public ulong BundleId { get; }
    public string Version { get; }
    public string ProviderId { get; }
    public MicroBundleExecutionStatus Status { get; }
    public DateTimeOffset ObservedAtUtc { get; }
    public string? Detail { get; }
}

/// <summary>
/// Read-only settings/runtime projection for one experiment. A null RunId represents an authoring
/// preview; runtime observations require a concrete run identity. Declarations and host observations
/// remain separate so a dependency is never presented as executing merely because it was declared.
/// </summary>
public sealed record ExperimentSettingsDashboard
{
    public ExperimentSettingsDashboard(
        ExperimentDefinition definition,
        IEnumerable<MicroBundleExecutionObservation>? observedMicroBundles = null,
        Guid? runId = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        observedMicroBundles ??= [];
        if (runId == Guid.Empty) throw new ArgumentException("A run identity cannot be empty.", nameof(runId));
        ExperimentId = definition.Id;
        ExperimentVersion = definition.Version;
        ExperimentName = definition.Name;
        RunId = runId;
        PhysicsDependencies = definition.PhysicsDependencies;
        RequiredMicroBundles = definition.MicroBundleRequirements;
        var observations = observedMicroBundles.ToArray();
        if (observations.Any(x => x is null))
            throw new ArgumentException("Runtime observations cannot contain null values.", nameof(observedMicroBundles));
        if (observations.Length > 0 && runId is null)
            throw new ArgumentException("Runtime observations must be associated with a concrete experiment run.", nameof(runId));
        if (observations.Select(x => (x.BundleId, x.Version, x.ProviderId)).Distinct().Count() != observations.Length)
            throw new ArgumentException("Runtime observations must be unique per bundle version and provider.", nameof(observedMicroBundles));
        ObservedMicroBundles = Array.AsReadOnly(observations);
    }

    /// <summary>Null for an authoring preview; otherwise the exact run these observations describe.</summary>
    public Guid? RunId { get; }
    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public string ExperimentName { get; }
    public IReadOnlyList<ExperimentPhysicsDependency> PhysicsDependencies { get; }
    public IReadOnlyList<ExperimentMicroBundleRequirement> RequiredMicroBundles { get; }
    /// <summary>Only observations supplied by the trusted composition/execution host appear here.</summary>
    public IReadOnlyList<MicroBundleExecutionObservation> ObservedMicroBundles { get; }
}
