namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// A host-neutral, read-only projection of a completed run for discovery and presentation.
/// The referenced run record remains the durable scientific record; this view is not a
/// second source of truth and deliberately contains no UI, renderer, or world types.
/// </summary>
public sealed class ExperimentResultView
{
    public ExperimentResultView(ExperimentRunRecord run)
    {
        ArgumentNullException.ThrowIfNull(run);
        Run = run;
    }

    /// <summary>The authoritative immutable run record projected by this view.</summary>
    public ExperimentRunRecord Run { get; }

    public Guid RunId => Run.RunId;
    public Guid? ReplayedFromRunId => Run.ReplayedFromRunId;
    public string LaboratoryId => Run.LaboratoryId;
    public string LaboratoryVersion => Run.LaboratoryVersion;
    public string ExperimentId => Run.ExperimentId;
    public string ExperimentVersion => Run.ExperimentVersion;
    public string ModelId => Run.ModelId;
    public string ModelVersion => Run.ModelVersion;
    public ScientificValidationLevel ValidationLevel => Run.ValidationLevel;
    public DateTimeOffset StartedAtUtc => Run.StartedAtUtc;
    public DateTimeOffset CompletedAtUtc => Run.CompletedAtUtc;
    public TimeSpan Elapsed => Run.CompletedAtUtc - Run.StartedAtUtc;
    public string ContentType => Run.ResultContentType;
    public string Payload => Run.ResultPayload;
    public IReadOnlyList<string> Assumptions => Run.Assumptions;
    public IReadOnlyList<string> Limitations => Run.Limitations;
}
