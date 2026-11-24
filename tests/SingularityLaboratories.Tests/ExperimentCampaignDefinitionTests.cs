using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentCampaignDefinitionTests
{
    [Test]
    public void Constructor_PreservesBaselineAndScenarioConfigurations()
    {
        var baseline = CreateScenario("baseline", ExperimentScenarioKind.Baseline, "{\"voltage\":12}");
        var boundary = CreateScenario("boundary", ExperimentScenarioKind.Boundary, "{\"voltage\":15}");
        var criteria = new List<string> { "Temperature rise remains below the declared limit." };
        var scenarios = new List<ExperimentCampaignScenario> { baseline, boundary };

        var campaign = CreateCampaign(criteria, scenarios);

        criteria.Add("This later mutation must not enter the campaign.");
        scenarios.Clear();

        Assert.That(campaign.Scenarios, Has.Count.EqualTo(2));
        Assert.That(campaign.Scenarios[0].Configuration.Payload, Is.EqualTo("{\"voltage\":12}"));
        Assert.That(campaign.AcceptanceCriteria, Has.Count.EqualTo(1));
        Assert.That(campaign.ExperimentId, Is.EqualTo("electrical-thermal"));
        Assert.That(campaign.ModelVersion, Is.EqualTo("2.1.0"));
    }

    [Test]
    public void Constructor_RequiresExactlyOneBaseline()
    {
        var noBaseline = new[]
        {
            CreateScenario("nominal", ExperimentScenarioKind.Nominal, "{}"),
            CreateScenario("boundary", ExperimentScenarioKind.Boundary, "{}")
        };

        Assert.Throws<ArgumentException>(() => CreateCampaign(scenarios: noBaseline));

        var twoBaselines = new[]
        {
            CreateScenario("baseline-a", ExperimentScenarioKind.Baseline, "{}"),
            CreateScenario("baseline-b", ExperimentScenarioKind.Baseline, "{}")
        };

        Assert.Throws<ArgumentException>(() => CreateCampaign(scenarios: twoBaselines));
    }

    [Test]
    public void Constructor_RejectsDuplicateScenarioIdentifiers()
    {
        var scenarios = new[]
        {
            CreateScenario("same", ExperimentScenarioKind.Baseline, "{}"),
            CreateScenario("same", ExperimentScenarioKind.Sensitivity, "{}")
        };

        Assert.Throws<ArgumentException>(() => CreateCampaign(scenarios: scenarios));
    }

    [Test]
    public void Constructor_RejectsScenarioForDifferentExperimentVersion()
    {
        var scenarios = new[]
        {
            CreateScenario("baseline", ExperimentScenarioKind.Baseline, "{}"),
            new ExperimentCampaignScenario(
                "changed-version",
                "Changed experiment",
                ExperimentScenarioKind.Boundary,
                "Probe a version mismatch.",
                new ExperimentConfiguration("electrical-thermal", "2.0.0", 1, "application/json", "{}"))
        };

        Assert.Throws<ArgumentException>(() => CreateCampaign(scenarios: scenarios));
    }

    [Test]
    public void Constructor_RequiresAcceptanceCriteria()
    {
        Assert.Throws<ArgumentException>(() => CreateCampaign(criteria: []));
    }

    private static ExperimentCampaignDefinition CreateCampaign(
        IEnumerable<string>? criteria = null,
        IEnumerable<ExperimentCampaignScenario>? scenarios = null) =>
        new(
            "electrical-thermal-study",
            "1.0.0",
            "Determine how the declared load affects the thermal body's temperature.",
            "electrical-thermal",
            "1.2.0",
            "electrical-thermal-coupling",
            "2.1.0",
            criteria ?? ["Temperature rise remains below the declared limit."],
            scenarios ?? [
                CreateScenario("baseline", ExperimentScenarioKind.Baseline, "{\"voltage\":12}"),
                CreateScenario("boundary", ExperimentScenarioKind.Boundary, "{\"voltage\":15}")
            ]);

    private static ExperimentCampaignScenario CreateScenario(
        string id,
        ExperimentScenarioKind kind,
        string payload) =>
        new(
            id,
            id,
            kind,
            $"Declared rationale for {id}.",
            new ExperimentConfiguration("electrical-thermal", "1.2.0", 1, "application/json", payload));
}
