using System.Text.Json;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Persists run records as individual JSON documents. Writes use a same-directory temporary
/// file and an atomic, non-overwriting move so a completed record is never partially visible.
/// </summary>
public sealed class FileSystemExperimentRunStore : IExperimentRunStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _directory;

    public FileSystemExperimentRunStore(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(_directory);
    }

    public async ValueTask SaveAsync(
        ExperimentRunRecord run,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(run);
        cancellationToken.ThrowIfCancellationRequested();

        var destination = GetPath(run.RunId);
        var temporary = Path.Combine(_directory, $".{run.RunId:N}.{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(
                temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, run, SerializerOptions, cancellationToken)
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

    public async ValueTask<ExperimentRunRecord?> GetAsync(
        Guid runId,
        CancellationToken cancellationToken = default)
    {
        if (runId == Guid.Empty)
            throw new ArgumentException("A run identity is required.", nameof(runId));

        var path = GetPath(runId);
        if (!File.Exists(path))
            return null;

        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var run = await JsonSerializer.DeserializeAsync<ExperimentRunRecord>(
            stream, SerializerOptions, cancellationToken).ConfigureAwait(false);

        if (run is null || run.RunId != runId)
            throw new InvalidDataException("The persisted run record does not match its requested identity.");

        return run;
    }

    public ValueTask<IReadOnlyList<Guid>> ListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var runs = Directory.EnumerateFiles(_directory, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(name => Guid.TryParseExact(name, "N", out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .OrderBy(id => id)
            .ToArray();

        return ValueTask.FromResult<IReadOnlyList<Guid>>(Array.AsReadOnly(runs));
    }

    private string GetPath(Guid runId) => Path.Combine(_directory, $"{runId:N}.json");
}
