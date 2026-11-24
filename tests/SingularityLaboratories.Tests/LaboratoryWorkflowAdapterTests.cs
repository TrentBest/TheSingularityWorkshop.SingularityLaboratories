using System.Text.Json;
using NUnit.Framework;
using TheSingularityWorkshop.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;
using TheSingularityWorkshop.SingularityLaboratories.ExecutionAdapter;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryWorkflowAdapterTests
{
    [Test]
    public async Task ExecutionWorkflow_ExecutesModelAndRetainsReproducibleRunRecord()
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

    [Test]
    public async Task ExecutionWorkflow_RunsRealPhysicsAdapterAndStoresScientificObservation()
    {
        var laboratory = new LaboratoryDefinition("thermal-lab", "1.0.0", "Thermal Lab", "Energy transfer study.");
        var model = new TheSingularityWorkshop.SingularityLaboratories.Physics.ElectricalThermalCouplingModel();
        var input = new TheSingularityWorkshop.SingularityLaboratories.Physics.ElectricalThermalCouplingConfiguration
        {
            BusVoltageVolts = 12d,
            ResistanceOhms = 6d,
            GenerationCapacityWatts = 100d,
            DurationSeconds = 10d,
            AbsorbedPowerFraction = 0.5d,
            InitialMassKilograms = 2d,
            InitialTemperatureKelvin = 300d,
            SpecificHeatCapacityJoulesPerKilogramKelvin = 500d,
            ReferenceTemperatureKelvin = 273.15d
        };
        var configuration = new ExperimentConfiguration(
            model.Definition.Id, model.Definition.Version, 1,
            "application/json", JsonSerializer.Serialize(input));
        var registry = new WorkflowHandlerRegistry();
        LaboratoryWorkflowAdapter.Register(registry, laboratory, new SingleModelCatalog(model));
        await using var coordinator = new WorkflowExecutionCoordinator(registry);
        var context = new WorkflowExecutionContext(new Dictionary<string, JsonElement>
        {
            [LaboratoryWorkflowAdapter.ConfigurationInputKey] = JsonSerializer.SerializeToElement(configuration)
        });
        var workflow = new WorkflowDefinition(
            "laboratory.physics.run.v1", "Run Physics-backed experiment", 1,
            [new WorkflowStep("run-physics-experiment", LaboratoryWorkflowAdapter.HandlerKey)]);
        var accepted = coordinator.Submit(workflow, context);

        var result = await coordinator.StartAsync(accepted.RunId);

        Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Succeeded));
        var run = context.GetOutput<ExperimentRunRecord>(LaboratoryWorkflowAdapter.RunRecordOutputKey);
        Assert.That(run, Is.Not.Null);
        Assert.That(run!.ModelId, Is.EqualTo(TheSingularityWorkshop.SingularityLaboratories.Physics.ElectricalThermalCouplingModel.PhysicsModelId));
        Assert.That(run.ValidationLevel, Is.EqualTo(ScientificValidationLevel.EducationalModel));
        var observation = JsonSerializer.Deserialize<TheSingularityWorkshop.SingularityLaboratories.Physics.ElectricalThermalCouplingObservation>(run.ResultPayload);
        Assert.That(observation, Is.Not.Null);
        Assert.That(observation!.ElectricalEnergyJoules, Is.EqualTo(240d).Within(1e-12));
        Assert.That(observation.ThermalEnergyTransferJoules, Is.EqualTo(120d).Within(1e-12));
        Assert.That(observation.FinalThermalState.TemperatureKelvin, Is.EqualTo(300.12d).Within(1e-10));
    }

    [Test]
    public async Task CampaignWorkflow_ExecutesAndPersistsCompleteScenarioLineage()
    {
        var directory = Path.Combine(Path.GetTempPath(), "singularity-campaign-workflow", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var laboratory = new LaboratoryDefinition("thermal-lab", "1.0.0", "Thermal Lab", "Energy transfer study.");
            var definition = new ExperimentDefinition(
                "electrical-thermal-coupling", "1.0.0", "Electrical to thermal coupling",
                "physics.electrical-thermal-coupling", "0.1.0-alpha.1",
                ScientificValidationLevel.EducationalModel,
                ["Ideal fixed-voltage DC bus."],
                ["No spatial gradients; absorbed fraction is a declared input."]);
            var campaign = new ExperimentCampaignDefinition(
                "thermal-campaign", "1.0.0", "Compare nominal and boundary conditions.",
                definition.Id, definition.Version, definition.ModelId, definition.ModelVersion,
                ["Temperature remains within the declared operating envelope."],
                [
                    new("baseline", "Baseline", ExperimentScenarioKind.Baseline, "Reference condition.",
                        new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}")),
                    new("boundary", "Boundary", ExperimentScenarioKind.Boundary, "Probe the upper declared condition.",
                        new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":15}"))
                ]);
            var model = new StubModel(definition);
            var registry = new WorkflowHandlerRegistry();
            var store = new FileSystemExperimentCampaignExecutionStore(directory);
            LaboratoryWorkflowAdapter.RegisterCampaign(registry, laboratory, new StubCatalog(model), store);
            await using var coordinator = new WorkflowExecutionCoordinator(registry);

            var context = new WorkflowExecutionContext(new Dictionary<string, JsonElement>
            {
                [LaboratoryWorkflowAdapter.CampaignDefinitionInputKey] = JsonSerializer.SerializeToElement(campaign)
            });
            var workflow = new WorkflowDefinition(
                "laboratory.campaign.v1", "Execute laboratory campaign", 1,
                [new WorkflowStep("run-campaign", LaboratoryWorkflowAdapter.CampaignHandlerKey)]);
            var accepted = coordinator.Submit(workflow, context);
            var result = await coordinator.StartAsync(accepted.RunId);

            Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Succeeded));
            var execution = context.GetOutput<ExperimentCampaignExecutionRecord>(
                LaboratoryWorkflowAdapter.CampaignExecutionOutputKey);
            Assert.That(execution, Is.Not.Null);
            Assert.That(execution!.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.Completed));
            Assert.That(execution.Scenarios, Has.Count.EqualTo(2));
            Assert.That(execution.Scenarios.All(scenario => scenario.Run is not null), Is.True);
            Assert.That(model.InvocationCount, Is.EqualTo(2));

            var persisted = await store.GetAsync(execution.ExecutionId);
            Assert.That(persisted, Is.Not.Null);
            Assert.That(persisted!.Scenarios.Select(scenario => scenario.Run!.RunId),
                Is.EqualTo(execution.Scenarios.Select(scenario => scenario.Run!.RunId)));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class SingleModelCatalog(IExperimentModel model) : IExperimentModelCatalog
    {
        public bool TryResolve(string experimentId, string version, out IExperimentModel? resolved)
        {
            resolved = model;
            return string.Equals(model.Definition.Id, experimentId, StringComparison.Ordinal) &&
                   string.Equals(model.Definition.Version, version, StringComparison.Ordinal);
        }
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
        public ExperimentParameterSchema ParameterSchema { get; } = new(
            definition.Id,
            definition.Version,
            1,
            [new("voltageV", "Voltage", "Input voltage.", ExperimentParameterType.Number, unitSymbol: "V")]);
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
