using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Execution;

/// <summary>
/// JSON-safe workflow output projection of a validated narrative attempt. The domain attempt
/// itself keeps construction controlled by the state machine and validated persistence restore.
/// </summary>
public sealed record NarrativeCampaignAttemptSnapshot(
    Guid AttemptId,
    NarrativeCampaignDefinition Definition,
    string CampaignId,
    string CampaignVersion,
    string PlayerRole,
    string StartingLocation,
    NarrativeCampaignOutcome Outcome,
    IReadOnlyList<NarrativeEvidenceRecord> Evidence,
    IReadOnlyList<NarrativeCampaignEvent> Events)
{
    /// <summary>Creates a detached, immutable output snapshot from a validated domain attempt.</summary>
    public static NarrativeCampaignAttemptSnapshot FromAttempt(NarrativeCampaignAttempt attempt)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        return new NarrativeCampaignAttemptSnapshot(
            attempt.AttemptId,
            attempt.Definition,
            attempt.CampaignId,
            attempt.CampaignVersion,
            attempt.PlayerRole,
            attempt.StartingLocation,
            attempt.Outcome,
            Array.AsReadOnly(attempt.Evidence.ToArray()),
            Array.AsReadOnly(attempt.Events.ToArray()));
    }
}
