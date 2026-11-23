namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// A portable composition of laboratory workspaces and their physical facilities.
/// Hosts own geometry, navigation, staffing enforcement, and visual presentation.
/// </summary>
public sealed record LaboratoryCampus
{
    public LaboratoryCampus(
        string id,
        string version,
        string name,
        string description,
        IEnumerable<LaboratoryDefinition> laboratories,
        IEnumerable<LaboratoryFacility> facilities,
        IEnumerable<LaboratoryCampusWorkspace> workspaces)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Name = Required(name, nameof(name));
        Description = Required(description, nameof(description));
        Laboratories = Snapshot(laboratories, nameof(laboratories));
        Facilities = Snapshot(facilities, nameof(facilities));
        Workspaces = Snapshot(workspaces, nameof(workspaces));

        EnsureUnique(Laboratories.Select(x => x.Id), nameof(laboratories));
        EnsureUnique(Facilities.Select(x => x.Id), nameof(facilities));
        EnsureUnique(Workspaces.Select(x => x.Id), nameof(workspaces));

        var laboratoriesById = Laboratories.ToDictionary(x => x.Id, StringComparer.Ordinal);
        var facilitiesById = Facilities.ToDictionary(x => x.Id, StringComparer.Ordinal);

        foreach (var workspace in Workspaces)
        {
            if (!laboratoriesById.ContainsKey(workspace.LaboratoryId))
                throw new ArgumentException("Every campus workspace must reference a laboratory in this campus.", nameof(workspaces));
            if (!facilitiesById.TryGetValue(workspace.FacilityId, out var facility) ||
                !facility.Rooms.Any(room => string.Equals(room.Id, workspace.RoomId, StringComparison.Ordinal)))
                throw new ArgumentException("Every campus workspace must reference a room in a facility in this campus.", nameof(workspaces));
        }
    }

    public string Id { get; }
    public string Version { get; }
    public string Name { get; }
    public string Description { get; }
    public IReadOnlyList<LaboratoryDefinition> Laboratories { get; }
    public IReadOnlyList<LaboratoryFacility> Facilities { get; }
    public IReadOnlyList<LaboratoryCampusWorkspace> Workspaces { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }

    private static IReadOnlyList<T> Snapshot<T>(IEnumerable<T> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        var snapshot = values.ToArray();
        if (snapshot.Any(value => value is null))
            throw new ArgumentException("Campus collections cannot contain null entries.", parameterName);
        return Array.AsReadOnly(snapshot);
    }

    private static void EnsureUnique(IEnumerable<string> ids, string parameterName)
    {
        var values = ids.ToArray();
        if (values.Distinct(StringComparer.Ordinal).Count() != values.Length)
            throw new ArgumentException("Campus identifiers must be unique within their collection.", parameterName);
    }
}

/// <summary>Places a portable laboratory workspace in a room of a campus facility.</summary>
public sealed record LaboratoryCampusWorkspace
{
    public LaboratoryCampusWorkspace(string id, string laboratoryId, string facilityId, string roomId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(laboratoryId);
        ArgumentException.ThrowIfNullOrWhiteSpace(facilityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(roomId);
        Id = id.Trim();
        LaboratoryId = laboratoryId.Trim();
        FacilityId = facilityId.Trim();
        RoomId = roomId.Trim();
    }

    public string Id { get; }
    public string LaboratoryId { get; }
    public string FacilityId { get; }
    public string RoomId { get; }
}
