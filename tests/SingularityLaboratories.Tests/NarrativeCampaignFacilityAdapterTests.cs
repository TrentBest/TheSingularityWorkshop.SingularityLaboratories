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
        var projection = new NarrativeCampaignFacilityAdapter().Bind(campaign, facility, CreateBinding(), CreateStaffAssignments(), StartTime);

        Assert.That(projection.CampaignId, Is.EqualTo(campaign.Id));
        Assert.That(projection.FacilityId, Is.EqualTo(facility.Id));
        Assert.That(projection.StartingLocationId, Is.EqualTo("research-workroom"));
        Assert.That(projection.StartingRoomId, Is.EqualTo("room-research"));
        Assert.That(projection.StaffAssignments, Has.Count.EqualTo(1));
        Assert.That(projection.StaffAssignments[0].Role, Is.EqualTo(LaboratoryStaffRole.Technician));
        Assert.That(projection.ResolveRoomId("records-room"), Is.EqualTo("room-records"));
        Assert.That(projection.ResolveApparatusIds("research-workroom"), Is.EqualTo(new[] { "workstation" }));
        Assert.That(projection.ResolveInstrumentIds("observation-gallery"), Is.EqualTo(new[] { "observation-monitor" }));
        Assert.That(projection.ResolveMeasurementChannelIds("transit-junction"), Is.EqualTo(new[] { "egress-state" }));
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
    public void Bind_RejectsInstrumentOrChannelNotAttachedToMappedLocation()
    {
        var binding = new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [new("research-workroom", "Research workroom", "room-research", ["workstation"], ["observation-monitor"])]);

        Assert.Throws<ArgumentException>(() =>
            new NarrativeCampaignFacilityAdapter().Bind(CreateCampaign(), CreateFacility(), binding));
    }

    [Test]
    public void Bind_RejectsMissingExpiredOrWrongRoleStaffAssignments()
    {
        var adapter = new NarrativeCampaignFacilityAdapter();
        var campaign = CreateCampaign();
        var facility = CreateFacility();
        var binding = CreateBinding();

        Assert.Throws<ArgumentException>(() => adapter.Bind(campaign, facility, binding, [], StartTime));
        Assert.Throws<ArgumentException>(() => adapter.Bind(
            campaign, facility, binding,
            [new LaboratoryStaffAssignment("player-tech-assignment", "northstar-lab", "opaque-principal", LaboratoryStaffRole.Technician, StartTime.AddMinutes(-10), expiresAtUtc: StartTime.AddMinutes(-1))],
            StartTime));
        Assert.Throws<ArgumentException>(() => adapter.Bind(
            campaign, facility, binding,
            [new LaboratoryStaffAssignment("player-tech-assignment", "northstar-lab", "opaque-principal", LaboratoryStaffRole.Researcher, StartTime.AddMinutes(-1))],
            StartTime));
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
            [
                new("observation-monitor", "Observation state monitor", "fictional-state-monitor", "observation-station", "1.0.0", "fictional-observation-state-v1"),
                new("operations-state", "Facility state monitor", "fictional-state-monitor", "operations-console", "1.0.0", "fictional-facility-state-v1"),
                new("egress-state", "Egress state monitor", "fictional-state-monitor", "route-terminal", "1.0.0", "fictional-egress-state-v1")
            ],
            [
                new("observation-state", "Observation state", "observation-monitor", "fictional-observation-state", "state", MeasurementSamplingMode.OnDemand),
                new("facility-state", "Facility state", "operations-state", "fictional-facility-state", "state", MeasurementSamplingMode.OnDemand),
                new("egress-state", "Egress state", "egress-state", "fictional-egress-state", "state", MeasurementSamplingMode.OnDemand)
            ]);

    private static IReadOnlyList<LaboratoryStaffAssignment> CreateStaffAssignments() =>
        [new("player-tech-assignment", "northstar-lab", "opaque-host-principal", LaboratoryStaffRole.Technician, StartTime.AddMinutes(-1))];

    private static NarrativeCampaignFacilityBinding CreateBinding() =>
        new(
            "night-signal",
            "1.0.0",
            "northstar-annex",
            "1.0.0",
            "research-workroom",
            [
                new("research-workroom", "Research workroom", "room-research", ["workstation"]),
                new("observation-gallery", "Observation gallery", "room-observation", ["observation-station"], ["observation-monitor"], ["observation-state"]),
                new("records-room", "Records room", "room-records", ["archive-terminal"]),
                new("facility-operations", "Facility operations", "room-operations", ["operations-console"], ["operations-state"], ["facility-state"]),
                new("transit-junction", "Transit junction", "room-transit", ["route-terminal"], ["egress-state"], ["egress-state"]),
                new("exterior-checkpoint", "Exterior checkpoint", "room-exterior", ["evidence-review"])
            ],
            "northstar-lab",
            [new("On-duty research technician", "player-tech-assignment", LaboratoryStaffRole.Technician)]);
}
