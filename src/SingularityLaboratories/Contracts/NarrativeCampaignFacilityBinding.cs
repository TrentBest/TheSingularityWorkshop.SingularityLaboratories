namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Maps one stable narrative location identity to a portable laboratory room identity.</summary>
public sealed record NarrativeCampaignLocationBinding
{
    public NarrativeCampaignLocationBinding(string locationId, string displayName, string roomId, IReadOnlyList<string>? apparatusIds = null)
    {
        LocationId = Required(locationId, nameof(locationId));
        DisplayName = Required(displayName, nameof(displayName));
        RoomId = Required(roomId, nameof(roomId));
        var apparatus = (apparatusIds ?? Array.Empty<string>()).Select(id => Required(id, nameof(apparatusIds))).ToArray();
        if (apparatus.Distinct(StringComparer.Ordinal).Count() != apparatus.Length)
            throw new ArgumentException("Apparatus identifiers must be unique within a location.", nameof(apparatusIds));
        ApparatusIds = Array.AsReadOnly(apparatus);
    }

    public string LocationId { get; }
    public string DisplayName { get; }
    public string RoomId { get; }
    public IReadOnlyList<string> ApparatusIds { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>
/// Version-bound mapping between a narrative campaign's stable location IDs and a portable
/// laboratory facility's room IDs. It contains no host geometry or navigation implementation.
/// </summary>
public sealed record NarrativeCampaignFacilityBinding
{
    public NarrativeCampaignFacilityBinding(
        string campaignId,
        string campaignVersion,
        string facilityId,
        string facilityVersion,
        string startingLocationId,
        IReadOnlyList<NarrativeCampaignLocationBinding> locations)
    {
        CampaignId = Required(campaignId, nameof(campaignId));
        CampaignVersion = Required(campaignVersion, nameof(campaignVersion));
        FacilityId = Required(facilityId, nameof(facilityId));
        FacilityVersion = Required(facilityVersion, nameof(facilityVersion));
        StartingLocationId = Required(startingLocationId, nameof(startingLocationId));

        ArgumentNullException.ThrowIfNull(locations);
        var snapshot = locations.ToArray();
        if (snapshot.Length == 0 || snapshot.Any(item => item is null))
            throw new ArgumentException("A facility binding requires non-null location mappings.", nameof(locations));
        if (snapshot.Select(item => item.LocationId).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Narrative location IDs must be unique.", nameof(locations));
        if (snapshot.Select(item => item.RoomId).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Each narrative location must map to a distinct facility room.", nameof(locations));
        var apparatusIds = snapshot.SelectMany(item => item.ApparatusIds).ToArray();
        if (apparatusIds.Distinct(StringComparer.Ordinal).Count() != apparatusIds.Length)
            throw new ArgumentException("Each apparatus item may be mapped to only one narrative location.", nameof(locations));
        if (!snapshot.Any(item => string.Equals(item.LocationId, StartingLocationId, StringComparison.Ordinal)))
            throw new ArgumentException("The starting location must be present in the facility mapping.", nameof(startingLocationId));

        Locations = Array.AsReadOnly(snapshot);
    }

    public string CampaignId { get; }
    public string CampaignVersion { get; }
    public string FacilityId { get; }
    public string FacilityVersion { get; }
    public string StartingLocationId { get; }
    public IReadOnlyList<NarrativeCampaignLocationBinding> Locations { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
