using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class FileSystemExperimentCampaignExecutionStoreTests
{
    [Test]
    public async Task SaveAndGetAsync_RoundTripsCampaignOutcomeAndEmbeddedRunLineage()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentCampaignExecutionStore(directory);
            var expected = CreateExecution();

            await store.SaveAsync(expected);
            var actual = await store.GetAsync(expected.ExecutionId);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual!.ExecutionId, Is.EqualTo(expected.ExecutionId));
            Assert.That(actual.CampaignId, Is.EqualTo(expected.CampaignId));
            Assert.That(actual.CampaignVersion, Is.EqualTo(expected.CampaignVersion));
            Assert.That(actual.CampaignDefinition.Scenarios[1].Configuration.Payload, Is.EqualTo("""{"voltageV":15}"""));
            Assert.That(actual.LaboratoryId, Is.EqualTo(expected.LaboratoryId));
            Assert.That(actual.LaboratoryVersion, Is.EqualTo(expected.LaboratoryVersion));
            Assert.That(actual.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.PartiallyCompleted));
            Assert.That(actual.Scenarios, Has.Count.EqualTo(2));
            Assert.That(actual.Scenarios[0].Run!.RunId, Is.EqualTo(expected.Scenarios[0].Run!.RunId));
            Assert.That(actual.Scenarios[0].Run!.ConfigurationPayload, Is.EqualTo(expected.Scenarios[0].Run!.ConfigurationPayload));
            Assert.That(actual.Scenarios[0].Run!.ResultPayload, Is.EqualTo(expected.Scenarios[0].Run!.ResultPayload));
            Assert.That(actual.Scenarios[1].FailureType, Is.EqualTo(expected.Scenarios[1].FailureType));
            Assert.That(await store.ListAsync(), Is.EquivalentTo([expected.ExecutionId]));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task SaveAsync_DoesNotOverwriteAnExistingExecution()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentCampaignExecutionStore(directory);
            var execution = CreateExecution();
            await store.SaveAsync(execution);

            Assert.ThrowsAsync<IOException>(async () => await store.SaveAsync(execution));
            var persisted = await store.GetAsync(execution.ExecutionId);
            Assert.That(persisted!.Scenarios[0].Run!.RunId, Is.EqualTo(execution.Scenarios[0].Run!.RunId));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task GetAsync_ReturnsNullForUnknownExecution()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentCampaignExecutionStore(directory);
            Assert.That(await store.GetAsync(Guid.NewGuid()), Is.Null);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public void GetAsync_RejectsEmptyExecutionIdentity()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentCampaignExecutionStore(directory);
            Assert.ThrowsAsync<ArgumentException>(async () => await store.GetAsync(Guid.Empty));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static ExperimentCampaignExecutionRecord CreateExecution()
    {
        var started = DateTimeOffset.Parse("2026-11-23T10:00:00+00:00");
        var run = new ExperimentRunRecord(
            Guid.NewGuid(),
            "thermal-lab",
            "1.0.0",
            "electrical-thermal",
            "1.2.0",
            "electrical-thermal-model",
            "2.1.0",
            1,
            "application/json",
            "{\"voltageV\":12}",
            ScientificValidationLevel.EducationalModel,
            started,
            started.AddSeconds(1),
            "application/json",
            "{\"temperatureC\":42}",
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);

        return new ExperimentCampaignExecutionRecord(
            Guid.NewGuid(),
            "thermal-campaign",
            "1.0.0",
            CreateCampaignDefinition(),
            "thermal-lab",
            "1.0.0",
            "electrical-thermal",
            "1.2.0",
            "electrical-thermal-model",
            "2.1.0",
            started,
            started.AddSeconds(2),
            ExperimentCampaignExecutionStatus.PartiallyCompleted,
            [
                new("baseline", ExperimentCampaignScenarioStatus.Completed, run),
                new("boundary", ExperimentCampaignScenarioStatus.Failed, null,
                    "System.InvalidOperationException", "The declared model range was exceeded.")
            ]);
    }

    private static ExperimentCampaignDefinition CreateCampaignDefinition() =>
        new(
            "thermal-campaign",
            "1.0.0",
            "Compare the baseline with the upper boundary.",
            "electrical-thermal",
            "1.2.0",
            "electrical-thermal-model",
            "2.1.0",
            ["Temperature remains within the declared envelope."],
            [
                new("baseline", "Baseline", ExperimentScenarioKind.Baseline, "Reference condition.",
                    new ExperimentConfiguration("electrical-thermal", "1.2.0", 1, "application/json", """{"voltageV":12}""")),
                new("boundary", "Boundary", ExperimentScenarioKind.Boundary, "Upper operating condition.",
                    new ExperimentConfiguration("electrical-thermal", "1.2.0", 1, "application/json", """{"voltageV":15}"""))
            ]);

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "singularity-laboratory-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
