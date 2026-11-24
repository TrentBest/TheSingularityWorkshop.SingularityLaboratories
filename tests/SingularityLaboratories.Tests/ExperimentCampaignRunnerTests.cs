using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentCampaignRunnerTests
{
    [Test]
    public async Task RunAsync_ExecutesEveryScenarioAndPreservesRunLineage()
    {
        var campaign = CreateCampaign();
        var model = new StubModel(CreateDefinition());
        var laboratory = CreateLaboratory();

        var result = await new ExperimentCampaignRunner().RunAsync(
            laboratory, campaign, (_, _) => model);

        Assert.That(result.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.Completed));
        Assert.That(result.Scenarios, Has.Count.EqualTo(2));
        Assert.That(result.Scenarios.All(scenario =>
            scenario.Status == ExperimentCampaignScenarioStatus.Completed), Is.True);
        Assert.That(result.Scenarios.Select(scenario => scenario.Run!.RunId).Distinct().Count(), Is.EqualTo(2));
        Assert.That(result.Scenarios.All(scenario =>
            scenario.Run!.ExperimentId == campaign.ExperimentId &&
            scenario.Run.ExperimentVersion == campaign.ExperimentVersion &&
            scenario.Run.ModelId == campaign.ModelId &&
            scenario.Run.ModelVersion == campaign.ModelVersion), Is.True);
        Assert.That(model.InvocationCount, Is.EqualTo(2));
    }

    [Test]
    public async Task RunAsync_ContinuesAfterScenarioFailureAndReturnsPartialOutcome()
    {
        var campaign = CreateCampaign(
        [
            CreateScenario("baseline", ExperimentScenarioKind.Baseline, "{\"voltageV\":12}"),
            CreateScenario("unsupported", ExperimentScenarioKind.Boundary, "{\"voltageV\":99}"),
            CreateScenario("sensitivity", ExperimentScenarioKind.Sensitivity, "{\"voltageV\":14}")
        ]);
        var model = new StubModel(CreateDefinition(), payload =>
            payload.Contains("\"voltageV\":99", StringComparison.Ordinal)
                ? throw new InvalidOperationException("The model does not support this condition.")
                : new ExperimentModelOutput("application/json", "{\"temperatureC\":42}"));

        var result = await new ExperimentCampaignRunner().RunAsync(
            CreateLaboratory(), campaign, (_, _) => model);

        Assert.That(result.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.PartiallyCompleted));
        Assert.That(result.Scenarios.Select(scenario => scenario.Status), Is.EqualTo([
            ExperimentCampaignScenarioStatus.Completed,
            ExperimentCampaignScenarioStatus.Failed,
            ExperimentCampaignScenarioStatus.Completed
        ]));
        Assert.That(result.Scenarios[1].FailureType, Does.Contain(nameof(InvalidOperationException)));
        Assert.That(result.Scenarios[1].FailureMessage, Does.Contain("does not support"));
        Assert.That(result.Scenarios[1].Run, Is.Null);
        Assert.That(model.InvocationCount, Is.EqualTo(3));
    }

    [Test]
    public async Task RunAsync_RejectsResolverReturningDifferentModelVersion()
    {
        var campaign = CreateCampaign();
        var wrongDefinition = new ExperimentDefinition(
            campaign.ExperimentId,
            campaign.ExperimentVersion,
            "Different model",
            campaign.ModelId,
            "9.9.9",
            ScientificValidationLevel.EducationalModel,
            ["Declared assumption."],
            ["Declared limitation."]);
        var model = new StubModel(wrongDefinition);

        var result = await new ExperimentCampaignRunner().RunAsync(
            CreateLaboratory(), campaign, (_, _) => model);

        Assert.That(result.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.Failed));
        Assert.That(result.Scenarios.All(scenario =>
            scenario.Status == ExperimentCampaignScenarioStatus.Failed), Is.True);
        Assert.That(result.Scenarios.All(scenario =>
            scenario.FailureMessage!.Contains("exact experiment and model versions", StringComparison.Ordinal)), Is.True);
        Assert.That(model.InvocationCount, Is.Zero);
    }

    [Test]
    public async Task RunAsync_RecordsCancellationAndLeavesRemainingScenariosExplicitlyNotRun()
    {
        using var cancellation = new CancellationTokenSource();
        var campaign = CreateCampaign(
        [
            CreateScenario("baseline", ExperimentScenarioKind.Baseline, "{\"voltageV\":12}"),
            CreateScenario("boundary", ExperimentScenarioKind.Boundary, "{\"voltageV\":15}"),
            CreateScenario("sensitivity", ExperimentScenarioKind.Sensitivity, "{\"voltageV\":14}")
        ]);
        var model = new StubModel(CreateDefinition(), _ =>
        {
            cancellation.Cancel();
            return new ExperimentModelOutput("application/json", "{}");
        });

        var result = await new ExperimentCampaignRunner().RunAsync(
            CreateLaboratory(), campaign, (_, _) => model, cancellation.Token);

        Assert.That(result.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.Cancelled));
        Assert.That(result.Scenarios.Select(scenario => scenario.Status), Is.EqualTo([
            ExperimentCampaignScenarioStatus.Cancelled,
            ExperimentCampaignScenarioStatus.NotRun,
            ExperimentCampaignScenarioStatus.NotRun
        ]));
        Assert.That(result.Scenarios.All(scenario => scenario.Run is null), Is.True);
        Assert.That(model.InvocationCount, Is.EqualTo(1));
    }

    [Test]
    public async Task RunAsync_AppliesHostAdmissionPolicyToEveryScenario()
    {
        var campaign = CreateCampaign();
        var model = new StubModel(CreateDefinition());
        var policy = new ExperimentExecutionPolicy(requireAcceptedValidationAssessment: true);

        var result = await new ExperimentCampaignRunner().RunAsync(
            CreateLaboratory(), campaign, (_, _) => model, executionPolicy: policy);

        Assert.That(result.Status, Is.EqualTo(ExperimentCampaignExecutionStatus.Failed));
        Assert.That(result.Scenarios.All(scenario =>
            scenario.FailureMessage!.Contains("admission", StringComparison.OrdinalIgnoreCase) ||
            scenario.FailureMessage!.Contains("validation", StringComparison.OrdinalIgnoreCase)), Is.True);
        Assert.That(model.InvocationCount, Is.Zero);
    }

    private static LaboratoryDefinition CreateLaboratory() =>
        new("thermal-lab", "1.0.0", "Thermal Lab", "Controlled thermal experiment environment.");

    private static ExperimentDefinition CreateDefinition() => new(
        "electrical-thermal",
        "1.2.0",
        "Electrical thermal response",
        "electrical-thermal-coupling",
        "2.1.0",
        ScientificValidationLevel.EducationalModel,
        ["Ideal fixed-voltage DC source."],
        ["The fixture does not predict spatial temperature gradients."]);

    private static ExperimentCampaignDefinition CreateCampaign(
        IReadOnlyList<ExperimentCampaignScenario>? scenarios = null) =>
        new(
            "thermal-campaign",
            "1.0.0",
            "Explore the declared voltage response.",
            "electrical-thermal",
            "1.2.0",
            "electrical-thermal-coupling",
            "2.1.0",
            ["Temperature remains within the declared operating envelope."],
            scenarios ?? [
                CreateScenario("baseline", ExperimentScenarioKind.Baseline, "{\"voltageV\":12}"),
                CreateScenario("boundary", ExperimentScenarioKind.Boundary, "{\"voltageV\":15}")
            ]);

    private static ExperimentCampaignScenario CreateScenario(
        string id,
        ExperimentScenarioKind kind,
        string payload) =>
        new(id, id, kind, $"Probe {id}.",
            new ExperimentConfiguration("electrical-thermal", "1.2.0", 1, "application/json", payload));

    private sealed class StubModel : IExperimentModel
    {
        private readonly Func<string, ExperimentModelOutput> _execute;

        public StubModel(
            ExperimentDefinition definition,
            Func<string, ExperimentModelOutput>? execute = null)
        {
            Definition = definition;
            ParameterSchema = new ExperimentParameterSchema(
                definition.Id,
                definition.Version,
                1,
                [new("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number, unitSymbol: "V")]);
            _execute = execute ?? (_ => new ExperimentModelOutput("application/json", "{\"temperatureC\":42}"));
        }

        public ExperimentDefinition Definition { get; }
        public ExperimentParameterSchema ParameterSchema { get; }
        public int InvocationCount { get; private set; }

        public ValueTask<ExperimentModelOutput> ExecuteAsync(
            ExperimentConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            InvocationCount++;
            return ValueTask.FromResult(_execute(configuration.Payload));
        }
    }
}
