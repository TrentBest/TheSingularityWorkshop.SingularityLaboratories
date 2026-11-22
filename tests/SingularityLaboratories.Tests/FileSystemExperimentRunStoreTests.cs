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
            Assert.That(actual!.RunId, Is.EqualTo(expected.RunId));
            Assert.That(actual.ConfigurationPayload, Is.EqualTo(expected.ConfigurationPayload));
            Assert.That(actual.ResultPayload, Is.EqualTo(expected.ResultPayload));
            Assert.That(actual.ModelId, Is.EqualTo(expected.ModelId));
            Assert.That(actual.ModelVersion, Is.EqualTo(expected.ModelVersion));
            Assert.That(actual.Assumptions, Is.EqualTo(expected.Assumptions));
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
            var persisted = await store.GetAsync(run.RunId);
            Assert.That(persisted!.RunId, Is.EqualTo(run.RunId));
            Assert.That(persisted.ResultPayload, Is.EqualTo(run.ResultPayload));
            Assert.That(persisted.Assumptions, Is.EqualTo(run.Assumptions));
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


    [Test]
    public async Task ReplayAsync_ReexecutesStoredConfigurationAndPersistsSourceLineage()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentRunStore(directory);
            var source = CreateRun();
            await store.SaveAsync(source);
            var model = CreateModel();

            var replay = await new ExperimentRunReplayService(store).ReplayAsync(
                new LaboratoryDefinition("thermal-lab", "1.0.0", "Thermal Lab", "Energy transfer study."),
                source.RunId,
                (experimentId, version) =>
                    experimentId == model.Definition.Id && version == model.Definition.Version ? model : null);

            Assert.That(replay.RunId, Is.Not.EqualTo(source.RunId));
            Assert.That(replay.ReplayedFromRunId, Is.EqualTo(source.RunId));
            Assert.That(replay.ConfigurationPayload, Is.EqualTo(source.ConfigurationPayload));
            Assert.That(replay.ModelId, Is.EqualTo(source.ModelId));
            Assert.That(replay.ModelVersion, Is.EqualTo(source.ModelVersion));
            Assert.That(replay.ResultPayload, Is.EqualTo("{\\"energyJ\\":240}"));
            Assert.That(model.InvocationCount, Is.EqualTo(1));

            var persistedReplay = await store.GetAsync(replay.RunId);
            Assert.That(persistedReplay!.ReplayedFromRunId, Is.EqualTo(source.RunId));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Test]
    public async Task ReplayAsync_RejectsDifferentModelVersionBeforeExecution()
    {
        var directory = CreateTemporaryDirectory();
        try
        {
            var store = new FileSystemExperimentRunStore(directory);
            var source = CreateRun();
            await store.SaveAsync(source);
            var model = CreateModel(modelVersion: "0.2.0");

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await new ExperimentRunReplayService(store).ReplayAsync(
                    new LaboratoryDefinition("thermal-lab", "1.0.0", "Thermal Lab", "Energy transfer study."),
                    source.RunId,
                    (_, _) => model));
            Assert.That(model.InvocationCount, Is.Zero);
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


    private static StubModel CreateModel(string modelVersion = "0.1.0")
    {
        var definition = new ExperimentDefinition(
            "electrical-thermal-coupling",
            "1.0.0",
            "Electrical to thermal coupling",
            "electrical-thermal-model",
            modelVersion,
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage DC bus."],
            ["No spatial temperature gradients."]);
        return new StubModel(definition);
    }

    private sealed class StubModel(ExperimentDefinition definition) : TheSingularityWorkshop.SingularityLaboratories.Execution.IExperimentModel
    {
        public ExperimentDefinition Definition { get; } = definition;
        public ExperimentParameterSchema ParameterSchema { get; } = new(
            definition.Id,
            definition.Version,
            1,
            [new("voltageV", "Voltage", "Input voltage.", ExperimentParameterType.Number, unitSymbol: "V")]);
        public int InvocationCount { get; private set; }

        public ValueTask<TheSingularityWorkshop.SingularityLaboratories.Execution.ExperimentModelOutput> ExecuteAsync(
            ExperimentConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            InvocationCount++;
            return ValueTask.FromResult(new TheSingularityWorkshop.SingularityLaboratories.Execution.ExperimentModelOutput(
                "application/json", "{\\"energyJ\\":240}"));
        }
    }

    private static string CreateTemporaryDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), "singularity-laboratory-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(path);
        return path;
    }
}
