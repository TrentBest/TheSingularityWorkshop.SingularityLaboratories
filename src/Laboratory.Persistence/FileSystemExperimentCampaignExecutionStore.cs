using System.Text.Json;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Persists complete campaign execution envelopes as atomic JSON documents. Each envelope embeds
/// the successful scenario run records, preserving campaign-to-run lineage in one durable artifact.
/// </summary>
public sealed class FileSystemExperimentCampaignExecutionStore : IExperimentCampaignExecutionStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _directory;

    public FileSystemExperimentCampaignExecutionStore(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(_directory);
    }

    public async ValueTask SaveAsync(
        ExperimentCampaignExecutionRecord execution,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(execution);
        cancellationToken.ThrowIfCancellationRequested();

        var destination = GetPath(execution.ExecutionId);
        var temporary = Path.Combine(_directory, $".{execution.ExecutionId:N}.{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(
                temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, execution, SerializerOptions, cancellationToken)
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

    public async ValueTask<ExperimentCampaignExecutionRecord?> GetAsync(
        Guid executionId,
        CancellationToken cancellationToken = default)
    {
        if (executionId == Guid.Empty)
            throw new ArgumentException("A campaign execution identity is required.", nameof(executionId));

        var path = GetPath(executionId);
        if (!File.Exists(path))
            return null;

        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var execution = await JsonSerializer.DeserializeAsync<ExperimentCampaignExecutionRecord>(
            stream, SerializerOptions, cancellationToken).ConfigureAwait(false);

        if (execution is null || execution.ExecutionId != executionId)
            throw new InvalidDataException("The persisted campaign execution does not match its requested identity.");

        return execution;
    }

    public ValueTask<IReadOnlyList<Guid>> ListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var executions = Directory.EnumerateFiles(_directory, "*.json", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(name => Guid.TryParseExact(name, "N", out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .OrderBy(id => id)
            .ToArray();

        return ValueTask.FromResult<IReadOnlyList<Guid>>(Array.AsReadOnly(executions));
    }

    private string GetPath(Guid executionId) =>
        Path.Combine(_directory, $"{executionId:N}.json");
}
