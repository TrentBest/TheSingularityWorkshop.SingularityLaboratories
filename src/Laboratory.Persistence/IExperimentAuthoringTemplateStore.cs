using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>Stores reusable, versioned experiment authoring templates.</summary>
public interface IExperimentAuthoringTemplateStore
{
    /// <summary>Persists a template without replacing an existing template version.</summary>
    ValueTask SaveAsync(ExperimentAuthoringTemplate template, CancellationToken cancellationToken = default);

    /// <summary>Retrieves an exact template version, or null when it does not exist.</summary>
    ValueTask<ExperimentAuthoringTemplate?> GetAsync(
        string templateId,
        string templateVersion,
        CancellationToken cancellationToken = default);

    /// <summary>Lists stored template identities in deterministic identity/version order.</summary>
    ValueTask<IReadOnlyList<ExperimentAuthoringTemplateIdentity>> ListAsync(
        CancellationToken cancellationToken = default);
}

/// <summary>Stable identity of one stored authoring-template version.</summary>
public sealed record ExperimentAuthoringTemplateIdentity(string Id, string Version);
