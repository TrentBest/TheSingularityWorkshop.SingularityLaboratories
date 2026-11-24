using System.Text.Json;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Persists narrative attempt snapshots as atomic JSON documents. Each document contains the
/// immutable campaign definition, current outcome, complete evidence history, and ordered events.
/// </summary>
public sealed class FileSystemNarrativeCampaignAttemptStore : INarrativeCampaignAttemptStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _directory;

    public FileSystemNarrativeCampaignAttemptStore(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(_directory);
    }

    public async ValueTask SaveAsync(
        NarrativeCampaignAttempt attempt,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        cancellationToken.ThrowIfCancellationRequested();

        var document = new NarrativeCampaignAttemptDocument
        {
            AttemptId = attempt.AttemptId,
            Definition = attempt.Definition,
            Outcome = attempt.Outcome,
            Evidence = attempt.Evidence.ToArray(),
            Events = attempt.Events.ToArray()
        };

        var destination = GetPath(attempt.AttemptId);
        var temporary = Path.Combine(_directory, $".{attempt.AttemptId:N}.{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(
                temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, document, SerializerOptions, cancellationToken)
                    .ConfigureAwait(false);
                await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporary, destination, overwrite: false);
        }
        finally
        {
            if (File.Exists(temporary))
                File.Delete(temporary);
        }
    }

    public async ValueTask<NarrativeCampaignAttempt?> GetAsync(
        Guid attemptId,
        CancellationToken cancellationToken = default)
    {
        if (attemptId == Guid.Empty)
            throw new ArgumentException("An attempt identity is required.", nameof(attemptId));

        var path = GetPath(attemptId);
        if (!File.Exists(path))
            return null;

        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, FileOptions.Asynchronous | FileOptions.SequentialScan);

        try
        {
            var document = await JsonSerializer.DeserializeAsync<NarrativeCampaignAttemptDocument>(
                stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
            if (document is null || document.AttemptId != attemptId || document.Definition is null ||
                document.Evidence is null || document.Events is null)
                throw new InvalidDataException("The persisted narrative attempt is incomplete or does not match its requested identity.");

            return NarrativeCampaignAttempt.Restore(
                document.AttemptId,
                document.Definition,
                document.Outcome,
                document.Evidence,
                document.Events);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("The persisted narrative attempt is not valid JSON.", exception);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidDataException("The persisted narrative attempt violates its domain invariants.", exception);
        }
    }

    public ValueTask<IReadOnlyList<Guid>> ListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var attempts = Directory.EnumerateFiles(_directory, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(name => Guid.TryParseExact(name, "N", out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .OrderBy(id => id)
            .ToArray();

        return ValueTask.FromResult<IReadOnlyList<Guid>>(Array.AsReadOnly(attempts));
    }

    private string GetPath(Guid attemptId) =>
        Path.Combine(_directory, $"{attemptId:N}.json");
}

internal sealed class NarrativeCampaignAttemptDocument
{
    public NarrativeCampaignAttemptDocument()
    {
    }

    public Guid AttemptId { get; set; }
    public NarrativeCampaignDefinition? Definition { get; set; }
    public NarrativeCampaignOutcome Outcome { get; set; }
    public IReadOnlyList<NarrativeEvidenceRecord>? Evidence { get; set; }
    public IReadOnlyList<NarrativeCampaignEvent>? Events { get; set; }
}
