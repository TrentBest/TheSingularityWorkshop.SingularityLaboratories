using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>Stores immutable campaign execution envelopes and their complete scenario lineage.</summary>
public interface IExperimentCampaignExecutionStore
{
    /// <summary>Persists a campaign execution without replacing an existing execution identity.</summary>
    ValueTask SaveAsync(
        ExperimentCampaignExecutionRecord execution,
        CancellationToken cancellationToken = default);

    /// <summary>Retrieves a campaign execution, or null when it does not exist.</summary>
    ValueTask<ExperimentCampaignExecutionRecord?> GetAsync(
        Guid executionId,
        CancellationToken cancellationToken = default);

    /// <summary>Lists campaign execution identities in deterministic order.</summary>
    ValueTask<IReadOnlyList<Guid>> ListAsync(CancellationToken cancellationToken = default);
}
