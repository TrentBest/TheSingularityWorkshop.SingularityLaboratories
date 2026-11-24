using TheSingularityWorkshop.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.ExecutionAdapter;

/// <summary>Resolves laboratory model capabilities for the host-approved FSM_Execution handler.</summary>
public interface IExperimentModelCatalog
{
    bool TryResolve(string experimentId, string version, out IExperimentModel? model);
}

/// <summary>
/// Registers laboratory experiment invocation with FSM_Execution. FSM_Execution retains ownership
/// of workflow validation, accepted/running/terminal states, cancellation, and timeouts.
/// </summary>
public static class LaboratoryWorkflowAdapter
{
    public const string HandlerKey = "laboratory.execute";
    public const string ConfigurationInputKey = "laboratory.configuration";
    public const string RunRecordOutputKey = "laboratory.runRecord";

    public const string CampaignHandlerKey = "laboratory.executeCampaign";
    public const string CampaignDefinitionInputKey = "laboratory.campaign";
    public const string CampaignExecutionOutputKey = "laboratory.campaignExecution";

    /// <summary>
    /// Registers version-bound campaign execution with FSM_Execution and atomically persists the
    /// complete campaign outcome. Successful scenario run records remain embedded in that outcome.
    /// </summary>
    public static void RegisterCampaign(
        WorkflowHandlerRegistry handlers,
        LaboratoryDefinition laboratory,
        IExperimentModelCatalog models,
        IExperimentCampaignExecutionStore campaignStore,
        ExperimentExecutionPolicy? executionPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        ArgumentNullException.ThrowIfNull(laboratory);
        ArgumentNullException.ThrowIfNull(models);
        ArgumentNullException.ThrowIfNull(campaignStore);

        handlers.Register(CampaignHandlerKey, async (context, cancellationToken) =>
        {
            var campaign = context.GetInput<ExperimentCampaignDefinition>(CampaignDefinitionInputKey);
            if (campaign is null)
                throw new InvalidOperationException("The workflow did not supply an experiment campaign.");

            var execution = await new ExperimentCampaignRunner().RunAsync(
                laboratory,
                campaign,
                (experimentId, version) =>
                    models.TryResolve(experimentId, version, out var model) ? model : null,
                cancellationToken,
                executionPolicy).ConfigureAwait(false);

            await campaignStore.SaveAsync(execution, cancellationToken).ConfigureAwait(false);
            context.SetOutput(CampaignExecutionOutputKey, execution);
        });
    }

    public static void Register(
        WorkflowHandlerRegistry handlers,
        LaboratoryDefinition laboratory,
        IExperimentModelCatalog models,
        IExperimentRunStore? runStore = null)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        ArgumentNullException.ThrowIfNull(laboratory);
        ArgumentNullException.ThrowIfNull(models);

        handlers.Register(HandlerKey, async (context, cancellationToken) =>
        {
            var configuration = context.GetInput<ExperimentConfiguration>(ConfigurationInputKey);
            if (configuration is null)
                throw new InvalidOperationException("The workflow did not supply an experiment configuration.");

            if (!models.TryResolve(configuration.ExperimentId, configuration.ExperimentVersion, out var model) ||
                model is null)
                throw new InvalidOperationException("The requested experiment model and version are unavailable.");

            var run = await new LaboratoryExperimentRunner().RunAsync(
                laboratory, model, configuration, cancellationToken).ConfigureAwait(false);
            if (runStore is not null)
                await runStore.SaveAsync(run, cancellationToken).ConfigureAwait(false);

            context.SetOutput(RunRecordOutputKey, run);
        });
    }
}
