using NUnit.Framework;
using TheSingularityWorkshop.MicroBundleDomain;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.MicroBundles;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryCatalogMicroBundleTests
{
    [Test]
    public void Constructor_ExposesPortableCatalogThroughTypedLoadedBundle()
    {
        var laboratories = new List<LaboratoryDefinition>
        {
            new("electrical-thermal", "1.0.0", "Electrical and thermal lab", "Educational coupling experiment.")
        };
        var facilities = new List<LaboratoryFacility>
        {
            new("teaching-facility", "1.0.0", "Teaching facility",
                [new LaboratoryRoom("bench", "Bench", "Controlled experiments.")],
                [], [], [])
        };

        var bundle = new LaboratoryCatalogMicroBundle(laboratories, facilities);
        laboratories.Clear();
        facilities.Clear();

        Assert.That(bundle, Is.InstanceOf<IMicroBundle>());
        Assert.That(bundle, Is.InstanceOf<ILaboratoryCatalogProvider>());
        Assert.That(bundle.Descriptor.Id, Is.EqualTo(LaboratoryCatalogMicroBundle.BundleId));
        Assert.That(bundle.Descriptor.Providers.Select(x => x.Id), Does.Contain(LaboratoryCatalogMicroBundle.ProviderId));
        Assert.That(bundle.Dependencies, Is.Empty);
        Assert.That(bundle.Laboratories, Has.Count.EqualTo(1));
        Assert.That(bundle.Facilities, Has.Count.EqualTo(1));
        Assert.That(bundle.TryGetLaboratory("electrical-thermal", out var laboratory), Is.True);
        Assert.That(laboratory!.Version, Is.EqualTo("1.0.0"));
        Assert.That(bundle.TryGetFacility("teaching-facility", out var facility), Is.True);
        Assert.That(facility!.Rooms[0].Id, Is.EqualTo("bench"));
        Assert.That(bundle.TryGetLaboratory("missing", out _), Is.False);
    }

    [Test]
    public void Constructor_RejectsDuplicateCatalogIdentifiers()
    {
        var duplicateLabs = new[]
        {
            new LaboratoryDefinition("lab", "1.0.0", "Lab A", "First."),
            new LaboratoryDefinition("lab", "2.0.0", "Lab B", "Second.")
        };

        Assert.Throws<ArgumentException>(() => new LaboratoryCatalogMicroBundle(duplicateLabs, []));
    }

    [Test]
    public void LoadAndArbitration_RespectNeutralMicroBundleLifecycle()
    {
        var bundle = new LaboratoryCatalogMicroBundle([], []);
        var loadContext = new TestLoadContext();
        var arbitrationContext = new TestArbitrationContext(bundle);

        Assert.DoesNotThrow(() => bundle.Load(loadContext));
        Assert.That(bundle.Arbitrate(arbitrationContext, 0), Is.False);
        Assert.Throws<ArgumentOutOfRangeException>(() => bundle.Arbitrate(arbitrationContext, -1));
    }

    private sealed class TestLoadContext : IMicroBundleLoadContext
    {
        public ulong RuntimeId => 42;
        public bool TryGetConfiguration(ulong bundleId, out ReadOnlyMemory<byte> configuration)
        {
            configuration = ReadOnlyMemory<byte>.Empty;
            return false;
        }
    }

    private sealed class TestArbitrationContext(IMicroBundle bundle) : IMicroBundleArbitrationContext
    {
        public ulong RuntimeId => 42;
        public IReadOnlyList<IMicroBundle> Bundles { get; } = [bundle];
        public object? ExperienceContext => null;
    }
}
