namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>Maps one stable narrative location identity to a portable laboratory room identity.</summary>
public sealed record NarrativeCampaignLocationBinding
{
    public NarrativeCampaignLocationBinding(string locationId, string displayName, string roomId, IReadOnlyList<string>? apparatusIds = null, IReadOnlyList<string>? instrumentIds = null, IReadOnlyList<string>? measurementChannelIds = null)
    {
        LocationId = Required(locationId, nameof(locationId));
        DisplayName = Required(displayName, nameof(displayName));
        RoomId = Required(roomId, nameof(roomId));
        var apparatus = (apparatusIds ?? Array.Empty<string>()).Select(id => Required(id, nameof(apparatusIds))).ToArray();
        if (apparatus.Distinct(StringComparer.Ordinal).Count() != apparatus.Length)
            throw new ArgumentException("Apparatus identifiers must be unique within a location.", nameof(apparatusIds));
        ApparatusIds = Array.AsReadOnly(apparatus);
        InstrumentIds = SnapshotIds(instrumentIds, nameof(instrumentIds));
        MeasurementChannelIds = SnapshotIds(measurementChannelIds, nameof(measurementChannelIds));
    }

    public string LocationId { get; }
    public string DisplayName { get; }
    public string RoomId { get; }
    public IReadOnlyList<string> ApparatusIds { get; }
    public IReadOnlyList<string> InstrumentIds { get; }
    public IReadOnlyList<string> MeasurementChannelIds { get; }

    private static IReadOnlyList<string> SnapshotIds(IReadOnlyList<string>? values, string parameterName)
    {
        var snapshot = (values ?? Array.Empty<string>()).Select(value => Required(value, parameterName)).ToArray();
        if (snapshot.Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Identifiers must be unique within a location.", parameterName);
        return Array.AsReadOnly(snapshot);
    }

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
        IReadOnlyList<NarrativeCampaignLocationBinding> locations,
        string? laboratoryId = null,
        IReadOnlyList<NarrativeCampaignStaffBinding>? staffBindings = null)
    {
        CampaignId = Required(campaignId, nameof(campaignId));
        CampaignVersion = Required(campaignVersion, nameof(campaignVersion));
        FacilityId = Required(facilityId, nameof(facilityId));
        FacilityVersion = Required(facilityVersion, nameof(facilityVersion));
        StartingLocationId = Required(startingLocationId, nameof(startingLocationId));
        LaboratoryId = string.IsNullOrWhiteSpace(laboratoryId) ? null : Required(laboratoryId, nameof(laboratoryId));

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
        var instrumentIds = snapshot.SelectMany(item => item.InstrumentIds).ToArray();
        if (instrumentIds.Distinct(StringComparer.Ordinal).Count() != instrumentIds.Length)
            throw new ArgumentException("Each instrument may be mapped to only one narrative location.", nameof(locations));
        var channelIds = snapshot.SelectMany(item => item.MeasurementChannelIds).ToArray();
        if (channelIds.Distinct(StringComparer.Ordinal).Count() != channelIds.Length)
            throw new ArgumentException("Each measurement channel may be mapped to only one narrative location.", nameof(locations));
        if (!snapshot.Any(item => string.Equals(item.LocationId, StartingLocationId, StringComparison.Ordinal)))
            throw new ArgumentException("The starting location must be present in the facility mapping.", nameof(startingLocationId));

        Locations = Array.AsReadOnly(snapshot);

        var staffSnapshot = (staffBindings ?? Array.Empty<NarrativeCampaignStaffBinding>()).ToArray();
        if (staffSnapshot.Any(item => item is null))
            throw new ArgumentException("Staff bindings cannot contain null entries.", nameof(staffBindings));
        if (staffSnapshot.Length > 0 && LaboratoryId is null)
            throw new ArgumentException("Staff bindings require a laboratory identity.", nameof(laboratoryId));
        if (staffSnapshot.Select(item => item.CampaignRoleDisplayName).Distinct(StringComparer.Ordinal).Count() != staffSnapshot.Length ||
            staffSnapshot.Select(item => item.AssignmentId).Distinct(StringComparer.Ordinal).Count() != staffSnapshot.Length)
            throw new ArgumentException("Campaign role and assignment IDs must be unique within staff bindings.", nameof(staffBindings));
        StaffBindings = Array.AsReadOnly(staffSnapshot);
    }

    public string CampaignId { get; }
    public string CampaignVersion { get; }
    public string FacilityId { get; }
    public string FacilityVersion { get; }
    public string StartingLocationId { get; }
    public string? LaboratoryId { get; }
    public IReadOnlyList<NarrativeCampaignLocationBinding> Locations { get; }
    public IReadOnlyList<NarrativeCampaignStaffBinding> StaffBindings { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>Maps a fixed campaign role to a host-supplied laboratory responsibility assignment.</summary>
public sealed record NarrativeCampaignStaffBinding
{
    public NarrativeCampaignStaffBinding(string campaignRoleDisplayName, string assignmentId, LaboratoryStaffRole requiredRole)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(campaignRoleDisplayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(assignmentId);
        if (!Enum.IsDefined(requiredRole))
            throw new ArgumentOutOfRangeException(nameof(requiredRole));
        CampaignRoleDisplayName = campaignRoleDisplayName.Trim();
        AssignmentId = assignmentId.Trim();
        RequiredRole = requiredRole;
    }

    public string CampaignRoleDisplayName { get; }
    public string AssignmentId { get; }
    public LaboratoryStaffRole RequiredRole { get; }
}
