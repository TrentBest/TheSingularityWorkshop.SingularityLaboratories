using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>Re-executes a stored configuration against the exact recorded experiment and model versions.</summary>
public sealed class ExperimentRunReplayService
{
    private readonly IExperimentRunStore _store;
    private readonly LaboratoryExperimentRunner _runner;

    public ExperimentRunReplayService(IExperimentRunStore store, LaboratoryExperimentRunner? runner = null)
    {
        ArgumentNullException.ThrowIfNull(store);
        _store = store;
        _runner = runner ?? new LaboratoryExperimentRunner();
    }

    /// <summary>
    /// Reproduces a stored run as a new run linked to its source. The resolver must return the
    /// exact experiment version; model identity, validation level, assumptions, and limitations
    /// are checked before any model execution occurs.
    /// </summary>
    public async ValueTask<ExperimentRunRecord> ReplayAsync(
        LaboratoryDefinition laboratory,
        Guid sourceRunId,
        Func<string, string, IExperimentModel?> modelResolver,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(laboratory);
        ArgumentNullException.ThrowIfNull(modelResolver);
        if (sourceRunId == Guid.Empty)
            throw new ArgumentException("A source run identity is required.", nameof(sourceRunId));

        var source = await _store.GetAsync(sourceRunId, cancellationToken).ConfigureAwait(false)
            ?? throw new KeyNotFoundException($"Experiment run '{sourceRunId}' was not found.");

        if (!string.Equals(source.LaboratoryId, laboratory.Id, StringComparison.Ordinal) ||
            !string.Equals(source.LaboratoryVersion, laboratory.Version, StringComparison.Ordinal))
            throw new InvalidOperationException("Replay requires the exact recorded laboratory identity and version.");

        var model = modelResolver(source.ExperimentId, source.ExperimentVersion)
            ?? throw new InvalidOperationException("The exact recorded experiment version is unavailable.");
        var definition = model.Definition
            ?? throw new InvalidOperationException("The resolved model has no experiment definition.");

        if (!string.Equals(definition.Id, source.ExperimentId, StringComparison.Ordinal) ||
            !string.Equals(definition.Version, source.ExperimentVersion, StringComparison.Ordinal) ||
            !string.Equals(definition.ModelId, source.ModelId, StringComparison.Ordinal) ||
            !string.Equals(definition.ModelVersion, source.ModelVersion, StringComparison.Ordinal))
            throw new InvalidOperationException("The resolved model does not match the exact recorded experiment and model identity.");

        if (definition.ValidationLevel != source.ValidationLevel ||
            !definition.Assumptions.SequenceEqual(source.Assumptions, StringComparer.Ordinal) ||
            !definition.Limitations.SequenceEqual(source.Limitations, StringComparer.Ordinal))
            throw new InvalidOperationException("The resolved model disclosures differ from the stored run provenance.");

        var configuration = new ExperimentConfiguration(
            source.ExperimentId,
            source.ExperimentVersion,
            source.ConfigurationSchemaVersion,
            source.ConfigurationContentType,
            source.ConfigurationPayload);

        var replay = await _runner.RunAsync(laboratory, model, configuration, cancellationToken)
            .ConfigureAwait(false);

        var linkedReplay = new ExperimentRunRecord(
            replay.RunId,
            replay.LaboratoryId,
            replay.LaboratoryVersion,
            replay.ExperimentId,
            replay.ExperimentVersion,
            replay.ModelId,
            replay.ModelVersion,
            replay.ConfigurationSchemaVersion,
            replay.ConfigurationContentType,
            replay.ConfigurationPayload,
            replay.ValidationLevel,
            replay.StartedAtUtc,
            replay.CompletedAtUtc,
            replay.ResultContentType,
            replay.ResultPayload,
            replay.Assumptions,
            replay.Limitations,
            source.RunId);

        await _store.SaveAsync(linkedReplay, cancellationToken).ConfigureAwait(false);
        return linkedReplay;
    }
}
