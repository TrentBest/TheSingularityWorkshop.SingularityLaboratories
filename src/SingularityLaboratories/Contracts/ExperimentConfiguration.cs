namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>An immutable, versioned configuration snapshot for one experiment definition.</summary>
public sealed record ExperimentConfiguration
{
    public ExperimentConfiguration(
        string experimentId,
        string experimentVersion,
        int schemaVersion,
        string contentType,
        string payload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(experimentId);
        ArgumentException.ThrowIfNullOrWhiteSpace(experimentVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentOutOfRangeException.ThrowIfLessThan(schemaVersion, 1);

        ExperimentId = experimentId.Trim();
        ExperimentVersion = experimentVersion.Trim();
        SchemaVersion = schemaVersion;
        ContentType = contentType.Trim();
        Payload = payload;
    }

    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public int SchemaVersion { get; }
    public string ContentType { get; }
    public string Payload { get; }
}
