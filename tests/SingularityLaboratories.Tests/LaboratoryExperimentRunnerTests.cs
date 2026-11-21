using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryExperimentRunnerTests
{
    [Test]
    public async Task RunAsync_CapturesExactConfigurationModelAndScientificDisclosures()
    {
        var laboratory = new LaboratoryDefinition("thermal-lab", "1.0.0", "Thermal Lab", "Energy transfer study.");
        var definition = CreateDefinition();
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}");
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{\"energyJ\":240}"));

        var run = await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration);

        Assert.That(run.RunId, Is.Not.EqualTo(Guid.Empty));
        Assert.That(run.LaboratoryId, Is.EqualTo(laboratory.Id));
        Assert.That(run.LaboratoryVersion, Is.EqualTo(laboratory.Version));
        Assert.That(run.ExperimentId, Is.EqualTo(definition.Id));
        Assert.That(run.ExperimentVersion, Is.EqualTo(definition.Version));
        Assert.That(run.ModelId, Is.EqualTo(definition.ModelId));
        Assert.That(run.ModelVersion, Is.EqualTo(definition.ModelVersion));
        Assert.That(run.ConfigurationSchemaVersion, Is.EqualTo(1));
        Assert.That(run.ConfigurationPayload, Is.EqualTo(configuration.Payload));
        Assert.That(run.ResultPayload, Is.EqualTo("{\"energyJ\":240}"));
        Assert.That(run.ValidationLevel, Is.EqualTo(ScientificValidationLevel.EducationalModel));
        Assert.That(run.Assumptions, Is.EquivalentTo(definition.Assumptions));
        Assert.That(run.Limitations, Is.EquivalentTo(definition.Limitations));
        Assert.That(run.StartedAtUtc.Offset, Is.EqualTo(TimeSpan.Zero));
        Assert.That(run.CompletedAtUtc, Is.GreaterThanOrEqualTo(run.StartedAtUtc));
    }

    [Test]
    public void RunAsync_RejectsConfigurationForDifferentExperimentVersion()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, "2.0.0", 1, "application/json", "{}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration));
    }

    [Test]
    public void RunAsync_HonorsCancellationBeforeInvokingModel()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration, cancellation.Token));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    private static ExperimentDefinition CreateDefinition() => new(
        "electrical-thermal-coupling",
        "1.0.0",
        "Electrical to thermal coupling",
        "TheSingularityWorkshop.Physics.ElectricalThermalCouplingExperiment",
        "0.1.0-alpha.1",
        ScientificValidationLevel.EducationalModel,
        ["Ideal fixed-voltage DC bus.", "Constant resistance and constant specific heat."],
        ["No spatial temperature gradients.", "Absorbed fraction is a declared input, not a predicted efficiency."]);

    private sealed class StubModel(
        ExperimentDefinition definition,
        ExperimentModelOutput output) : IExperimentModel
    {
        public ExperimentDefinition Definition { get; } = definition;
        public int InvocationCount { get; private set; }

        public ValueTask<ExperimentModelOutput> ExecuteAsync(
            ExperimentConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            InvocationCount++;
            return ValueTask.FromResult(output);
        }
    }
}
