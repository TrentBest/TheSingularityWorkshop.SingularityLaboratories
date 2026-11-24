using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>
/// Projects host-authored room status observations onto validated narrative location identities.
/// The adapter does not infer freshness, interpret status codes, enforce access, or certify safety.
/// </summary>
public sealed class NarrativeCampaignFacilityStatusAdapter
{
    /// <summary>
    /// Creates a complete location status snapshot. The host must supply one observation for every
    /// mapped room and explicitly supply each observation's current/stale flag.
    /// </summary>
    public NarrativeCampaignFacilityStatusSnapshot Project(
        NarrativeCampaignFacilityProjection facilityProjection,
        IEnumerable<NarrativeFacilityStatusObservation> observations,
        DateTimeOffset capturedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(facilityProjection);
        ArgumentNullException.ThrowIfNull(observations);
        if (capturedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Facility status snapshot timestamps must be UTC.", nameof(capturedAtUtc));

        var observationSnapshot = observations.ToArray();
        if (observationSnapshot.Any(item => item is null))
            throw new ArgumentException("Facility status observations cannot contain null entries.", nameof(observations));
        if (observationSnapshot.Any(item => item.ObservedAtUtc > capturedAtUtc))
            throw new ArgumentException("A facility status observation cannot occur after its containing snapshot.", nameof(observations));
        if (observationSnapshot.Select(item => item.RoomId).Distinct(StringComparer.Ordinal).Count() != observationSnapshot.Length)
            throw new ArgumentException("A room may have only one status observation in a snapshot.", nameof(observations));

        var mappedRooms = facilityProjection.Locations.ToDictionary(
            location => location.RoomId,
            location => location.LocationId,
            StringComparer.Ordinal);
        var unknownRooms = observationSnapshot
            .Where(item => !mappedRooms.ContainsKey(item.RoomId))
            .Select(item => item.RoomId)
            .ToArray();
        if (unknownRooms.Length > 0)
            throw new ArgumentException(
                $"The host supplied status for rooms outside the validated campaign facility map: {string.Join(", ", unknownRooms)}.",
                nameof(observations));

        var observationsByRoom = observationSnapshot.ToDictionary(item => item.RoomId, StringComparer.Ordinal);
        var missingRooms = mappedRooms.Keys.Where(roomId => !observationsByRoom.ContainsKey(roomId)).ToArray();
        if (missingRooms.Length > 0)
            throw new ArgumentException(
                $"The host status snapshot is incomplete; missing room observations: {string.Join(", ", missingRooms)}.",
                nameof(observations));

        var locations = facilityProjection.Locations
            .Select(location =>
            {
                var observation = observationsByRoom[location.RoomId];
                return new NarrativeLocationStatusSnapshot(
                    location.LocationId,
                    location.RoomId,
                    observation.StatusCode,
                    observation.SourceId,
                    observation.ObservedAtUtc,
                    observation.IsCurrent);
            })
            .ToArray();

        return new NarrativeCampaignFacilityStatusSnapshot(
            facilityProjection.CampaignId,
            facilityProjection.CampaignVersion,
            facilityProjection.FacilityId,
            facilityProjection.FacilityVersion,
            capturedAtUtc,
            Array.AsReadOnly(locations));
    }
}
