using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>A scientific model adapter that validates and executes one versioned experiment configuration.</summary>
public interface IExperimentModel
{
    ExperimentDefinition Definition { get; }

    ValueTask<ExperimentModelOutput> ExecuteAsync(
        ExperimentConfiguration configuration,
        CancellationToken cancellationToken = default);
}

/// <summary>Serialized model output with an explicit media type.</summary>
public sealed record ExperimentModelOutput
{
    public ExperimentModelOutput(string contentType, string payload)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);
        ArgumentNullException.ThrowIfNull(payload);
        ContentType = contentType.Trim();
        Payload = payload;
    }

    public string ContentType { get; }
    public string Payload { get; }
}
