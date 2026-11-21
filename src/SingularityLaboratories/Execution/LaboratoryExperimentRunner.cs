using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>Executes a model and captures a reproducible, host-neutral run record.</summary>
public sealed class LaboratoryExperimentRunner
{
    public async ValueTask<ExperimentRunRecord> RunAsync(
        LaboratoryDefinition laboratory,
        IExperimentModel model,
        ExperimentConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(laboratory);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(configuration);

        var definition = model.Definition ?? throw new InvalidOperationException("The model has no experiment definition.");
        if (!string.Equals(configuration.ExperimentId, definition.Id, StringComparison.Ordinal) ||
            !string.Equals(configuration.ExperimentVersion, definition.Version, StringComparison.Ordinal))
        {
            throw new ArgumentException("The configuration does not target the model's exact experiment identity and version.", nameof(configuration));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var runId = Guid.NewGuid();
        var startedAtUtc = DateTimeOffset.UtcNow;
        var output = await model.ExecuteAsync(configuration, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var completedAtUtc = DateTimeOffset.UtcNow;

        return new ExperimentRunRecord(
            runId,
            laboratory.Id,
            laboratory.Version,
            definition.Id,
            definition.Version,
            definition.ModelId,
            definition.ModelVersion,
            configuration.SchemaVersion,
            configuration.ContentType,
            configuration.Payload,
            definition.ValidationLevel,
            startedAtUtc,
            completedAtUtc,
            output.ContentType,
            output.Payload,
            definition.Assumptions,
            definition.Limitations);
    }
}
