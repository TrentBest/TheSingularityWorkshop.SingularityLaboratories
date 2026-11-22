using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Persistence;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentAuthoringTemplateStoreTests
{
    private string _directory = null!;

    [SetUp]
    public void SetUp()
    {
        _directory = Path.Combine(Path.GetTempPath(), "SingularityLaboratories", Guid.NewGuid().ToString("N"));
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }

    private static ExperimentAuthoringTemplate CreateTemplate(string id = "thermal-template", string version = "1.0.0")
    {
        var experiment = new ExperimentDefinition(
            "thermal-coupling", "1.0.0", "Electrical to thermal coupling", "physics.thermal", "2.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);
        var schema = new ExperimentParameterSchema(
            experiment.Id, experiment.Version, 1,
            [
                new ExperimentParameterDefinition("voltageV", "Voltage", "Applied voltage.", ExperimentParameterType.Number, minimum: 0),
                new ExperimentParameterDefinition("resistanceOhm", "Resistance", "Load resistance.", ExperimentParameterType.Number, minimum: 0.001)
            ]);
        var configuration = new ExperimentConfiguration(
            experiment.Id, experiment.Version, 1, "application/json",
            "{\"voltageV\":12,\"resistanceOhm\":6}");
        return new ExperimentAuthoringTemplate(id, version, "Introductory thermal experiment", experiment, schema, configuration);
    }

    [Test]
    public async Task SaveAndGet_RoundTripsExactTemplateVersion()
    {
        var store = new FileSystemExperimentAuthoringTemplateStore(_directory);
        var template = CreateTemplate();

        await store.SaveAsync(template);
        var restored = await store.GetAsync(template.Id, template.Version);

        Assert.That(restored, Is.Not.Null);
        Assert.That(restored!.Id, Is.EqualTo(template.Id));
        Assert.That(restored.Version, Is.EqualTo(template.Version));
        Assert.That(restored.Experiment.Id, Is.EqualTo(template.Experiment.Id));
        Assert.That(restored.ParameterSchema.SchemaVersion, Is.EqualTo(template.ParameterSchema.SchemaVersion));
        Assert.That(restored.DefaultConfiguration.Payload, Is.EqualTo(template.DefaultConfiguration.Payload));
    }

    [Test]
    public async Task SaveAsync_RejectsReplacingAnExistingTemplateVersion()
    {
        var store = new FileSystemExperimentAuthoringTemplateStore(_directory);
        var template = CreateTemplate();

        await store.SaveAsync(template);

        Assert.ThrowsAsync<IOException>(async () =>
            await store.SaveAsync(CreateTemplate(template.Id, template.Version)));
    }

    [Test]
    public async Task ListAsync_ReturnsDeterministicIdentityAndVersionOrder()
    {
        var store = new FileSystemExperimentAuthoringTemplateStore(_directory);
        await store.SaveAsync(CreateTemplate("zeta", "2.0.0"));
        await store.SaveAsync(CreateTemplate("alpha", "2.0.0"));
        await store.SaveAsync(CreateTemplate("alpha", "1.0.0"));

        var listed = await store.ListAsync();

        Assert.That(listed.Select(item => (item.Id, item.Version)), Is.EqualTo(new[]
        {
            ("alpha", "1.0.0"),
            ("alpha", "2.0.0"),
            ("zeta", "2.0.0")
        }));
    }

    [Test]
    public async Task GetAsync_ReturnsNullForUnknownVersion()
    {
        var store = new FileSystemExperimentAuthoringTemplateStore(_directory);

        Assert.That(await store.GetAsync("missing", "1.0.0"), Is.Null);
    }
}
