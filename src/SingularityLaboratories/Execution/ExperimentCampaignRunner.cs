using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>
/// Executes every scenario in a campaign through the ordinary laboratory runner, preserving
/// successful run records and explicit failures without allowing one failed scenario to hide the rest.
/// </summary>
public sealed class ExperimentCampaignRunner
{
    private readonly LaboratoryExperimentRunner _runner;

    public ExperimentCampaignRunner(LaboratoryExperimentRunner? runner = null)
    {
        _runner = runner ?? new LaboratoryExperimentRunner();
    }

    /// <summary>
    /// Executes a campaign against the exact experiment and model versions declared by its
    /// definition. The resolver is called for each scenario so hosts can enforce their own
    /// version-availability and admission policies.
    /// </summary>
    public async ValueTask<ExperimentCampaignExecutionRecord> RunAsync(
        LaboratoryDefinition laboratory,
        ExperimentCampaignDefinition campaign,
        Func<string, string, IExperimentModel?> modelResolver,
        CancellationToken cancellationToken = default,
        ExperimentExecutionPolicy? executionPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(laboratory);
        ArgumentNullException.ThrowIfNull(campaign);
        ArgumentNullException.ThrowIfNull(modelResolver);

        var startedAtUtc = DateTimeOffset.UtcNow;
        var results = new List<ExperimentCampaignScenarioResult>(campaign.Scenarios.Count);

        for (var index = 0; index < campaign.Scenarios.Count; index++)
        {
            var scenario = campaign.Scenarios[index];
            if (cancellationToken.IsCancellationRequested)
            {
                results.Add(new ExperimentCampaignScenarioResult(
                    scenario.Id, ExperimentCampaignScenarioStatus.Cancelled, null));
                AddNotRunResults(campaign, results, index + 1);
                break;
            }

            try
            {
                var model = modelResolver(campaign.ExperimentId, campaign.ExperimentVersion)
                    ?? throw new InvalidOperationException("The exact experiment version is unavailable.");
                var definition = model.Definition
                    ?? throw new InvalidOperationException("The resolved model has no experiment definition.");

                if (!string.Equals(definition.Id, campaign.ExperimentId, StringComparison.Ordinal) ||
                    !string.Equals(definition.Version, campaign.ExperimentVersion, StringComparison.Ordinal) ||
                    !string.Equals(definition.ModelId, campaign.ModelId, StringComparison.Ordinal) ||
                    !string.Equals(definition.ModelVersion, campaign.ModelVersion, StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        "The resolved model does not match the campaign's exact experiment and model versions.");
                }

                var run = await _runner.RunAsync(
                    laboratory,
                    model,
                    scenario.Configuration,
                    cancellationToken,
                    executionPolicy: executionPolicy).ConfigureAwait(false);

                results.Add(new ExperimentCampaignScenarioResult(
                    scenario.Id, ExperimentCampaignScenarioStatus.Completed, run));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                results.Add(new ExperimentCampaignScenarioResult(
                    scenario.Id, ExperimentCampaignScenarioStatus.Cancelled, null));
                AddNotRunResults(campaign, results, index + 1);
                break;
            }
            catch (Exception exception)
            {
                results.Add(new ExperimentCampaignScenarioResult(
                    scenario.Id,
                    ExperimentCampaignScenarioStatus.Failed,
                    null,
                    exception.GetType().FullName ?? exception.GetType().Name,
                    exception.Message));
            }
        }

        var completedAtUtc = DateTimeOffset.UtcNow;
        var completedCount = results.Count(result =>
            result.Status == ExperimentCampaignScenarioStatus.Completed);
        var failedCount = results.Count(result =>
            result.Status == ExperimentCampaignScenarioStatus.Failed);
        var cancelledCount = results.Count(result =>
            result.Status == ExperimentCampaignScenarioStatus.Cancelled);
        var notRunCount = results.Count(result =>
            result.Status == ExperimentCampaignScenarioStatus.NotRun);

        var status = cancelledCount > 0
            ? ExperimentCampaignExecutionStatus.Cancelled
            : failedCount == 0 && notRunCount == 0
                ? ExperimentCampaignExecutionStatus.Completed
                : completedCount > 0
                    ? ExperimentCampaignExecutionStatus.PartiallyCompleted
                    : ExperimentCampaignExecutionStatus.Failed;

        return new ExperimentCampaignExecutionRecord(
            Guid.NewGuid(),
            campaign.Id,
            campaign.Version,
            laboratory.Id,
            laboratory.Version,
            campaign.ExperimentId,
            campaign.ExperimentVersion,
            campaign.ModelId,
            campaign.ModelVersion,
            startedAtUtc,
            completedAtUtc,
            status,
            results);
    }

    private static void AddNotRunResults(
        ExperimentCampaignDefinition campaign,
        ICollection<ExperimentCampaignScenarioResult> results,
        int startIndex)
    {
        for (var index = startIndex; index < campaign.Scenarios.Count; index++)
        {
            results.Add(new ExperimentCampaignScenarioResult(
                campaign.Scenarios[index].Id,
                ExperimentCampaignScenarioStatus.NotRun,
                null));
        }
    }
}
