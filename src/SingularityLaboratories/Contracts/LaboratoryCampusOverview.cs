namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// A host-neutral semantic projection of a campus for navigation and presentation adapters.
/// It resolves the campus relationships into display-ready labels without defining geometry,
/// coordinates, visual styles, navigation policy, or authorization.
/// </summary>
public sealed class LaboratoryCampusOverview
{
    private LaboratoryCampusOverview(
        string id,
        string version,
        string name,
        string description,
        IReadOnlyList<LaboratoryCampusWorkspaceOverview> workspaces)
    {
        Id = id;
        Version = version;
        Name = name;
        Description = description;
        Workspaces = workspaces;
    }

    /// <summary>Gets the stable campus identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the campus definition version.</summary>
    public string Version { get; }

    /// <summary>Gets the campus display name.</summary>
    public string Name { get; }

    /// <summary>Gets the campus description.</summary>
    public string Description { get; }

    /// <summary>Gets the workspace summaries in the source campus order.</summary>
    public IReadOnlyList<LaboratoryCampusWorkspaceOverview> Workspaces { get; }

    /// <summary>Creates a semantic overview by resolving each workspace against its campus definitions.</summary>
    public static LaboratoryCampusOverview Create(LaboratoryCampus campus)
    {
        ArgumentNullException.ThrowIfNull(campus);

        var laboratories = campus.Laboratories.ToDictionary(x => x.Id, StringComparer.Ordinal);
        var facilities = campus.Facilities.ToDictionary(x => x.Id, StringComparer.Ordinal);
        var workspaces = campus.Workspaces.Select(workspace =>
        {
            var laboratory = laboratories[workspace.LaboratoryId];
            var facility = facilities[workspace.FacilityId];
            var room = facility.Rooms.Single(x => x.Id == workspace.RoomId);
            var apparatus = facility.Apparatus
                .Where(x => x.RoomId == room.Id)
                .Select(x => new LaboratoryCampusApparatusOverview(
                    x.Id, x.Name, x.ApparatusType, x.Description))
                .ToArray();
            var apparatusIds = apparatus.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
            var instruments = facility.Instruments
                .Where(instrument => apparatusIds.Contains(instrument.ApparatusId))
                .Select(instrument => new LaboratoryCampusInstrumentOverview(
                    instrument.Id,
                    instrument.Name,
                    instrument.InstrumentType,
                    instrument.InstrumentVersion,
                    instrument.CalibrationReference,
                    Array.AsReadOnly(facility.MeasurementChannels
                        .Where(channel => channel.InstrumentId == instrument.Id)
                        .Select(channel => new LaboratoryCampusMeasurementChannelOverview(
                            channel.Id,
                            channel.Name,
                            channel.Quantity,
                            channel.UnitSymbol,
                            channel.SamplingMode,
                            channel.SamplesPerSecond))
                        .ToArray())))
                .ToArray();

            return new LaboratoryCampusWorkspaceOverview(
                workspace.Id,
                laboratory.Id,
                laboratory.Name,
                laboratory.Description,
                facility.Id,
                facility.Name,
                room.Id,
                room.Name,
                room.Purpose,
                Array.AsReadOnly(apparatus),
                Array.AsReadOnly(instruments));
        }).ToArray();

        return new LaboratoryCampusOverview(
            campus.Id,
            campus.Version,
            campus.Name,
            campus.Description,
            Array.AsReadOnly(workspaces));
    }
}

/// <summary>Resolved semantic content for one workspace placement.</summary>
public sealed record LaboratoryCampusWorkspaceOverview(
    string Id,
    string LaboratoryId,
    string LaboratoryName,
    string LaboratoryDescription,
    string FacilityId,
    string FacilityName,
    string RoomId,
    string RoomName,
    string RoomPurpose,
    IReadOnlyList<LaboratoryCampusApparatusOverview> Apparatus,
    IReadOnlyList<LaboratoryCampusInstrumentOverview> Instruments);

/// <summary>Displayable apparatus metadata associated with a workspace room.</summary>
public sealed record LaboratoryCampusApparatusOverview(
    string Id,
    string Name,
    string ApparatusType,
    string Description);

/// <summary>Displayable instrument metadata and its declared observation channels.</summary>
public sealed record LaboratoryCampusInstrumentOverview(
    string Id,
    string Name,
    string InstrumentType,
    string InstrumentVersion,
    string CalibrationReference,
    IReadOnlyList<LaboratoryCampusMeasurementChannelOverview> MeasurementChannels);

/// <summary>Unit-explicit observation-channel metadata; contains no measured values.</summary>
public sealed record LaboratoryCampusMeasurementChannelOverview(
    string Id,
    string Name,
    string Quantity,
    string UnitSymbol,
    MeasurementSamplingMode SamplingMode,
    double? SamplesPerSecond);
