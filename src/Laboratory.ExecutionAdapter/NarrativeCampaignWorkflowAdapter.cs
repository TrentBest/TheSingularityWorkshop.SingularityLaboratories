using TheSingularityWorkshop.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.ExecutionAdapter;

/// <summary>
/// Registers fixed-opening narrative campaign execution with FSM_Execution. Scientific experiment
/// campaigns continue to use ExperimentCampaignRunner and are not routed through this adapter.
/// </summary>
public static class NarrativeCampaignWorkflowAdapter
{
    public const string HandlerKey = "laboratory.executeNarrativeCampaign";
    public const string RequestInputKey = "laboratory.narrativeCampaignRequest";
    public const string AttemptOutputKey = "laboratory.narrativeCampaignAttempt";

    /// <summary>Registers one deterministic narrative attempt workflow and persists its final snapshot.</summary>
    public static void Register(
        WorkflowHandlerRegistry handlers,
        INarrativeCampaignAttemptStore attemptStore)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        ArgumentNullException.ThrowIfNull(attemptStore);

        handlers.Register(HandlerKey, async (context, cancellationToken) =>
        {
            var request = context.GetInput<NarrativeCampaignExecutionRequest>(RequestInputKey);
            if (request is null)
                throw new InvalidOperationException("The workflow did not supply a narrative campaign request.");

            var machine = new NarrativeCampaignStateMachine();
            var attempt = machine.Start(request.Definition, request.AttemptId, request.StartedAtUtc);

            try
            {
                foreach (var command in request.Commands)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    attempt = command.Kind switch
                    {
                        NarrativeCampaignCommandKind.RecordEvidence =>
                            machine.RecordEvidence(request.Definition, attempt, command.Evidence!, command.OccurredAtUtc),
                        NarrativeCampaignCommandKind.PlayerDeath =>
                            machine.RecordPlayerDeath(request.Definition, attempt, command.Value!, command.OccurredAtUtc),
                        NarrativeCampaignCommandKind.AttemptSafeEscape =>
                            machine.AttemptSafeEscape(request.Definition, attempt, command.Value!, command.OccurredAtUtc),
                        _ => throw new ArgumentOutOfRangeException(nameof(command), "Unknown narrative campaign command.")
                    };
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Preserve the active attempt's last complete snapshot even though FSM_Execution
                // owns and reports the workflow cancellation.
                await attemptStore.SaveAsync(attempt, CancellationToken.None).ConfigureAwait(false);
                throw;
            }

            try
            {
                await attemptStore.SaveAsync(attempt, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                await attemptStore.SaveAsync(attempt, CancellationToken.None).ConfigureAwait(false);
                throw;
            }

            context.SetOutput(AttemptOutputKey, NarrativeCampaignAttemptSnapshot.FromAttempt(attempt));
        });
    }
}
