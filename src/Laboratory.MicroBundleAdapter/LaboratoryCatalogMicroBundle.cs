using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.MicroBundles;

/// <summary>Typed laboratory catalog capability exposed by a loaded laboratory MicroBundle.</summary>
/// <remarks>
/// A composition host resolves this interface by casting the loaded bundle instance.
/// The provider declaration in <see cref="MicroBundleDescriptor"/> is metadata, not a service locator.
/// </remarks>
public interface ILaboratoryCatalogProvider
{
    /// <summary>Gets the immutable laboratory definitions supplied by this capability.</summary>
    IReadOnlyList<LaboratoryDefinition> Laboratories { get; }

    /// <summary>Gets the immutable facility manifests supplied by this capability.</summary>
    IReadOnlyList<LaboratoryFacility> Facilities { get; }

    /// <summary>Attempts to resolve a laboratory by its stable identifier.</summary>
    bool TryGetLaboratory(string id, out LaboratoryDefinition? laboratory);

    /// <summary>Attempts to resolve a facility by its stable identifier.</summary>
    bool TryGetFacility(string id, out LaboratoryFacility? facility);
}

/// <summary>
/// A host-neutral, immutable catalog of portable laboratory and facility definitions.
/// </summary>
public sealed class LaboratoryCatalogMicroBundle : IMicroBundle, ILaboratoryCatalogProvider
{
    /// <summary>Stable identity for the first laboratory catalog capability.</summary>
    public const ulong BundleId = 6101;

    /// <summary>Stable typed provider declaration.</summary>
    public const string ProviderId = "laboratories.catalog.v1";

    private readonly IReadOnlyDictionary<string, LaboratoryDefinition> _laboratoriesById;
    private readonly IReadOnlyDictionary<string, LaboratoryFacility> _facilitiesById;

    /// <summary>Creates a catalog capability from portable definitions and facility manifests.</summary>
    public LaboratoryCatalogMicroBundle(
        IEnumerable<LaboratoryDefinition> laboratories,
        IEnumerable<LaboratoryFacility> facilities)
    {
        Laboratories = Snapshot(laboratories, nameof(laboratories));
        Facilities = Snapshot(facilities, nameof(facilities));
        _laboratoriesById = Index(Laboratories, x => x.Id, nameof(laboratories));
        _facilitiesById = Index(Facilities, x => x.Id, nameof(facilities));
    }

    /// <inheritdoc />
    public MicroBundleDescriptor Descriptor { get; } =
        new(BundleId, "0.1.0-alpha.1", providers: [new MicroBundleProvider(ProviderId)]);

    /// <inheritdoc />
    public IReadOnlyList<MicroBundleDependencyRequest> Dependencies { get; } =
        Array.Empty<MicroBundleDependencyRequest>();

    /// <inheritdoc />
    public IReadOnlyList<LaboratoryDefinition> Laboratories { get; }

    /// <inheritdoc />
    public IReadOnlyList<LaboratoryFacility> Facilities { get; }

    /// <inheritdoc />
    public void Load(IMicroBundleLoadContext context) =>
        ArgumentNullException.ThrowIfNull(context);

    /// <inheritdoc />
    public bool Arbitrate(IMicroBundleArbitrationContext context, int roundIndex)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentOutOfRangeException.ThrowIfNegative(roundIndex);
        return false;
    }

    /// <inheritdoc />
    public bool TryGetLaboratory(string id, out LaboratoryDefinition? laboratory) =>
        _laboratoriesById.TryGetValue(id, out laboratory);

    /// <inheritdoc />
    public bool TryGetFacility(string id, out LaboratoryFacility? facility) =>
        _facilitiesById.TryGetValue(id, out facility);

    private static IReadOnlyList<T> Snapshot<T>(IEnumerable<T> values, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(values, parameterName);
        var snapshot = values.ToArray();
        if (snapshot.Any(value => value is null))
            throw new ArgumentException("Catalog entries cannot contain null values.", parameterName);
        return Array.AsReadOnly(snapshot);
    }

    private static IReadOnlyDictionary<string, T> Index<T>(
        IReadOnlyList<T> values,
        Func<T, string> keySelector,
        string parameterName)
    {
        var index = new Dictionary<string, T>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            if (!index.TryAdd(keySelector(value), value))
                throw new ArgumentException("Catalog identifiers must be unique.", parameterName);
        }

        return new System.Collections.ObjectModel.ReadOnlyDictionary<string, T>(index);
    }
}
