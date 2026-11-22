using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>Stores immutable experiment run records for later retrieval and reproduction.</summary>
public interface IExperimentRunStore
{
    /// <summary>Persists a completed run without replacing an existing run identity.</summary>
    ValueTask SaveAsync(ExperimentRunRecord run, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a previously persisted run, or null when it does not exist.</summary>
    ValueTask<ExperimentRunRecord?> GetAsync(Guid runId, CancellationToken cancellationToken = default);

    /// <summary>Lists persisted run identities in deterministic order.</summary>
    ValueTask<IReadOnlyList<Guid>> ListAsync(CancellationToken cancellationToken = default);
}
