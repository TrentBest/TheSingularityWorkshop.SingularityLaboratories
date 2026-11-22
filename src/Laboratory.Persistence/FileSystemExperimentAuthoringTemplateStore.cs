using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Persists versioned authoring templates as atomic JSON documents. A template ID/version pair
/// maps to a stable opaque filename, and existing versions are never overwritten.
/// </summary>
public sealed class FileSystemExperimentAuthoringTemplateStore : IExperimentAuthoringTemplateStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string _directory;

    public FileSystemExperimentAuthoringTemplateStore(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        _directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(_directory);
    }

    public async ValueTask SaveAsync(
        ExperimentAuthoringTemplate template,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(template);
        cancellationToken.ThrowIfCancellationRequested();

        var destination = GetPath(template.Id, template.Version);
        var temporary = Path.Combine(_directory, $".{Guid.NewGuid():N}.tmp");
        try
        {
            await using (var stream = new FileStream(
                temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                4096, FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, ExperimentAuthoringTemplateDocument.From(template), SerializerOptions, cancellationToken)
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

    public async ValueTask<ExperimentAuthoringTemplate?> GetAsync(
        string templateId,
        string templateVersion,
        CancellationToken cancellationToken = default)
    {
        var id = Required(templateId, nameof(templateId));
        var version = Required(templateVersion, nameof(templateVersion));
        cancellationToken.ThrowIfCancellationRequested();

        var path = GetPath(id, version);
        if (!File.Exists(path))
            return null;

        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        var template = await JsonSerializer.DeserializeAsync<ExperimentAuthoringTemplateDocument>(
            stream, SerializerOptions, cancellationToken).ConfigureAwait(false);

        if (template is null ||
            !StringComparer.Ordinal.Equals(template.Id, id) ||
            !StringComparer.Ordinal.Equals(template.Version, version))
            throw new InvalidDataException("The persisted authoring template does not match its requested identity.");

        return template.ToTemplate();
    }

    public async ValueTask<IReadOnlyList<ExperimentAuthoringTemplateIdentity>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var templates = new List<ExperimentAuthoringTemplateIdentity>();

        foreach (var path in Directory.EnumerateFiles(_directory, "*.json", SearchOption.TopDirectoryOnly))
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var stream = new FileStream(
                path, FileMode.Open, FileAccess.Read, FileShare.Read,
                4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            var template = await JsonSerializer.DeserializeAsync<ExperimentAuthoringTemplateDocument>(
                stream, SerializerOptions, cancellationToken).ConfigureAwait(false);

            if (template is null || !StringComparer.Ordinal.Equals(path, GetPath(template.Id, template.Version)))
                throw new InvalidDataException("A persisted authoring template has an invalid identity or filename.");

            templates.Add(new ExperimentAuthoringTemplateIdentity(template.Id, template.Version));
        }

        var ordered = templates
            .OrderBy(template => template.Id, StringComparer.Ordinal)
            .ThenBy(template => template.Version, StringComparer.Ordinal)
            .ToArray();
        return Array.AsReadOnly(ordered);
    }

    private string GetPath(string id, string version)
    {
        var identity = Encoding.UTF8.GetBytes($"{id}\n{version}");
        var hash = Convert.ToHexString(SHA256.HashData(identity));
        return Path.Combine(_directory, $"{hash}.json");
    }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
