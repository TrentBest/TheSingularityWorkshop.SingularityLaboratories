using System.Text.Json;
using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Physics;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ElectricalThermalCouplingModelTests
{
    [Test]
    public async Task ExecuteAsync_UsesPhysicsModelAndPreservesUnitsAndLimitations()
    {
        var model = new ElectricalThermalCouplingModel();
        var input = new ElectricalThermalCouplingConfiguration
        {
            BusVoltageVolts = 12d,
            ResistanceOhms = 6d,
            GenerationCapacityWatts = 100d,
            DurationSeconds = 10d,
            AbsorbedPowerFraction = 0.5d,
            InitialMassKilograms = 2d,
            InitialTemperatureKelvin = 300d,
            SpecificHeatCapacityJoulesPerKilogramKelvin = 500d,
            ReferenceTemperatureKelvin = 273.15d
        };
        var configuration = new ExperimentConfiguration(
            model.Definition.Id, model.Definition.Version, 1,
            "application/json", JsonSerializer.Serialize(input));

        var output = await model.ExecuteAsync(configuration);
        var observation = JsonSerializer.Deserialize<ElectricalThermalCouplingObservation>(output.Payload);

        Assert.That(output.ContentType, Is.EqualTo("application/json"));
        Assert.That(observation, Is.Not.Null);
        Assert.That(observation!.TotalCurrentAmperes, Is.EqualTo(2d).Within(1e-12));
        Assert.That(observation.DemandWatts, Is.EqualTo(24d).Within(1e-12));
        Assert.That(observation.ElectricalEnergyJoules, Is.EqualTo(240d).Within(1e-12));
        Assert.That(observation.ThermalEnergyTransferJoules, Is.EqualTo(120d).Within(1e-12));
        Assert.That(observation.EnergyNotTransferredToThermalBodyJoules, Is.EqualTo(120d).Within(1e-12));
        Assert.That(observation.FinalThermalState.TemperatureKelvin, Is.EqualTo(300.12d).Within(1e-10));
        Assert.That(model.Definition.ValidationLevel, Is.EqualTo(ScientificValidationLevel.EducationalModel));
        Assert.That(model.Definition.Limitations, Has.Count.GreaterThanOrEqualTo(4));
        Assert.That(model.Definition.PhysicsDependencies, Has.Count.EqualTo(2));
        Assert.That(model.Definition.PhysicsDependencies.Select(x => x.Domain),
            Is.EquivalentTo(new[] { "Electrical engineering", "Thermodynamics" }));
    }

    [Test]
    public void ExecuteAsync_RejectsUnknownConfigurationProperties()
    {
        var model = new ElectricalThermalCouplingModel();
        var configuration = new ExperimentConfiguration(
            model.Definition.Id, model.Definition.Version, 1,
            "application/json", "{\"unexpectedParameter\":1}");

        Assert.ThrowsAsync<JsonException>(async () =>
            await model.ExecuteAsync(configuration));
    }
}
