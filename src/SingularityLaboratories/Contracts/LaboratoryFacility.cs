namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// A portable facility manifest. It describes scientific work areas and installed
/// equipment without defining host geometry, navigation, or visual presentation.
/// </summary>
public sealed record LaboratoryFacility
{
    public LaboratoryFacility(
        string id,
        string version,
        string name,
        IEnumerable<LaboratoryRoom> rooms,
        IEnumerable<LaboratoryApparatus> apparatus,
        IEnumerable<LaboratoryInstrument> instruments,
        IEnumerable<MeasurementChannel> measurementChannels)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Name = Required(name, nameof(name));
        Rooms = Snapshot(rooms, nameof(rooms));
        Apparatus = Snapshot(apparatus, nameof(apparatus));
        Instruments = Snapshot(instruments, nameof(instruments));
        MeasurementChannels = Snapshot(measurementChannels, nameof(measurementChannels));

        EnsureUnique(Rooms.Select(x => x.Id), nameof(rooms));
        EnsureUnique(Apparatus.Select(x => x.Id), nameof(apparatus));
        EnsureUnique(Instruments.Select(x => x.Id), nameof(instruments));
        EnsureUnique(MeasurementChannels.Select(x => x.Id), nameof(measurementChannels));

        var roomIds = Rooms.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);
        var apparatusIds = Apparatus.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);
        var instrumentIds = Instruments.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);

        if (Apparatus.Any(x => !roomIds.Contains(x.RoomId)))
            throw new ArgumentException("Every apparatus item must reference a room in this facility.", nameof(apparatus));
        if (Instruments.Any(x => !apparatusIds.Contains(x.ApparatusId)))
            throw new ArgumentException("Every instrument must reference apparatus in this facility.", nameof(instruments));
        if (MeasurementChannels.Any(x => !instrumentIds.Contains(x.InstrumentId)))
            throw new ArgumentException("Every measurement channel must reference an instrument in this facility.", nameof(measurementChannels));
    }

    public string Id { get; }
    public string Version { get; }
    public string Name { get; }
    public IReadOnlyList<LaboratoryRoom> Rooms { get; }
    public IReadOnlyList<LaboratoryApparatus> Apparatus { get; }
    public IReadOnlyList<LaboratoryInstrument> Instruments { get; }
    public IReadOnlyList<MeasurementChannel> MeasurementChannels { get; }

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
            throw new ArgumentException("Manifest collections cannot contain null entries.", parameterName);
        return Array.AsReadOnly(snapshot);
    }

    private static void EnsureUnique(IEnumerable<string> ids, string parameterName)
    {
        if (ids.Distinct(StringComparer.Ordinal).Count() != ids.Count())
            throw new ArgumentException("Manifest identifiers must be unique within their collection.", parameterName);
    }
}

/// <summary>A named scientific work area. Hosts decide how, or whether, it is spatially rendered.</summary>
public sealed record LaboratoryRoom
{
    public LaboratoryRoom(string id, string name, string purpose)
    {
        Id = Required(id, nameof(id));
        Name = Required(name, nameof(name));
        Purpose = Required(purpose, nameof(purpose));
    }

    public string Id { get; }
    public string Name { get; }
    public string Purpose { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>
/// A reusable piece of laboratory apparatus. Interaction behavior is supplied by
/// capabilities/adapters; this manifest does not embed game or UI behavior.
/// </summary>
public sealed record LaboratoryApparatus
{
    public LaboratoryApparatus(string id, string name, string apparatusType, string roomId, string description)
    {
        Id = Required(id, nameof(id));
        Name = Required(name, nameof(name));
        ApparatusType = Required(apparatusType, nameof(apparatusType));
        RoomId = Required(roomId, nameof(roomId));
        Description = Required(description, nameof(description));
    }

    public string Id { get; }
    public string Name { get; }
    public string ApparatusType { get; }
    public string RoomId { get; }
    public string Description { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>
/// A measurement-capable instrument attached to apparatus. Calibration and model
/// semantics are explicit metadata, not implied by its display name.
/// </summary>
public sealed record LaboratoryInstrument
{
    public LaboratoryInstrument(
        string id,
        string name,
        string instrumentType,
        string apparatusId,
        string instrumentVersion,
        string calibrationReference)
    {
        Id = Required(id, nameof(id));
        Name = Required(name, nameof(name));
        InstrumentType = Required(instrumentType, nameof(instrumentType));
        ApparatusId = Required(apparatusId, nameof(apparatusId));
        InstrumentVersion = Required(instrumentVersion, nameof(instrumentVersion));
        CalibrationReference = Required(calibrationReference, nameof(calibrationReference));
    }

    public string Id { get; }
    public string Name { get; }
    public string InstrumentType { get; }
    public string ApparatusId { get; }
    public string InstrumentVersion { get; }
    public string CalibrationReference { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>A typed, unit-explicit observation stream exposed by an instrument.</summary>
public sealed record MeasurementChannel
{
    public MeasurementChannel(
        string id,
        string name,
        string instrumentId,
        string quantity,
        string unitSymbol,
        MeasurementSamplingMode samplingMode,
        double? samplesPerSecond = null)
    {
        Id = Required(id, nameof(id));
        Name = Required(name, nameof(name));
        InstrumentId = Required(instrumentId, nameof(instrumentId));
        Quantity = Required(quantity, nameof(quantity));
        UnitSymbol = Required(unitSymbol, nameof(unitSymbol));

        if (!Enum.IsDefined(samplingMode))
            throw new ArgumentOutOfRangeException(nameof(samplingMode));
        if (samplesPerSecond is not null &&
            (!double.IsFinite(samplesPerSecond.Value) || samplesPerSecond.Value <= 0d))
            throw new ArgumentOutOfRangeException(nameof(samplesPerSecond));
        if (samplingMode == MeasurementSamplingMode.Periodic && samplesPerSecond is null)
            throw new ArgumentException("Periodic channels require an explicit positive sample rate.", nameof(samplesPerSecond));
        if (samplingMode != MeasurementSamplingMode.Periodic && samplesPerSecond is not null)
            throw new ArgumentException("Only periodic channels may declare a sample rate.", nameof(samplesPerSecond));

        SamplingMode = samplingMode;
        SamplesPerSecond = samplesPerSecond;
    }

    public string Id { get; }
    public string Name { get; }
    public string InstrumentId { get; }
    public string Quantity { get; }
    public string UnitSymbol { get; }
    public MeasurementSamplingMode SamplingMode { get; }
    public double? SamplesPerSecond { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>Defines when an instrument emits observations, not how the host schedules execution.</summary>
public enum MeasurementSamplingMode
{
    OnDemand = 0,
    Periodic = 1,
    EventDriven = 2
}
