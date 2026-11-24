using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>Stores immutable narrative campaign attempts without replacing prior attempt identities.</summary>
public interface INarrativeCampaignAttemptStore
{
    /// <summary>Persists one attempt snapshot, including its complete evidence and event history.</summary>
    ValueTask SaveAsync(NarrativeCampaignAttempt attempt, CancellationToken cancellationToken = default);

    /// <summary>Retrieves an attempt by identity, or null when no such attempt exists.</summary>
    ValueTask<NarrativeCampaignAttempt?> GetAsync(Guid attemptId, CancellationToken cancellationToken = default);

    /// <summary>Lists persisted attempt identities in deterministic order.</summary>
    ValueTask<IReadOnlyList<Guid>> ListAsync(CancellationToken cancellationToken = default);
}
