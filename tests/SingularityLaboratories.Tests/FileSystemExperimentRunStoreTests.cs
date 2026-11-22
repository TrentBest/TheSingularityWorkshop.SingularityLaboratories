using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class FileSystemExperimentRunStoreTests
{
    [Test]
    public async Task SaveAndGetAsync_RoundTripsCompleteRunProvenance()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentRunStore(directory);
            var expected = CreateRun();

            await store.SaveAsync(expected);
            var actual = await store.GetAsync(expected.RunId);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(actual!.Assumptions, Is.EqualTo(expected.Assumptions));
            Assert.That(actual.Limitations, Is.EqualTo(expected.Limitations));
            Assert.That(await store.ListAsync(), Is.EquivalentTo([expected.RunId]));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task SaveAsync_DoesNotOverwriteAnExistingRun()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentRunStore(directory);
            var run = CreateRun();

            await store.SaveAsync(run);

            Assert.ThrowsAsync<IOException>(async () => await store.SaveAsync(run));
            Assert.That(await store.GetAsync(run.RunId), Is.EqualTo(run));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task GetAsync_ReturnsNullForAnUnknownRun()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentRunStore(directory);

            Assert.That(await store.GetAsync(Guid.NewGuid()), Is.Null);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public void GetAsync_RejectsAnEmptyRunIdentity()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentRunStore(directory);

            Assert.ThrowsAsync<ArgumentException>(async () => await store.GetAsync(Guid.Empty));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static ExperimentRunRecord CreateRun()
    {
        var started = DateTimeOffset.Parse("2026-11-22T10:00:00+00:00");
        return new ExperimentRunRecord(
            Guid.NewGuid(),
            "thermal-lab",
            "1.0.0",
            "electrical-thermal-coupling",
            "1.0.0",
            "electrical-thermal-model",
            "0.1.0",
            1,
            "application/json",
            "{\"voltageV\":12}",
            ScientificValidationLevel.EducationalModel,
            started,
            started.AddSeconds(1),
            "application/json",
            "{\"energyJ\":240}",
            ["Ideal fixed-voltage DC bus."],
            ["No spatial temperature gradients."]);
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "singularity-laboratory-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
