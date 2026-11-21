using System.Text.Json;
using System.Text.Json.Serialization;
using TheSingularityWorkshop.Physics.Experiments;
using TheSingularityWorkshop.Physics.Thermodynamics;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;
using TheSingularityWorkshop.SingularityLaboratories.Execution;

namespace TheSingularityWorkshop.SingularityLaboratories.Physics;

/// <summary>Strict JSON configuration for the Physics electrical-to-lumped-thermal fixture.</summary>
public sealed record ElectricalThermalCouplingConfiguration
{
    public required double BusVoltageVolts { get; init; }
    public required double ResistanceOhms { get; init; }
    public required double GenerationCapacityWatts { get; init; }
    public required double DurationSeconds { get; init; }
    public required double AbsorbedPowerFraction { get; init; }
    public required double InitialMassKilograms { get; init; }
    public required double InitialTemperatureKelvin { get; init; }
    public required double SpecificHeatCapacityJoulesPerKilogramKelvin { get; init; }
    public required double ReferenceTemperatureKelvin { get; init; }
}

/// <summary>
/// Invokes the authoritative Physics implementation. This is an educational numerical fixture,
/// not a validated appliance, material, or heat-transfer prediction.
/// </summary>
public sealed class ElectricalThermalCouplingModel : IExperimentModel
{
    public const string ExperimentId = "electrical-thermal-coupling";
    public const string ExperimentVersion = "1.0.0";
    public const string PhysicsModelId = "TheSingularityWorkshop.Physics.Experiments.ElectricalThermalCouplingExperiment";
    public const string PhysicsModelVersion = "0.1.0-alpha.1";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    public ExperimentDefinition Definition { get; } = new(
        ExperimentId,
        ExperimentVersion,
        "Electrical to lumped thermal energy coupling",
        PhysicsModelId,
        PhysicsModelVersion,
        ScientificValidationLevel.EducationalModel,
        [
            "The electrical bus is an ideal fixed-voltage DC source.",
            "The load resistance and specific heat capacity are constant.",
            "The absorbed power fraction is an explicit experiment input."
        ],
        [
            "The model does not predict heat-transfer efficiency.",
            "The model does not represent thermal exchange with surroundings or spatial temperature gradients.",
            "The model excludes resistance variation, phase change, startup transients, and real appliance behavior.",
            "The specific heat capacity must be appropriate to the declared educational scenario and temperature range."
        ]);

    public ValueTask<ExperimentModelOutput> ExecuteAsync(
        ExperimentConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(configuration.ExperimentId, Definition.Id, StringComparison.Ordinal) ||
            !string.Equals(configuration.ExperimentVersion, Definition.Version, StringComparison.Ordinal))
            throw new ArgumentException("Configuration targets a different experiment version.", nameof(configuration));
        if (!string.Equals(configuration.ContentType, "application/json", StringComparison.Ordinal))
            throw new ArgumentException("The electrical-to-thermal model requires application/json configuration.", nameof(configuration));

        var input = JsonSerializer.Deserialize<ElectricalThermalCouplingConfiguration>(
            configuration.Payload, JsonOptions)
            ?? throw new ArgumentException("The experiment configuration cannot be null.", nameof(configuration));

        var initial = new LumpedThermalState(
            input.InitialMassKilograms,
            input.InitialTemperatureKelvin,
            input.SpecificHeatCapacityJoulesPerKilogramKelvin,
            input.ReferenceTemperatureKelvin);

        var result = ElectricalThermalCouplingExperiment.Run(
            initial,
            input.BusVoltageVolts,
            input.ResistanceOhms,
            input.GenerationCapacityWatts,
            input.DurationSeconds,
            input.AbsorbedPowerFraction);

        cancellationToken.ThrowIfCancellationRequested();
        var output = new ElectricalThermalCouplingObservation(
            input,
            result.ElectricalObservation.BusVoltageVolts,
            result.ElectricalObservation.GenerationCapacityWatts,
            result.ElectricalObservation.DemandWatts,
            result.ElectricalObservation.TotalCurrentAmperes,
            result.ElectricalObservation.RemainingCapacityWatts,
            result.ElectricalObservation.IsOverloaded,
            result.ElectricalEnergyJoules,
            result.ThermalEnergyTransferJoules,
            result.EnergyNotTransferredToThermalBodyJoules,
            result.InitialThermalState,
            result.FinalThermalState,
            Definition.Assumptions,
            Definition.Limitations);

        return ValueTask.FromResult(new ExperimentModelOutput(
            "application/json",
            JsonSerializer.Serialize(output)));
    }
}

/// <summary>Unit-explicit output envelope from the Physics electrical-to-thermal fixture.</summary>
public sealed record ElectricalThermalCouplingObservation(
    ElectricalThermalCouplingConfiguration Inputs,
    double BusVoltageVolts,
    double GenerationCapacityWatts,
    double DemandWatts,
    double TotalCurrentAmperes,
    double RemainingCapacityWatts,
    bool IsOverloaded,
    double ElectricalEnergyJoules,
    double ThermalEnergyTransferJoules,
    double EnergyNotTransferredToThermalBodyJoules,
    LumpedThermalState InitialThermalState,
    LumpedThermalState FinalThermalState,
    IReadOnlyList<string> Assumptions,
    IReadOnlyList<string> Limitations);
