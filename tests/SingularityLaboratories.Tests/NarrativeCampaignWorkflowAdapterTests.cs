using System.Text.Json;
using NUnit.Framework;
using TheSingularityWorkshop.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;
using TheSingularityWorkshop.SingularityLaboratories.ExecutionAdapter;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class NarrativeCampaignWorkflowAdapterTests
{
    private static readonly DateTimeOffset StartTime = new(2026, 11, 24, 2, 0, 0, TimeSpan.Zero);

    [Test]
    public async Task Workflow_ExecutesEvidenceGateAndPersistsVerifiedSafeEscape()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var definition = CreateDefinition();
            var request = new NarrativeCampaignExecutionRequest(
                definition,
                Guid.NewGuid(),
                StartTime,
                [
                    EvidenceCommand(NarrativeEvidenceKind.IncidentTimeline, "timeline"),
                    EvidenceCommand(NarrativeEvidenceKind.FacilityState, "facility"),
                    EvidenceCommand(NarrativeEvidenceKind.EgressRecord, "route-A"),
                    NarrativeCampaignCommand.AttemptSafeEscape("route-A", StartTime.AddMinutes(5))
                ]);
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            var registry = new WorkflowHandlerRegistry();
            NarrativeCampaignWorkflowAdapter.Register(registry, store);
            await using var coordinator = new WorkflowExecutionCoordinator(registry);
            var context = CreateContext(request);
            var accepted = coordinator.Submit(CreateWorkflow(), context);

            var result = await coordinator.StartAsync(accepted.RunId);

            Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Succeeded));
            var output = context.GetOutput<NarrativeCampaignAttemptSnapshot>(NarrativeCampaignWorkflowAdapter.AttemptOutputKey);
            Assert.That(output, Is.Not.Null);
            Assert.That(output!.Outcome, Is.EqualTo(NarrativeCampaignOutcome.VerifiedSafeEscape));
            Assert.That(output.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeVerified));
            var persisted = await store.GetAsync(request.AttemptId);
            Assert.That(persisted!.Outcome, Is.EqualTo(NarrativeCampaignOutcome.VerifiedSafeEscape));
            Assert.That(persisted.Events.Select(item => item.Sequence),
                Is.EqualTo(output.Events.Select(item => item.Sequence)));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task Workflow_RecordsPlayerDeathAsTerminalOutcome()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var request = new NarrativeCampaignExecutionRequest(
                CreateDefinition(),
                Guid.NewGuid(),
                StartTime,
                [NarrativeCampaignCommand.RecordPlayerDeath("Fictional player health reached zero", StartTime.AddMinutes(1))]);
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            var registry = new WorkflowHandlerRegistry();
            NarrativeCampaignWorkflowAdapter.Register(registry, store);
            await using var coordinator = new WorkflowExecutionCoordinator(registry);
            var context = CreateContext(request);
            var accepted = coordinator.Submit(CreateWorkflow(), context);

            var result = await coordinator.StartAsync(accepted.RunId);

            Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Succeeded));
            var persisted = await store.GetAsync(request.AttemptId);
            Assert.That(persisted!.Outcome, Is.EqualTo(NarrativeCampaignOutcome.PlayerDeath));
            Assert.That(persisted.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.PlayerDeath));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public void Request_RejectsCommandsAfterTerminalOutcome()
    {
        Assert.Throws<ArgumentException>(() => new NarrativeCampaignExecutionRequest(
            CreateDefinition(),
            Guid.NewGuid(),
            StartTime,
            [
                NarrativeCampaignCommand.RecordPlayerDeath("Terminal event", StartTime.AddMinutes(1)),
                EvidenceCommand(NarrativeEvidenceKind.IncidentTimeline, "timeline")
            ]));
    }

    private static NarrativeCampaignDefinition CreateDefinition() =>
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

    private static NarrativeCampaignCommand EvidenceCommand(NarrativeEvidenceKind kind, string reference) =>
        NarrativeCampaignCommand.RecordEvidence(
            new NarrativeEvidenceRecord(
                kind,
                $"source-{kind}",
                reference,
                NarrativeEvidenceState.VerifiedCurrent,
                StartTime.AddMinutes(2)),
            StartTime.AddMinutes(3));

    private static WorkflowExecutionContext CreateContext(NarrativeCampaignExecutionRequest request) =>
        new(new Dictionary<string, JsonElement>
        {
            [NarrativeCampaignWorkflowAdapter.RequestInputKey] = JsonSerializer.SerializeToElement(request)
        });

    private static WorkflowDefinition CreateWorkflow() =>
        new(
            "laboratory.narrativeCampaign.v1",
            "Execute narrative laboratory campaign",
            1,
            [new WorkflowStep("run-narrative-campaign", NarrativeCampaignWorkflowAdapter.HandlerKey)]);

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "singularity-narrative-workflow-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
