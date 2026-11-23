using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryCampusOverviewTests
{
    [Test]
    public void Create_ResolvesWorkspaceRelationshipsIntoSemanticPresentationData()
    {
        var campus = TeachingResearchCampusFixture.Create();

        var overview = LaboratoryCampusOverview.Create(campus);

        Assert.That(overview.Id, Is.EqualTo(campus.Id));
        Assert.That(overview.Name, Is.EqualTo(campus.Name));
        Assert.That(overview.Workspaces, Has.Count.EqualTo(2));

        var teaching = overview.Workspaces.Single(x => x.Id == "teaching-physics-workspace");
        Assert.That(teaching.LaboratoryName, Is.EqualTo("Teaching Physics Laboratory"));
        Assert.That(teaching.FacilityName, Is.EqualTo("Teaching and Demonstration Hall"));
        Assert.That(teaching.RoomName, Is.EqualTo("Measurement Benches"));
        Assert.That(teaching.Apparatus.Single().Name, Is.EqualTo("Insulated Calorimetry Station"));

        var temperature = teaching.Instruments.Single().MeasurementChannels.Single();
        Assert.That(temperature.Name, Is.EqualTo("Sample Temperature"));
        Assert.That(temperature.Quantity, Is.EqualTo("thermodynamic-temperature"));
        Assert.That(temperature.UnitSymbol, Is.EqualTo("K"));
        Assert.That(temperature.SamplesPerSecond, Is.EqualTo(1d));

        var research = overview.Workspaces.Single(x => x.Id == "materials-research-workspace");
        Assert.That(research.RoomPurpose, Is.EqualTo("Controlled observation of material response."));
        Assert.That(research.Instruments.Single().MeasurementChannels.Single().UnitSymbol, Is.EqualTo("N"));
    }

    [Test]
    public void Create_DoesNotMutateOrReplaceTheSourceCampusDefinitions()
    {
        var campus = TeachingResearchCampusFixture.Create();

        var overview = LaboratoryCampusOverview.Create(campus);

        Assert.That(overview.Workspaces.Select(x => x.Id),
            Is.EqualTo(campus.Workspaces.Select(x => x.Id)));
        Assert.That(campus.Facilities.Single(x => x.Id == "teaching-hall")
            .MeasurementChannels.Single().UnitSymbol, Is.EqualTo("K"));
    }

    [Test]
    public void Create_RejectsNullCampus()
    {
        Assert.That(() => LaboratoryCampusOverview.Create(null!),
            Throws.ArgumentNullException);
    }
}
