using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentResultCatalogTests
{
    private string _directory = null!;

    [SetUp]
    public void SetUp() => _directory = Path.Combine(Path.GetTempPath(), "SingularityLaboratories", Guid.NewGuid().ToString("N"));

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }

    [Test]
    public async Task GetAsync_ReturnsHostNeutralProjectionOverPersistedRun()
    {
        var run = CreateRun(DateTimeOffset.Parse("2026-11-22T10:00:00+00:00"));
        var store = new FileSystemExperimentRunStore(_directory);
        await store.SaveAsync(run);
        var catalog = new ExperimentResultCatalog(store);

        var result = await catalog.GetAsync(run.RunId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Run.RunId, Is.EqualTo(run.RunId));
        Assert.That(result.Run, Is.Not.SameAs(run));
        Assert.That(result.Payload, Is.EqualTo(run.ResultPayload));
        Assert.That(result.ModelId, Is.EqualTo(run.ModelId));
    }

    [Test]
    public async Task ListAsync_OrdersByCompletionThenRunIdentity()
    {
        var store = new FileSystemExperimentRunStore(_directory);
        var later = CreateRun(DateTimeOffset.Parse("2026-11-22T10:00:02+00:00"));
        var firstAtSameTime = CreateRun(DateTimeOffset.Parse("2026-11-22T10:00:01+00:00"));
        var secondAtSameTime = CreateRun(DateTimeOffset.Parse("2026-11-22T10:00:01+00:00"));
        await store.SaveAsync(later);
        await store.SaveAsync(secondAtSameTime);
        await store.SaveAsync(firstAtSameTime);
        var catalog = new ExperimentResultCatalog(store);

        var results = await catalog.ListAsync();

        Assert.That(results.Select(x => x.RunId), Is.EqualTo(new[]
        {
            firstAtSameTime, secondAtSameTime, later
        }.OrderBy(x => x.CompletedAtUtc).ThenBy(x => x.RunId).Select(x => x.RunId)));
    }

    [Test]
    public async Task GetAsync_ReturnsNullForUnknownRun()
    {
        var catalog = new ExperimentResultCatalog(new FileSystemExperimentRunStore(_directory));

        Assert.That(await catalog.GetAsync(Guid.NewGuid()), Is.Null);
    }

    private static ExperimentRunRecord CreateRun(DateTimeOffset completed)
    {
        var runId = Guid.NewGuid();
        var started = completed.AddSeconds(-3);
        return new ExperimentRunRecord(
            runId,
            "thermal-lab", "1.0.0",
            "thermal-coupling", "2.1.0",
            "physics.thermal-model", "3.0.0",
            1, "application/json", "{\"voltageV\":12}",
            ScientificValidationLevel.EducationalModel,
            started, completed,
            "application/json", "{\"energyJ\":240}",
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);
    }
}
