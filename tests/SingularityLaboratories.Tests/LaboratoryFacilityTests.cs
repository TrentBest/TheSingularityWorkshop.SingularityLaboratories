using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryFacilityTests
{
    [Test]
    public void Constructor_CapturesPortableFacilityManifestAndMeasurementSemantics()
    {
        var rooms = new List<LaboratoryRoom>
        {
            new("bench", "Thermal bench", "Controlled thermal experiments.")
        };
        var facility = new LaboratoryFacility(
            "teaching-lab",
            "1.0.0",
            "Teaching laboratory",
            rooms,
            [new LaboratoryApparatus("supply", "DC supply", "power-source", "bench", "Variable low-voltage supply.")],
            [new LaboratoryInstrument("voltmeter", "Voltmeter", "voltage-meter", "supply", "1.2.0", "CAL-2026-014")],
            [new MeasurementChannel("bus-voltage", "Bus voltage", "voltmeter", "electric-potential", "V", MeasurementSamplingMode.Periodic, 10d)]);

        rooms.Clear();

        Assert.That(facility.Rooms, Has.Count.EqualTo(1));
        Assert.That(facility.Apparatus[0].RoomId, Is.EqualTo("bench"));
        Assert.That(facility.Instruments[0].ApparatusId, Is.EqualTo("supply"));
        Assert.That(facility.MeasurementChannels[0].InstrumentId, Is.EqualTo("voltmeter"));
        Assert.That(facility.MeasurementChannels[0].UnitSymbol, Is.EqualTo("V"));
        Assert.That(facility.MeasurementChannels[0].SamplesPerSecond, Is.EqualTo(10d));
    }

    [Test]
    public void Constructor_RejectsApparatusReferencingUnknownRoom()
    {
        Assert.Throws<ArgumentException>(() => new LaboratoryFacility(
            "lab",
            "1.0.0",
            "Lab",
            [new LaboratoryRoom("room-a", "Room A", "Purpose.")],
            [new LaboratoryApparatus("apparatus-a", "Apparatus A", "generic", "missing-room", "Description.")],
            [],
            []));
    }

    [TestCase(null)]
    [TestCase(0d)]
    [TestCase(-1d)]
    [TestCase(double.NaN)]
    [TestCase(double.PositiveInfinity)]
    public void PeriodicMeasurementChannel_RejectsMissingOrInvalidRate(double? rate)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MeasurementChannel(
            "channel",
            "Channel",
            "instrument",
            "temperature",
            "K",
            MeasurementSamplingMode.Periodic,
            rate));
    }

    [Test]
    public void OnDemandMeasurementChannel_RejectsUnneededSamplingRate()
    {
        Assert.Throws<ArgumentException>(() => new MeasurementChannel(
            "channel",
            "Channel",
            "instrument",
            "temperature",
            "K",
            MeasurementSamplingMode.OnDemand,
            1d));
    }
}
