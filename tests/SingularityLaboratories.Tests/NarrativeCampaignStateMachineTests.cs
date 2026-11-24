using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class NarrativeCampaignStateMachineTests
{
    private static readonly DateTimeOffset StartTime = new(2026, 11, 24, 2, 0, 0, TimeSpan.Zero);

    [Test]
    public void Start_AssignsFixedRoleAndLocationWithoutCharacterCreation()
    {
        var definition = CreateDefinition();
        var attempt = new NarrativeCampaignStateMachine().Start(definition, Guid.NewGuid(), StartTime);

        Assert.That(attempt.PlayerRole, Is.EqualTo("On-duty research technician"));
        Assert.That(attempt.StartingLocation, Is.EqualTo("Research workroom"));
        Assert.That(attempt.Outcome, Is.EqualTo(NarrativeCampaignOutcome.Active));
        Assert.That(attempt.Events, Has.Count.EqualTo(1));
        Assert.That(attempt.Events[0].Kind, Is.EqualTo(NarrativeCampaignEventKind.AttemptStarted));
    }

    [Test]
    public void SafeEscape_IsDeniedUntilEveryRequiredEvidenceItemIsCurrentAndVerified()
    {
        var machine = new NarrativeCampaignStateMachine();
        var definition = CreateDefinition();
        var attempt = machine.Start(definition, Guid.NewGuid(), StartTime);
        attempt = machine.RecordEvidence(definition, attempt, Evidence(
            NarrativeEvidenceKind.IncidentTimeline, NarrativeEvidenceState.VerifiedCurrent, "timeline"), StartTime.AddMinutes(2));
        attempt = machine.RecordEvidence(definition, attempt, Evidence(
            NarrativeEvidenceKind.FacilityState, NarrativeEvidenceState.VerifiedCurrent, "facility"), StartTime.AddMinutes(2));

        var denied = machine.AttemptSafeEscape(definition, attempt, "route-A", StartTime.AddMinutes(3));

        Assert.That(denied.Outcome, Is.EqualTo(NarrativeCampaignOutcome.Active));
        Assert.That(denied.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeDenied));
        Assert.That(denied.Events[^1].Details, Does.Contain(nameof(NarrativeEvidenceKind.EgressRecord)));
    }

    [Test]
    public void SafeEscape_SucceedsOnlyWhenAllThreeEvidenceRecordsAreVerifiedCurrent()
    {
        var machine = new NarrativeCampaignStateMachine();
        var definition = CreateDefinition();
        var attempt = machine.Start(definition, Guid.NewGuid(), StartTime);
        foreach (var kind in definition.RequiredSafeEscapeEvidence)
            attempt = machine.RecordEvidence(definition, attempt, Evidence(kind, NarrativeEvidenceState.VerifiedCurrent, kind.ToString()), StartTime.AddMinutes(2));

        var escaped = machine.AttemptSafeEscape(definition, attempt, "route-A", StartTime.AddMinutes(4));

        Assert.That(escaped.Outcome, Is.EqualTo(NarrativeCampaignOutcome.VerifiedSafeEscape));
        Assert.That(escaped.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeVerified));
        Assert.That(escaped.Evidence, Has.Count.EqualTo(3));
    }

    [TestCase(NarrativeEvidenceState.Observed)]
    [TestCase(NarrativeEvidenceState.Stale)]
    [TestCase(NarrativeEvidenceState.Contradicted)]
    public void SafeEscape_RejectsEvidenceThatIsNotVerifiedCurrent(NarrativeEvidenceState state)
    {
        var machine = new NarrativeCampaignStateMachine();
        var definition = CreateDefinition();
        var attempt = machine.Start(definition, Guid.NewGuid(), StartTime);
        foreach (var kind in definition.RequiredSafeEscapeEvidence)
            attempt = machine.RecordEvidence(definition, attempt, Evidence(
                kind,
                kind == NarrativeEvidenceKind.FacilityState ? state : NarrativeEvidenceState.VerifiedCurrent,
                kind.ToString()), StartTime.AddMinutes(2));

        var result = machine.AttemptSafeEscape(definition, attempt, "route-A", StartTime.AddMinutes(5));

        Assert.That(result.Outcome, Is.EqualTo(NarrativeCampaignOutcome.Active));
        Assert.That(result.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeDenied));
    }

    [Test]
    public void PlayerDeath_IsTerminalAndPreservesTheAttemptHistory()
    {
        var machine = new NarrativeCampaignStateMachine();
        var definition = CreateDefinition();
        var attempt = machine.Start(definition, Guid.NewGuid(), StartTime);
        attempt = machine.RecordEvidence(definition, attempt, Evidence(
            NarrativeEvidenceKind.IncidentTimeline, NarrativeEvidenceState.Observed, "timeline"), StartTime.AddMinutes(2));

        var dead = machine.RecordPlayerDeath(definition, attempt, "Fictional player health reached zero", StartTime.AddMinutes(2));

        Assert.That(dead.Outcome, Is.EqualTo(NarrativeCampaignOutcome.PlayerDeath));
        Assert.That(dead.Events.Select(item => item.Sequence), Is.EqualTo([1, 2, 3]));
        Assert.Throws<InvalidOperationException>(() =>
            machine.AttemptSafeEscape(definition, dead, "route-A", StartTime.AddMinutes(3)));
    }

    [Test]
    public void Restart_UsesANewAttemptAndDoesNotMutateThePreviousAttempt()
    {
        var machine = new NarrativeCampaignStateMachine();
        var definition = CreateDefinition();
        var first = machine.Start(definition, Guid.NewGuid(), StartTime);
        var dead = machine.RecordPlayerDeath(definition, first, "Terminal event", StartTime.AddMinutes(1));
        var second = machine.Start(definition, Guid.NewGuid(), StartTime.AddMinutes(2));

        Assert.That(second.AttemptId, Is.Not.EqualTo(dead.AttemptId));
        Assert.That(second.Events, Has.Count.EqualTo(1));
        Assert.That(dead.Outcome, Is.EqualTo(NarrativeCampaignOutcome.PlayerDeath));
        Assert.That(dead.Events, Has.Count.EqualTo(2));
    }

    [Test]
    public void EvidenceHistoryAllowsReinspectionButRejectsAnotherCampaign()
    {
        var machine = new NarrativeCampaignStateMachine();
        var definition = CreateDefinition();
        var attempt = machine.Start(definition, Guid.NewGuid(), StartTime);
        var evidence = Evidence(NarrativeEvidenceKind.IncidentTimeline, NarrativeEvidenceState.VerifiedCurrent, "timeline");
        attempt = machine.RecordEvidence(definition, attempt, evidence, StartTime.AddMinutes(2));

        attempt = machine.RecordEvidence(definition, attempt, Evidence(
            NarrativeEvidenceKind.IncidentTimeline, NarrativeEvidenceState.Stale, "timeline-refresh"), StartTime.AddMinutes(3));
        var denied = machine.AttemptSafeEscape(definition, attempt, "route-A", StartTime.AddMinutes(4));
        Assert.That(denied.Outcome, Is.EqualTo(NarrativeCampaignOutcome.Active));
        Assert.That(denied.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeDenied));
        Assert.Throws<ArgumentException>(() => machine.RecordEvidence(
            new NarrativeCampaignDefinition("other", "1.0.0", "Role", "Start", definition.RequiredSafeEscapeEvidence),
            attempt,
            Evidence(NarrativeEvidenceKind.FacilityState, NarrativeEvidenceState.VerifiedCurrent, "facility"), StartTime.AddMinutes(5)));
    }

    private static NarrativeCampaignDefinition CreateDefinition() => new(
        "night-signal-containment",
        "1.0.0",
        "On-duty research technician",
        "Research workroom",
        [
            NarrativeEvidenceKind.IncidentTimeline,
            NarrativeEvidenceKind.FacilityState,
            NarrativeEvidenceKind.EgressRecord
        ]);

    private static NarrativeEvidenceRecord Evidence(
        NarrativeEvidenceKind kind,
        NarrativeEvidenceState state,
        string reference) =>
        new(kind, $"source-{reference}", reference, state, StartTime.AddMinutes(1));
}
