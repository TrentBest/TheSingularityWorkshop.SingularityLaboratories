using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentSettingsDashboardTests
{
    private static ExperimentDefinition Definition() =>
        new(
            "electrical-thermal-coupling",
            "1.0.0",
            "Electrical to lumped thermal energy coupling",
            "physics.electrical-thermal",
            "2.1.0",
            ScientificValidationLevel.EducationalModel,
            ["Ideal fixed-voltage bus."],
            ["No spatial temperature gradients."],
            physicsDependencies:
            [
                new ExperimentPhysicsDependency(
                    "electrical.power",
                    "Electromagnetism",
                    "physics.electrical-thermal",
                    "2.1.0",
                    "Calculate resistive electrical demand and energy.")
            ]);

    [Test]
    public void Dashboard_SeparatesDeclaredPhysicsFromObservedLoadedBundles()
    {
        var definition = Definition();
        var configured = new ExperimentSettingsDashboard(definition);

        Assert.That(configured.RunId, Is.Null, "An authoring preview has no selected run.");
        Assert.That(configured.PhysicsDependencies, Has.Count.EqualTo(1));
        Assert.That(configured.RequiredMicroBundles, Is.Empty,
            "This experiment adapter references Physics directly; it does not declare a Physics MicroBundle.");
        Assert.That(configured.ObservedMicroBundles, Is.Empty,
            "A declaration alone must not imply that a MicroBundle was loaded or executed.");

        var runId = Guid.NewGuid();
        var observed = new ExperimentSettingsDashboard(
            definition,
            [
                new MicroBundleExecutionObservation(
                    6101, "0.1.0-alpha.1", "laboratories.catalog.v1",
                    MicroBundleExecutionStatus.Loaded,
                    new DateTimeOffset(2026, 11, 23, 5, 0, 0, TimeSpan.Zero))
            ],
            runId);

        Assert.That(observed.RunId, Is.EqualTo(runId));
        Assert.That(observed.ObservedMicroBundles, Has.Count.EqualTo(1));
        Assert.That(observed.ObservedMicroBundles[0].Status, Is.EqualTo(MicroBundleExecutionStatus.Loaded));
        Assert.That(observed.ObservedMicroBundles[0].BundleId, Is.EqualTo(6101));
    }

    [Test]
    public void Dashboard_RejectsRuntimeObservationsWithoutRunIdentity()
    {
        var observation = new MicroBundleExecutionObservation(
            6101, "0.1.0-alpha.1", "laboratories.catalog.v1",
            MicroBundleExecutionStatus.Loaded,
            new DateTimeOffset(2026, 11, 23, 5, 0, 0, TimeSpan.Zero));

        Assert.Throws<ArgumentException>(() =>
            new ExperimentSettingsDashboard(Definition(), [observation]));
    }

    [Test]
    public void Definition_SnapshotsPhysicsDeclarations()
    {
        var physics = new List<ExperimentPhysicsDependency>
        {
            new("thermal.state", "Thermodynamics", "physics.thermal", "1.0.0", "Resolve thermal state.")
        };

        var definition = new ExperimentDefinition(
            "thermal", "1.0.0", "Thermal", "physics.thermal", "1.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Constant heat capacity."], ["No phase changes."],
            physicsDependencies: physics);

        physics.Clear();

        Assert.That(definition.PhysicsDependencies, Has.Count.EqualTo(1));
    }

    [Test]
    public void Dashboard_RejectsDuplicateRuntimeObservations()
    {
        var observation = new MicroBundleExecutionObservation(
            6101, "0.1.0-alpha.1", "laboratories.catalog.v1",
            MicroBundleExecutionStatus.Loaded,
            new DateTimeOffset(2026, 11, 23, 5, 0, 0, TimeSpan.Zero));

        Assert.Throws<ArgumentException>(() =>
            new ExperimentSettingsDashboard(Definition(), [observation, observation], Guid.NewGuid()));
    }
}
