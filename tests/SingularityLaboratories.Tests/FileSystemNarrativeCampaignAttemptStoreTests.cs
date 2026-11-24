using System.Text.Json;
using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class FileSystemNarrativeCampaignAttemptStoreTests
{
    private static readonly DateTimeOffset StartTime = new(2026, 11, 24, 2, 0, 0, TimeSpan.Zero);

    [Test]
    public async Task SaveAndGetAsync_RoundTripsEvidenceAndAppendOnlyEventHistory()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            var attempt = CreateAttempt();
            attempt = new NarrativeCampaignStateMachine().AttemptSafeEscape(
                attempt.Definition, attempt, "route-B", StartTime.AddMinutes(5));

            await store.SaveAsync(attempt);
            var restored = await store.GetAsync(attempt.AttemptId);

            Assert.That(restored, Is.Not.Null);
            Assert.That(restored!.AttemptId, Is.EqualTo(attempt.AttemptId));
            Assert.That(restored.CampaignId, Is.EqualTo(attempt.CampaignId));
            Assert.That(restored.Definition.RequiredSafeEscapeEvidence,
                Is.EqualTo(attempt.Definition.RequiredSafeEscapeEvidence));
            Assert.That(restored.Evidence, Has.Count.EqualTo(3));
            Assert.That(restored.Events.Select(item => item.Sequence),
                Is.EqualTo(attempt.Events.Select(item => item.Sequence)));
            Assert.That(restored.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeDenied));
            Assert.That(restored.Outcome, Is.EqualTo(NarrativeCampaignOutcome.Active));
            Assert.That(await store.ListAsync(), Is.EquivalentTo([attempt.AttemptId]));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task SaveAndGetAsync_RestoresVerifiedSafeEscapeAsTerminal()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            var attempt = CreateAttempt();
            attempt = new NarrativeCampaignStateMachine().AttemptSafeEscape(
                attempt.Definition, attempt, "route-A", StartTime.AddMinutes(5));

            await store.SaveAsync(attempt);
            var restored = await store.GetAsync(attempt.AttemptId);

            Assert.That(restored!.Outcome, Is.EqualTo(NarrativeCampaignOutcome.VerifiedSafeEscape));
            Assert.That(restored.Events[^1].Kind, Is.EqualTo(NarrativeCampaignEventKind.SafeEscapeVerified));
            Assert.Throws<ArgumentException>(() => NarrativeCampaignAttempt.Restore(
                restored.AttemptId,
                restored.Definition,
                NarrativeCampaignOutcome.VerifiedSafeEscape,
                [],
                restored.Events));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task SaveAsync_DoesNotOverwriteAnExistingAttempt()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            var attempt = CreateAttempt();
            await store.SaveAsync(attempt);

            Assert.ThrowsAsync<IOException>(async () => await store.SaveAsync(attempt));
            Assert.That((await store.GetAsync(attempt.AttemptId))!.Events, Has.Count.EqualTo(attempt.Events.Count));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task GetAsync_ReturnsNullForUnknownAttempt()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            Assert.That(await store.GetAsync(Guid.NewGuid()), Is.Null);
            Assert.ThrowsAsync<ArgumentException>(async () => await store.GetAsync(Guid.Empty));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task GetAsync_RejectsPersistedIdentityMismatch()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemNarrativeCampaignAttemptStore(directory);
            var attempt = CreateAttempt();
            await store.SaveAsync(attempt);
            var path = Path.Combine(directory, $"{attempt.AttemptId:N}.json");
            var json = await File.ReadAllTextAsync(path);
            var otherId = Guid.NewGuid();
            await File.WriteAllTextAsync(path, json.Replace(attempt.AttemptId.ToString(), otherId.ToString(), StringComparison.Ordinal));

            Assert.ThrowsAsync<InvalidDataException>(async () => await store.GetAsync(attempt.AttemptId));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static NarrativeCampaignAttempt CreateAttempt()
    {
        var definition = new NarrativeCampaignDefinition(
            "night-signal",
            "1.0.0",
            "On-duty research technician",
            "Research workroom",
            [
                NarrativeEvidenceKind.IncidentTimeline,
                NarrativeEvidenceKind.FacilityState,
                NarrativeEvidenceKind.EgressRecord
            ]);
        var machine = new NarrativeCampaignStateMachine();
        var attempt = machine.Start(definition, Guid.NewGuid(), StartTime);
        foreach (var kind in definition.RequiredSafeEscapeEvidence)
        {
            var reference = kind == NarrativeEvidenceKind.EgressRecord ? "route-A" : kind.ToString();
            attempt = machine.RecordEvidence(
                definition,
                attempt,
                new NarrativeEvidenceRecord(
                    kind,
                    $"source-{kind}",
                    reference,
                    NarrativeEvidenceState.VerifiedCurrent,
                    StartTime.AddMinutes(2)),
                StartTime.AddMinutes(3));
        }

        return attempt;
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "singularity-narrative-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
