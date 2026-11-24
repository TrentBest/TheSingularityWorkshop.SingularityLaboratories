using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class NarrativeCampaignFacilityAdapterTests
{
    [Test]
    public void Bind_ResolvesFixedOpeningAndEveryNarrativeAreaToStableFacilityRooms()
    {
        var campaign = CreateCampaign();
        var facility = CreateFacility();
        var projection = new NarrativeCampaignFacilityAdapter().Bind(campaign, facility, CreateBinding());

        Assert.That(projection.CampaignId, Is.EqualTo(campaign.Id));
        Assert.That(projection.FacilityId, Is.EqualTo(facility.Id));
        Assert.That(projection.StartingLocationId, Is.EqualTo("research-workroom"));
        Assert.That(projection.StartingRoomId, Is.EqualTo("room-research"));
        Assert.That(projection.ResolveRoomId("records-room"), Is.EqualTo("room-records"));
        Assert.That(projection.ResolveApparatusIds("research-workroom"), Is.EqualTo(new[] { "workstation" }));
        Assert.That(projection.ResolveRoomId("exterior-checkpoint"), Is.EqualTo("room-exterior"));
        Assert.Throws<KeyNotFoundException>(() => projection.ResolveRoomId("unknown-area"));
    }

    [Test]
    public void Bind_RejectsCampaignIdentityOrVersionMismatch()
    {
        var binding = CreateBinding();
        var changedCampaign = new NarrativeCampaignDefinition(
            "night-signal", "2.0.0", "On-duty research technician", "Research workroom",
            [NarrativeEvidenceKind.IncidentTimeline, NarrativeEvidenceKind.FacilityState, NarrativeEvidenceKind.EgressRecord]);

        Assert.Throws<ArgumentException>(() =>
            new NarrativeCampaignFacilityAdapter().Bind(changedCampaign, CreateFacility(), binding));
    }

    [Test]
    public void Bind_RejectsFacilityIdentityOrVersionMismatch()
    {
        var facility = CreateFacility();
        var changedFacility = new LaboratoryFacility(
            facility.Id, "2.0.0", facility.Name, facility.Rooms, [], [], []);

        Assert.Throws<ArgumentException>(() =>
            new NarrativeCampaignFacilityAdapter().Bind(CreateCampaign(), changedFacility, CreateBinding()));
    }

    [Test]
    public void Bind_RejectsUnknownRoomReferences()
    {
        var binding = new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [
                new("research-workroom", "Research workroom", "missing-room"),
                new("observation-gallery", "Observation gallery", "room-observation")
            ]);

        Assert.Throws<ArgumentException>(() =>
            new NarrativeCampaignFacilityAdapter().Bind(CreateCampaign(), CreateFacility(), binding));
    }

    [Test]
    public void Bind_RequiresMappedStartingLocationToMatchCampaignOpening()
    {
        var binding = new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [new("research-workroom", "Records room", "room-research")]);

        Assert.Throws<ArgumentException>(() =>
            new NarrativeCampaignFacilityAdapter().Bind(CreateCampaign(), CreateFacility(), binding));
    }

    [Test]
    public void Bind_RejectsApparatusMappedToTheWrongRoom()
    {
        var binding = new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [new("research-workroom", "Research workroom", "room-research", ["archive-terminal"])]);

        Assert.Throws<ArgumentException>(() =>
            new NarrativeCampaignFacilityAdapter().Bind(CreateCampaign(), CreateFacility(), binding));
    }

    [Test]
    public void FacilityBinding_RejectsDuplicateNarrativeLocationsAndRoomAssignments()
    {
        Assert.Throws<ArgumentException>(() => new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [
                new("research-workroom", "Research workroom", "room-research"),
                new("research-workroom", "Duplicate", "room-records")
            ]));

        Assert.Throws<ArgumentException>(() => new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [
                new("research-workroom", "Research workroom", "room-research"),
                new("records-room", "Records room", "room-research")
            ]));
    }

    private static NarrativeCampaignDefinition CreateCampaign() =>
        new(
            "night-signal",
            "1.0.0",
            "On-duty research technician",
            "Research workroom",
            [
                NarrativeEvidenceKind.IncidentTimeline,
                NarrativeEvidenceKind.FacilityState,
                NarrativeEvidenceKind.EgressRecord
            ]);

    private static LaboratoryFacility CreateFacility() =>
        new(
            "northstar-annex",
            "1.0.0",
            "Northstar Institute — Sublevel Research Annex",
            [
                new("room-research", "Research workroom", "Fixed campaign opening and workstation."),
                new("room-observation", "Observation gallery", "Fictional observation area."),
                new("room-records", "Records room", "Incident records and archive."),
                new("room-operations", "Facility operations", "Fictional facility status."),
                new("room-transit", "Transit junction", "Route decision and egress record."),
                new("room-exterior", "Exterior checkpoint", "Terminal campaign review.")
            ],
            [
                new("workstation", "Workstation", "workstation", "room-research", "Fictional campaign interface."),
                new("observation-station", "Observation station", "observation-console", "room-observation", "Fictional campaign interface."),
                new("archive-terminal", "Archive terminal", "archive-console", "room-records", "Fictional campaign interface."),
                new("operations-console", "Operations console", "status-console", "room-operations", "Fictional campaign interface."),
                new("route-terminal", "Route terminal", "route-console", "room-transit", "Fictional campaign interface."),
                new("evidence-review", "Evidence review station", "review-console", "room-exterior", "Fictional campaign interface.")
            ],
            [],
            []);

    private static NarrativeCampaignFacilityBinding CreateBinding() =>
        new(
            "night-signal",
            "1.0.0",
            "northstar-annex",
            "1.0.0",
            "research-workroom",
            [
                new("research-workroom", "Research workroom", "room-research", ["workstation"]),
                new("observation-gallery", "Observation gallery", "room-observation", ["observation-station"]),
                new("records-room", "Records room", "room-records", ["archive-terminal"]),
                new("facility-operations", "Facility operations", "room-operations", ["operations-console"]),
                new("transit-junction", "Transit junction", "room-transit", ["route-terminal"]),
                new("exterior-checkpoint", "Exterior checkpoint", "room-exterior", ["evidence-review"])
            ]);
}
