using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class NarrativeCampaignFacilityStatusAdapterTests
{
    private static readonly DateTimeOffset CapturedAt = new(2026, 11, 24, 3, 0, 0, TimeSpan.Zero);

    [Test]
    public void Project_MapsCompleteHostStatusSnapshotToStableNarrativeLocations()
    {
        var projection = CreateFacilityProjection();
        var snapshot = new NarrativeCampaignFacilityStatusAdapter().Project(
            projection,
            CreateObservations(),
            CapturedAt);

        Assert.That(snapshot.CampaignId, Is.EqualTo("night-signal"));
        Assert.That(snapshot.FacilityId, Is.EqualTo("northstar-annex"));
        Assert.That(snapshot.Locations, Has.Count.EqualTo(6));
        var operations = snapshot.Locations.Single(item => item.LocationId == "facility-operations");
        Assert.That(operations.RoomId, Is.EqualTo("room-operations"));
        Assert.That(operations.StatusCode, Is.EqualTo("fictional-state-alert"));
        Assert.That(operations.SourceId, Is.EqualTo("host-status-feed"));
        Assert.That(operations.IsCurrent, Is.True);
    }

    [Test]
    public void Project_PreservesHostFreshnessWithoutInferringFromObservationAge()
    {
        var observations = CreateObservations()
            .Select(item => item.RoomId == "room-research"
                ? new NarrativeFacilityStatusObservation(
                    item.RoomId, item.StatusCode, item.SourceId, CapturedAt.AddDays(-30), isCurrent: true)
                : item)
            .ToArray();

        var snapshot = new NarrativeCampaignFacilityStatusAdapter().Project(
            CreateFacilityProjection(), observations, CapturedAt);

        Assert.That(snapshot.Locations.Single(item => item.RoomId == "room-research").IsCurrent, Is.True);
    }

    [Test]
    public void Project_RejectsUnknownDuplicateMissingAndFutureRoomObservations()
    {
        var adapter = new NarrativeCampaignFacilityStatusAdapter();
        var projection = CreateFacilityProjection();
        var complete = CreateObservations();

        Assert.Throws<ArgumentException>(() => adapter.Project(
            projection,
            complete.Append(new NarrativeFacilityStatusObservation(
                "unknown-room", "unknown", "host", CapturedAt.AddMinutes(-1), true)),
            CapturedAt));

        Assert.Throws<ArgumentException>(() => adapter.Project(
            projection,
            complete.Append(complete[0]),
            CapturedAt));

        Assert.Throws<ArgumentException>(() => adapter.Project(
            projection,
            complete.Skip(1),
            CapturedAt));

        Assert.Throws<ArgumentException>(() => adapter.Project(
            projection,
            complete.Select(item => item.RoomId == "room-research"
                ? new NarrativeFacilityStatusObservation(
                    item.RoomId, item.StatusCode, item.SourceId, CapturedAt.AddMinutes(1), item.IsCurrent)
                : item),
            CapturedAt));
    }

    [Test]
    public void Observation_RequiresUtcTimestamp()
    {
        Assert.Throws<ArgumentException>(() => new NarrativeFacilityStatusObservation(
            "room-research", "available", "host", new DateTimeOffset(2026, 11, 24, 3, 0, 0, TimeSpan.FromHours(-8)), true));
    }

    private static NarrativeCampaignFacilityProjection CreateFacilityProjection()
    {
        var campaign = new NarrativeCampaignDefinition(
            "night-signal", "1.0.0", "On-duty research technician", "Research workroom",
            [NarrativeEvidenceKind.IncidentTimeline, NarrativeEvidenceKind.FacilityState, NarrativeEvidenceKind.EgressRecord]);
        var facility = new LaboratoryFacility(
            "northstar-annex", "1.0.0", "Northstar Institute — Sublevel Research Annex",
            [
                new("room-research", "Research workroom", "Fixed campaign opening."),
                new("room-observation", "Observation gallery", "Fictional observation area."),
                new("room-records", "Records room", "Incident records."),
                new("room-operations", "Facility operations", "Fictional status area."),
                new("room-transit", "Transit junction", "Route decision."),
                new("room-exterior", "Exterior checkpoint", "Terminal review.")
            ],
            [], [], []);
        var binding = new NarrativeCampaignFacilityBinding(
            "night-signal", "1.0.0", "northstar-annex", "1.0.0", "research-workroom",
            [
                new("research-workroom", "Research workroom", "room-research"),
                new("observation-gallery", "Observation gallery", "room-observation"),
                new("records-room", "Records room", "room-records"),
                new("facility-operations", "Facility operations", "room-operations"),
                new("transit-junction", "Transit junction", "room-transit"),
                new("exterior-checkpoint", "Exterior checkpoint", "room-exterior")
            ]);
        return new NarrativeCampaignFacilityAdapter().Bind(campaign, facility, binding);
    }

    private static IReadOnlyList<NarrativeFacilityStatusObservation> CreateObservations() =>
    [
        new("room-research", "fictional-state-available", "host-status-feed", CapturedAt.AddMinutes(-1), true),
        new("room-observation", "fictional-state-available", "host-status-feed", CapturedAt.AddMinutes(-1), true),
        new("room-records", "fictional-state-unknown", "host-status-feed", CapturedAt.AddMinutes(-1), false),
        new("room-operations", "fictional-state-alert", "host-status-feed", CapturedAt.AddMinutes(-1), true),
        new("room-transit", "fictional-state-unavailable", "host-status-feed", CapturedAt.AddMinutes(-1), true),
        new("room-exterior", "fictional-state-available", "host-status-feed", CapturedAt.AddMinutes(-1), true)
    ];
}
