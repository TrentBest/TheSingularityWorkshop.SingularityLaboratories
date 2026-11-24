namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Host-authored facility status observation. Status meaning and freshness are supplied by the host;
/// this contract does not infer operational safety or access permissions.
/// </summary>
public sealed record NarrativeFacilityStatusObservation
{
    public NarrativeFacilityStatusObservation(
        string roomId,
        string statusCode,
        string sourceId,
        DateTimeOffset observedAtUtc,
        bool isCurrent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roomId);
        ArgumentException.ThrowIfNullOrWhiteSpace(statusCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceId);
        if (observedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Facility status observation timestamps must be UTC.", nameof(observedAtUtc));

        RoomId = roomId.Trim();
        StatusCode = statusCode.Trim();
        SourceId = sourceId.Trim();
        ObservedAtUtc = observedAtUtc;
        IsCurrent = isCurrent;
    }

    public string RoomId { get; }
    public string StatusCode { get; }
    public string SourceId { get; }
    public DateTimeOffset ObservedAtUtc { get; }
    public bool IsCurrent { get; }
}

/// <summary>Immutable status projection for one stable narrative location.</summary>
public sealed record NarrativeLocationStatusSnapshot(
    string LocationId,
    string RoomId,
    string StatusCode,
    string SourceId,
    DateTimeOffset ObservedAtUtc,
    bool IsCurrent);

/// <summary>
/// Complete host-supplied status projection for the locations in one validated campaign/facility binding.
/// Status labels are opaque host values and are not safety certifications.
/// </summary>
public sealed record NarrativeCampaignFacilityStatusSnapshot(
    string CampaignId,
    string CampaignVersion,
    string FacilityId,
    string FacilityVersion,
    DateTimeOffset CapturedAtUtc,
    IReadOnlyList<NarrativeLocationStatusSnapshot> Locations);
