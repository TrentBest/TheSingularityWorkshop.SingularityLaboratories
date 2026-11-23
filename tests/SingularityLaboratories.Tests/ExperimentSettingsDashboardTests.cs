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
            ],
            microBundleRequirements:
            [
                new ExperimentMicroBundleRequirement(
                    7301,
                    "2.1.0",
                    "physics.electrical-thermal.v1",
                    "Provide the electrical-to-thermal Physics capability.")
            ]);

    [Test]
    public void Dashboard_SeparatesDeclaredPhysicsFromObservedBundleExecution()
    {
        var definition = Definition();
        var configured = new ExperimentSettingsDashboard(definition);

        Assert.That(configured.RunId, Is.Null, "An authoring preview has no selected run.");
        Assert.That(configured.PhysicsDependencies, Has.Count.EqualTo(1));
        Assert.That(configured.RequiredMicroBundles, Has.Count.EqualTo(1));
        Assert.That(configured.ObservedMicroBundles, Is.Empty,
            "A declaration alone must not imply that a MicroBundle was loaded or executed.");

        var runId = Guid.NewGuid();
        var observed = new ExperimentSettingsDashboard(
            definition,
            [
                new MicroBundleExecutionObservation(
                    7301, "2.1.0", "physics.electrical-thermal.v1",
                    MicroBundleExecutionStatus.Executing,
                    new DateTimeOffset(2026, 11, 23, 5, 0, 0, TimeSpan.Zero))
            ],
            runId);

        Assert.That(observed.RunId, Is.EqualTo(runId));
        Assert.That(observed.ObservedMicroBundles, Has.Count.EqualTo(1));
        Assert.That(observed.ObservedMicroBundles[0].Status, Is.EqualTo(MicroBundleExecutionStatus.Executing));
        Assert.That(observed.ObservedMicroBundles[0].BundleId, Is.EqualTo(7301));
    }

    [Test]
    public void Dashboard_RejectsRuntimeObservationsWithoutRunIdentity()
    {
        var observation = new MicroBundleExecutionObservation(
            7301, "2.1.0", "physics.electrical-thermal.v1",
            MicroBundleExecutionStatus.Loaded,
            new DateTimeOffset(2026, 11, 23, 5, 0, 0, TimeSpan.Zero));

        Assert.Throws<ArgumentException>(() =>
            new ExperimentSettingsDashboard(Definition(), [observation]));
    }

    [Test]
    public void Definition_SnapshotsPhysicsAndMicroBundleDeclarations()
    {
        var physics = new List<ExperimentPhysicsDependency>
        {
            new("thermal.state", "Thermodynamics", "physics.thermal", "1.0.0", "Resolve thermal state.")
        };
        var bundles = new List<ExperimentMicroBundleRequirement>
        {
            new(7302, "1.0.0", "physics.thermal.v1", "Thermal state provider.")
        };

        var definition = new ExperimentDefinition(
            "thermal", "1.0.0", "Thermal", "physics.thermal", "1.0.0",
            ScientificValidationLevel.EducationalModel,
            ["Constant heat capacity."], ["No phase changes."],
            physicsDependencies: physics,
            microBundleRequirements: bundles);

        physics.Clear();
        bundles.Clear();

        Assert.That(definition.PhysicsDependencies, Has.Count.EqualTo(1));
        Assert.That(definition.MicroBundleRequirements, Has.Count.EqualTo(1));
    }

    [Test]
    public void Dashboard_RejectsDuplicateRuntimeObservations()
    {
        var observation = new MicroBundleExecutionObservation(
            7301, "2.1.0", "physics.electrical-thermal.v1",
            MicroBundleExecutionStatus.Loaded,
            new DateTimeOffset(2026, 11, 23, 5, 0, 0, TimeSpan.Zero));

        Assert.Throws<ArgumentException>(() =>
            new ExperimentSettingsDashboard(Definition(), [observation, observation], Guid.NewGuid()));
    }
}
