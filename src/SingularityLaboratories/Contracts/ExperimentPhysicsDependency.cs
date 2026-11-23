namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>A declared Physics capability consumed by an experiment.</summary>
public sealed record ExperimentPhysicsDependency
{
    public ExperimentPhysicsDependency(
        string capabilityId,
        string domain,
        string modelId,
        string modelVersion,
        string purpose)
    {
        CapabilityId = Required(capabilityId, nameof(capabilityId));
        Domain = Required(domain, nameof(domain));
        ModelId = Required(modelId, nameof(modelId));
        ModelVersion = Required(modelVersion, nameof(modelVersion));
        Purpose = Required(purpose, nameof(purpose));
    }

    public string CapabilityId { get; }
    public string Domain { get; }
    public string ModelId { get; }
    public string ModelVersion { get; }
    public string Purpose { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>A required MicroBundle capability declared by an experiment.</summary>
public sealed record ExperimentMicroBundleRequirement
{
    public ExperimentMicroBundleRequirement(ulong bundleId, string version, string providerId, string purpose)
    {
        if (bundleId == 0) throw new ArgumentOutOfRangeException(nameof(bundleId));
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(providerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);
        BundleId = bundleId;
        Version = version.Trim();
        ProviderId = providerId.Trim();
        Purpose = purpose.Trim();
    }

    public ulong BundleId { get; }
    public string Version { get; }
    public string ProviderId { get; }
    public string Purpose { get; }
}
