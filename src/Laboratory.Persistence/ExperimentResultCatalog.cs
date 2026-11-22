using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Reads persisted experiment runs as host-neutral result views. The underlying run store
/// remains authoritative; this catalog adds no host-specific presentation or copied payload.
/// </summary>
public sealed class ExperimentResultCatalog
{
    private readonly IExperimentRunStore _store;

    public ExperimentResultCatalog(IExperimentRunStore store)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
    }

    /// <summary>Gets a result projection for one persisted run, or null when it is absent.</summary>
    public async ValueTask<ExperimentResultView?> GetAsync(
        Guid runId,
        CancellationToken cancellationToken = default)
    {
        var run = await _store.GetAsync(runId, cancellationToken).ConfigureAwait(false);
        return run is null ? null : new ExperimentResultView(run);
    }

    /// <summary>
    /// Lists available result projections in completion-time order, with run identity as a
    /// deterministic tie-breaker. A record removed between listing and retrieval is skipped.
    /// </summary>
    public async ValueTask<IReadOnlyList<ExperimentResultView>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var ids = await _store.ListAsync(cancellationToken).ConfigureAwait(false);
        var results = new List<ExperimentResultView>(ids.Count);

        foreach (var id in ids)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var run = await _store.GetAsync(id, cancellationToken).ConfigureAwait(false);
            if (run is not null)
                results.Add(new ExperimentResultView(run));
        }

        results.Sort(static (left, right) =>
        {
            var completed = left.CompletedAtUtc.CompareTo(right.CompletedAtUtc);
            return completed != 0 ? completed : left.RunId.CompareTo(right.RunId);
        });

        return Array.AsReadOnly(results.ToArray());
    }
}
