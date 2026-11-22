using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Persistence;

/// <summary>
/// Exports and imports portable, versioned authoring-template packages.
/// The package contains a SHA-256 integrity digest; it is not a digital signature or proof of publisher identity.
/// </summary>
public static class ExperimentAuthoringTemplateSerializer
{
    private const int CurrentFormatVersion = 1;
    private static readonly JsonSerializerOptions PayloadOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    private static readonly JsonSerializerOptions PackageOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>Creates deterministic JSON containing a versioned template payload and its integrity digest.</summary>
    public static string ExportToJson(ExperimentAuthoringTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);
        var payload = JsonSerializer.Serialize(ExperimentAuthoringTemplateDocument.From(template), PayloadOptions);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var package = new ExportPackageDocument
        {
            FormatVersion = CurrentFormatVersion,
            Payload = Convert.ToBase64String(payloadBytes),
            Sha256 = Convert.ToHexString(SHA256.HashData(payloadBytes))
        };
        return JsonSerializer.Serialize(package, PackageOptions);
    }

    /// <summary>Validates package format and payload integrity, then reconstructs the template through domain constructors.</summary>
    public static ExperimentAuthoringTemplate ImportFromJson(string packageJson)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(packageJson);
        var package = JsonSerializer.Deserialize<ExportPackageDocument>(packageJson, PackageOptions)
            ?? throw new InvalidDataException("The authoring-template package is empty.");

        if (package.FormatVersion != CurrentFormatVersion)
            throw new NotSupportedException($"Authoring-template package format version '{package.FormatVersion}' is not supported.");
        if (string.IsNullOrWhiteSpace(package.Payload) || string.IsNullOrWhiteSpace(package.Sha256))
            throw new InvalidDataException("The authoring-template package is missing its payload or integrity digest.");

        byte[] payloadBytes;
        try
        {
            payloadBytes = Convert.FromBase64String(package.Payload);
        }
        catch (FormatException exception)
        {
            throw new InvalidDataException("The authoring-template package payload is not valid Base64.", exception);
        }

        byte[] expectedDigest;
        try
        {
            expectedDigest = Convert.FromHexString(package.Sha256);
        }
        catch (FormatException exception)
        {
            throw new InvalidDataException("The authoring-template package digest is malformed.", exception);
        }

        var actualDigest = SHA256.HashData(payloadBytes);
        if (expectedDigest.Length != actualDigest.Length ||
            !CryptographicOperations.FixedTimeEquals(expectedDigest, actualDigest))
            throw new InvalidDataException("The authoring-template package failed its SHA-256 integrity check.");

        var document = JsonSerializer.Deserialize<ExperimentAuthoringTemplateDocument>(
            payloadBytes, PayloadOptions)
            ?? throw new InvalidDataException("The authoring-template package payload is empty.");
        try
        {
            return document.ToTemplate();
        }
        catch (ArgumentException exception)
        {
            throw new InvalidDataException("The authoring-template package contains an invalid domain definition.", exception);
        }
    }

    private sealed record ExportPackageDocument
    {
        public required int FormatVersion { get; init; }
        public required string Payload { get; init; }
        public required string Sha256 { get; init; }
    }
}
