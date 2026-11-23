using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryCampusTests
{
    [Test]
    public void Constructor_CapturesWorkspacePlacementAcrossPortableLabsAndFacilities()
    {
        var laboratories = new List<LaboratoryDefinition>
        {
            new("physics-teaching", "1.0.0", "Physics teaching lab", "Supervised educational experiments.")
        };
        var facilities = new List<LaboratoryFacility>
        {
            new("teaching-building", "1.0.0", "Teaching building",
                [new LaboratoryRoom("thermal-room", "Thermal room", "Low-energy thermal experiments.")],
                [], [], [])
        };
        var workspaces = new List<LaboratoryCampusWorkspace>
        {
            new("physics-workspace", "physics-teaching", "teaching-building", "thermal-room")
        };

        var campus = new LaboratoryCampus(
            "flagship-campus", "1.0.0", "Flagship campus", "Original teaching and research campus.",
            laboratories, facilities, workspaces);

        laboratories.Clear();
        facilities.Clear();
        workspaces.Clear();

        Assert.That(campus.Laboratories, Has.Count.EqualTo(1));
        Assert.That(campus.Facilities, Has.Count.EqualTo(1));
        Assert.That(campus.Workspaces, Has.Count.EqualTo(1));
        Assert.That(campus.Workspaces[0].LaboratoryId, Is.EqualTo("physics-teaching"));
        Assert.That(campus.Workspaces[0].FacilityId, Is.EqualTo("teaching-building"));
        Assert.That(campus.Workspaces[0].RoomId, Is.EqualTo("thermal-room"));
    }

    [Test]
    public void Constructor_RejectsWorkspaceWithUnknownLaboratory()
    {
        var facility = new LaboratoryFacility(
            "building", "1.0.0", "Building",
            [new LaboratoryRoom("room", "Room", "Purpose.")], [], [], []);

        Assert.Throws<ArgumentException>(() => new LaboratoryCampus(
            "campus", "1.0.0", "Campus", "Description.",
            [], [facility],
            [new LaboratoryCampusWorkspace("workspace", "missing-lab", "building", "room")]));
    }

    [Test]
    public void Constructor_RejectsWorkspaceWithUnknownRoom()
    {
        var laboratory = new LaboratoryDefinition("physics", "1.0.0", "Physics", "Description.");
        var facility = new LaboratoryFacility(
            "building", "1.0.0", "Building",
            [new LaboratoryRoom("room", "Room", "Purpose.")], [], [], []);

        Assert.Throws<ArgumentException>(() => new LaboratoryCampus(
            "campus", "1.0.0", "Campus", "Description.",
            [laboratory], [facility],
            [new LaboratoryCampusWorkspace("workspace", "physics", "building", "missing-room")]));
    }

    [Test]
    public void Constructor_RejectsDuplicateWorkspaceIdentifiers()
    {
        var laboratory = new LaboratoryDefinition("physics", "1.0.0", "Physics", "Description.");
        var facility = new LaboratoryFacility(
            "building", "1.0.0", "Building",
            [new LaboratoryRoom("room", "Room", "Purpose.")], [], [], []);
        var placement = new LaboratoryCampusWorkspace("workspace", "physics", "building", "room");

        Assert.Throws<ArgumentException>(() => new LaboratoryCampus(
            "campus", "1.0.0", "Campus", "Description.",
            [laboratory], [facility], [placement, placement]));
    }
}
