using System.Collections.ObjectModel;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>
/// Validates the relationship between a narrative campaign's stable locations and a portable
/// laboratory facility. Host geometry, navigation, access control, and presentation remain outside.
/// </summary>
public sealed class NarrativeCampaignFacilityAdapter
{
    /// <summary>Validates exact campaign/facility versions and returns a host-neutral room projection.</summary>
    public NarrativeCampaignFacilityProjection Bind(
        NarrativeCampaignDefinition campaign,
        LaboratoryFacility facility,
        NarrativeCampaignFacilityBinding binding)
    {
        ArgumentNullException.ThrowIfNull(campaign);
        ArgumentNullException.ThrowIfNull(facility);
        ArgumentNullException.ThrowIfNull(binding);

        if (!string.Equals(binding.CampaignId, campaign.Id, StringComparison.Ordinal) ||
            !string.Equals(binding.CampaignVersion, campaign.Version, StringComparison.Ordinal))
            throw new ArgumentException("The facility binding must target the exact narrative campaign identity and version.", nameof(binding));

        if (!string.Equals(binding.FacilityId, facility.Id, StringComparison.Ordinal) ||
            !string.Equals(binding.FacilityVersion, facility.Version, StringComparison.Ordinal))
            throw new ArgumentException("The facility binding must target the exact laboratory facility identity and version.", nameof(binding));

        var roomIds = facility.Rooms.Select(room => room.Id).ToHashSet(StringComparer.Ordinal);
        var missingRooms = binding.Locations
            .Where(location => !roomIds.Contains(location.RoomId))
            .Select(location => location.RoomId)
            .ToArray();
        if (missingRooms.Length > 0)
            throw new ArgumentException(
                $"The facility binding references rooms that are not present in the selected facility: {string.Join(", ", missingRooms)}.",
                nameof(binding));

        var startingLocation = binding.Locations.Single(location =>
            string.Equals(location.LocationId, binding.StartingLocationId, StringComparison.Ordinal));
        if (!string.Equals(startingLocation.DisplayName, campaign.StartingLocation, StringComparison.Ordinal))
            throw new ArgumentException("The mapped starting location display name must match the campaign's fixed starting location.", nameof(binding));

        return new NarrativeCampaignFacilityProjection(
            campaign.Id,
            campaign.Version,
            facility.Id,
            facility.Version,
            binding.StartingLocationId,
            startingLocation.RoomId,
            binding.Locations);
    }
}

/// <summary>Validated host-neutral projection of narrative locations onto facility room identities.</summary>
public sealed class NarrativeCampaignFacilityProjection
{
    private readonly IReadOnlyDictionary<string, string> _roomIdsByLocation;

    internal NarrativeCampaignFacilityProjection(
        string campaignId,
        string campaignVersion,
        string facilityId,
        string facilityVersion,
        string startingLocationId,
        string startingRoomId,
        IReadOnlyList<NarrativeCampaignLocationBinding> locations)
    {
        CampaignId = campaignId;
        CampaignVersion = campaignVersion;
        FacilityId = facilityId;
        FacilityVersion = facilityVersion;
        StartingLocationId = startingLocationId;
        StartingRoomId = startingRoomId;
        Locations = Array.AsReadOnly(locations.ToArray());
        _roomIdsByLocation = new ReadOnlyDictionary<string, string>(
            Locations.ToDictionary(location => location.LocationId, location => location.RoomId, StringComparer.Ordinal));
    }

    public string CampaignId { get; }
    public string CampaignVersion { get; }
    public string FacilityId { get; }
    public string FacilityVersion { get; }
    public string StartingLocationId { get; }
    public string StartingRoomId { get; }
    public IReadOnlyList<NarrativeCampaignLocationBinding> Locations { get; }

    /// <summary>Resolves a stable narrative location ID to its validated laboratory room ID.</summary>
    public string ResolveRoomId(string locationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);
        return _roomIdsByLocation.TryGetValue(locationId, out var roomId)
            ? roomId
            : throw new KeyNotFoundException($"No facility room is bound to narrative location '{locationId}'.");
    }
}
