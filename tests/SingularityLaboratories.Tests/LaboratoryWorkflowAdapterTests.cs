using System.Text.Json;
using NUnit.Framework;
using TheSingularityWorkshop.Foundry;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Foundry;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryWorkflowAdapterTests
{
    [Test]
    public async Task FoundryWorkflow_ExecutesModelAndRetainsReproducibleRunRecord()
    {
        var laboratory = new LaboratoryDefinition("thermal-lab", "1.0.0", "Thermal Lab", "Energy transfer study.");
        var definition = new ExperimentDefinition(
            "electrical-thermal-coupling", "1.0.0", "Electrical to thermal coupling",
            "physics.electrical-thermal-coupling", "0.1.0-alpha.1",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage DC bus."],
            ["No spatial gradients; absorbed fraction is a declared input."]);
        var configuration = new ExperimentConfiguration(
            definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}");
        var model = new StubModel(definition);
        var registry = new WorkflowHandlerRegistry();
        LaboratoryWorkflowAdapter.Register(registry, laboratory, new StubCatalog(model));
        await using var coordinator = new WorkflowExecutionCoordinator(registry);

        var json = JsonSerializer.SerializeToElement(configuration);
        var context = new WorkflowExecutionContext(new Dictionary<string, JsonElement>
        {
            [LaboratoryWorkflowAdapter.ConfigurationInputKey] = json
        });
        var workflow = new WorkflowDefinition(
            "laboratory.run.v1", "Run laboratory experiment", 1,
            [new WorkflowStep("run-experiment", LaboratoryWorkflowAdapter.HandlerKey)]);
        var accepted = coordinator.Submit(workflow, context);

        Assert.That(accepted.Status, Is.EqualTo(WorkflowRunStatus.Accepted));
        var result = await coordinator.StartAsync(accepted.RunId);

        Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Succeeded));
        Assert.That(coordinator.GetSnapshot(accepted.RunId).Status, Is.EqualTo(WorkflowRunStatus.Succeeded));
        Assert.That(model.InvocationCount, Is.EqualTo(1));
        var run = context.GetOutput<ExperimentRunRecord>(LaboratoryWorkflowAdapter.RunRecordOutputKey);
        Assert.That(run, Is.Not.Null);
        Assert.That(run!.LaboratoryId, Is.EqualTo(laboratory.Id));
        Assert.That(run.ExperimentId, Is.EqualTo(definition.Id));
        Assert.That(run.ConfigurationPayload, Is.EqualTo(configuration.Payload));
        Assert.That(run.ResultPayload, Is.EqualTo("{\"thermalEnergyJ\":120}"));
    }

    private sealed class StubCatalog(IExperimentModel model) : IExperimentModelCatalog
    {
        public bool TryResolve(string experimentId, string version, out IExperimentModel? resolved)
        {
            resolved = model;
            return string.Equals(model.Definition.Id, experimentId, StringComparison.Ordinal) &&
                   string.Equals(model.Definition.Version, version, StringComparison.Ordinal);
        }
    }

    private sealed class StubModel(ExperimentDefinition definition) : IExperimentModel
    {
        public ExperimentDefinition Definition { get; } = definition;
        public int InvocationCount { get; private set; }

        public ValueTask<ExperimentModelOutput> ExecuteAsync(
            ExperimentConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            InvocationCount++;
            return ValueTask.FromResult(new ExperimentModelOutput(
                "application/json", "{\"thermalEnergyJ\":120}"));
        }
    }
}
