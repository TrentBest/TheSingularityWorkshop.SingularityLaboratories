using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class TeachingResearchCampusFixtureTests
{
    [Test]
    public void Create_ComposesTeachingAndResearchWorkspacesWithTheirOwnFacilities()
    {
        var campus = TeachingResearchCampusFixture.Create();

        Assert.That(campus.Id, Is.EqualTo("singularity-teaching-research-campus"));
        Assert.That(campus.Laboratories.Select(x => x.Id),
            Is.EquivalentTo(["teaching-physics", "materials-research"]));
        Assert.That(campus.Facilities.Select(x => x.Id),
            Is.EquivalentTo(["teaching-hall", "materials-research-hall"]));
        Assert.That(campus.Workspaces, Has.Count.EqualTo(2));

        var teaching = campus.Workspaces.Single(x => x.LaboratoryId == "teaching-physics");
        Assert.That(teaching.FacilityId, Is.EqualTo("teaching-hall"));
        Assert.That(teaching.RoomId, Is.EqualTo("teaching-bench-room"));

        var research = campus.Workspaces.Single(x => x.LaboratoryId == "materials-research");
        Assert.That(research.FacilityId, Is.EqualTo("materials-research-hall"));
        Assert.That(research.RoomId, Is.EqualTo("mechanical-characterization-room"));
    }

    [Test]
    public void Create_DeclaresUnitExplicitMeasurementChannelsAndSampleCalibrationReferences()
    {
        var campus = TeachingResearchCampusFixture.Create();
        var teaching = campus.Facilities.Single(x => x.Id == "teaching-hall");
        var research = campus.Facilities.Single(x => x.Id == "materials-research-hall");

        Assert.That(teaching.MeasurementChannels.Single().UnitSymbol, Is.EqualTo("K"));
        Assert.That(teaching.MeasurementChannels.Single().SamplingMode,
            Is.EqualTo(MeasurementSamplingMode.Periodic));
        Assert.That(teaching.MeasurementChannels.Single().SamplesPerSecond, Is.EqualTo(1d));
        Assert.That(teaching.Instruments.Single().CalibrationReference,
            Is.EqualTo("SAMPLE-CAL-TEACHING-001"));

        Assert.That(research.MeasurementChannels.Single().UnitSymbol, Is.EqualTo("N"));
        Assert.That(research.MeasurementChannels.Single().SamplingMode,
            Is.EqualTo(MeasurementSamplingMode.OnDemand));
        Assert.That(research.Instruments.Single().CalibrationReference,
            Is.EqualTo("SAMPLE-CAL-RESEARCH-001"));
    }
}
