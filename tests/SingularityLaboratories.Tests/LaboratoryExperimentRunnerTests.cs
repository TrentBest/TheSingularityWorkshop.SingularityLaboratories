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
    public void RunAsync_RejectsUnknownParametersBeforeInvokingModel()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12,\"unexpected\":true}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RejectsWrongParameterTypeBeforeInvokingModel()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":\"twelve\"}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RejectsMismatchedParameterSchemaVersionBeforeInvokingModel()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 2, "application/json", "{\"voltageV\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_HonorsCancellationBeforeInvokingModel()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration, cancellation.Token));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RejectsResourceEstimateOverBudgetBeforeInvokingModel()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        var profile = new ExperimentResourceProfile(definition.ModelId, definition.ModelVersion, TimeSpan.FromSeconds(5), 1024 * 1024, 100);
        var budget = new ExecutionResourceBudget(TimeSpan.FromSeconds(2), 2 * 1024 * 1024, 1000);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration,
                resourceProfile: profile, resourceBudget: budget));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RejectsResourceProfileForDifferentModelVersion()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        var profile = new ExperimentResourceProfile(definition.ModelId, "different-version", TimeSpan.FromSeconds(1), 1024, 10);
        var budget = new ExecutionResourceBudget(TimeSpan.FromSeconds(2), 2048, 20);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration,
                resourceProfile: profile, resourceBudget: budget));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RequiresResourceProfileAndBudgetTogether()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\"voltageV\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        var profile = new ExperimentResourceProfile(definition.ModelId, definition.ModelVersion, TimeSpan.FromSeconds(1), 1024, 10);

        Assert.ThrowsAsync<ArgumentException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration,
                resourceProfile: profile));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RequiresAcceptedAssessmentWhenHostPolicyDemandsIt()
    {
        var definition = CreateDefinition();
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\\"voltageV\\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        var policy = new ExperimentExecutionPolicy(requireAcceptedValidationAssessment: true);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration,
                executionPolicy: policy));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public void RunAsync_RejectsRiskAboveHostCeilingBeforeInvokingModel()
    {
        var baseDefinition = CreateDefinition();
        var safety = new ExperimentSafetyMetadata(ExperimentRiskLevel.High,
            ["High temperature surfaces."], ["Use thermal protection."], ["Unsupervised operation."]);
        var definition = new ExperimentDefinition(baseDefinition.Id, baseDefinition.Version, baseDefinition.Name,
            baseDefinition.ModelId, baseDefinition.ModelVersion, baseDefinition.ValidationLevel,
            baseDefinition.Assumptions, baseDefinition.Limitations, safety);
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\\"voltageV\\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        var policy = new ExperimentExecutionPolicy(maximumRiskLevel: ExperimentRiskLevel.Moderate);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration,
                executionPolicy: policy));
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public async Task RunAsync_AllowsDefinitionMeetingHostAdmissionPolicy()
    {
        var baseDefinition = CreateDefinition();
        var safety = new ExperimentSafetyMetadata(ExperimentRiskLevel.Low,
            ["No known hazards under stated conditions."], ["Follow the published procedure."],
            ["Use outside stated conditions."]);
        var evidence = new ExperimentValidationEvidence("review-1", "Reviewed model test evidence.",
            new DateTimeOffset(2026, 11, 22, 18, 0, 0, TimeSpan.Zero));
        var assessment = new ExperimentValidationAssessment(baseDefinition.Id, baseDefinition.Version,
            baseDefinition.ModelId, baseDefinition.ModelVersion, ExperimentValidationState.Accepted,
            new DateTimeOffset(2026, 11, 22, 18, 1, 0, TimeSpan.Zero), [evidence], "Host reviewer");
        var definition = new ExperimentDefinition(baseDefinition.Id, baseDefinition.Version, baseDefinition.Name,
            baseDefinition.ModelId, baseDefinition.ModelVersion, baseDefinition.ValidationLevel,
            baseDefinition.Assumptions, baseDefinition.Limitations, safety, assessment);
        var model = new StubModel(definition, new ExperimentModelOutput("application/json", "{}"));
        var configuration = new ExperimentConfiguration(definition.Id, definition.Version, 1, "application/json", "{\\"voltageV\\":12}");
        var laboratory = new LaboratoryDefinition("lab", "1.0.0", "Lab", "Test lab.");
        var policy = new ExperimentExecutionPolicy(true, ExperimentRiskLevel.Moderate);

        await new LaboratoryExperimentRunner().RunAsync(laboratory, model, configuration,
            executionPolicy: policy);

        Assert.That(model.InvocationCount, Is.EqualTo(1));
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
            InvocationCount++;
            return ValueTask.FromResult(output);
        }
    }
}
