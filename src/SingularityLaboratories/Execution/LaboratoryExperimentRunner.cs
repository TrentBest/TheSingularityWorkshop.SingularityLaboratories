using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>Executes a model and captures a reproducible, host-neutral run record.</summary>
public sealed class LaboratoryExperimentRunner
{
    public async ValueTask<ExperimentRunRecord> RunAsync(
        LaboratoryDefinition laboratory,
        IExperimentModel model,
        ExperimentConfiguration configuration,
        CancellationToken cancellationToken = default,
        ExperimentResourceProfile? resourceProfile = null,
        ExecutionResourceBudget? resourceBudget = null,
        ExperimentExecutionPolicy? executionPolicy = null)
    {
        ArgumentNullException.ThrowIfNull(laboratory);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(configuration);

        if ((resourceProfile is null) != (resourceBudget is null))
            throw new ArgumentException("A resource profile and budget must be supplied together.");

        var definition = model.Definition ?? throw new InvalidOperationException("The model has no experiment definition.");
        var schema = model.ParameterSchema ?? throw new InvalidOperationException("The model has no parameter schema.");
        executionPolicy?.EnsureCanExecute(definition);
        if (!string.Equals(configuration.ExperimentId, definition.Id, StringComparison.Ordinal) ||
            !string.Equals(configuration.ExperimentVersion, definition.Version, StringComparison.Ordinal))
        {
            throw new ArgumentException("The configuration does not target the model's exact experiment identity and version.", nameof(configuration));
        }
        if (!string.Equals(schema.ExperimentId, definition.Id, StringComparison.Ordinal) ||
            !string.Equals(schema.ExperimentVersion, definition.Version, StringComparison.Ordinal) ||
            configuration.SchemaVersion != schema.SchemaVersion)
        {
            throw new ArgumentException("The configuration does not target the model's exact parameter schema version.", nameof(configuration));
        }
        if (!string.Equals(configuration.ContentType, "application/json", StringComparison.Ordinal))
            throw new ArgumentException("The current portable parameter schema requires application/json.", nameof(configuration));
        schema.ValidateJson(configuration.Payload);

        if (resourceProfile is not null && resourceBudget is not null)
        {
            if (!string.Equals(resourceProfile.ModelId, definition.ModelId, StringComparison.Ordinal) ||
                !string.Equals(resourceProfile.ModelVersion, definition.ModelVersion, StringComparison.Ordinal))
            {
                throw new ArgumentException("The resource profile does not target the exact model identity and version.", nameof(resourceProfile));
            }

            resourceProfile.EnsureWithin(resourceBudget);
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
