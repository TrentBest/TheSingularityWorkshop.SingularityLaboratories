using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

/// <summary>
/// Executable, fictional campus specimen. All instruments and readings are illustrative;
/// this fixture does not represent a real facility or a validated scientific experiment.
/// </summary>
internal static class TeachingResearchCampusFixture
{
    public static LaboratoryCampus Create()
    {
        var teachingLaboratory = new LaboratoryDefinition(
            "teaching-physics",
            "1.0.0",
            "Teaching Physics Laboratory",
            "A supervised environment for introductory measurement and model-based learning.");

        var materialsLaboratory = new LaboratoryDefinition(
            "materials-research",
            "1.0.0",
            "Materials Research Laboratory",
            "A research workspace for comparing material response and measurement provenance.");

        var teachingFacility = new LaboratoryFacility(
            "teaching-hall",
            "1.0.0",
            "Teaching and Demonstration Hall",
            [
                new LaboratoryRoom("teaching-bench-room", "Measurement Benches",
                    "Supervised, repeatable measurement activities.")
            ],
            [
                new LaboratoryApparatus("calorimetry-station", "Insulated Calorimetry Station",
                    "thermal-measurement-apparatus", "teaching-bench-room",
                    "Illustrative apparatus for observing temperature change in a bounded lesson.")
            ],
            [
                new LaboratoryInstrument("teaching-temperature-probe", "Temperature Probe",
                    "temperature-sensor", "calorimetry-station", "1.0.0",
                    "SAMPLE-CAL-TEACHING-001")
            ],
            [
                new MeasurementChannel("teaching-temperature", "Sample Temperature",
                    "teaching-temperature-probe", "thermodynamic-temperature", "K",
                    MeasurementSamplingMode.Periodic, 1d)
            ]);

        var researchFacility = new LaboratoryFacility(
            "materials-research-hall",
            "1.0.0",
            "Materials Characterization Hall",
            [
                new LaboratoryRoom("mechanical-characterization-room", "Mechanical Characterization",
                    "Controlled observation of material response.")
            ],
            [
                new LaboratoryApparatus("instrumented-test-frame", "Instrumented Test Frame",
                    "mechanical-test-apparatus", "mechanical-characterization-room",
                    "Fictional test-frame manifest; no operating procedure or force limits are implied.")
            ],
            [
                new LaboratoryInstrument("research-load-sensor", "Load Sensor",
                    "force-sensor", "instrumented-test-frame", "1.0.0",
                    "SAMPLE-CAL-RESEARCH-001")
            ],
            [
                new MeasurementChannel("research-force", "Applied Force",
                    "research-load-sensor", "force", "N",
                    MeasurementSamplingMode.OnDemand)
            ]);

        return new LaboratoryCampus(
            "singularity-teaching-research-campus",
            "1.0.0",
            "Singularity Teaching and Research Campus",
            "A fictional, compact campus specimen connecting supervised education and materials research.",
            [teachingLaboratory, materialsLaboratory],
            [teachingFacility, researchFacility],
            [
                new LaboratoryCampusWorkspace(
                    "teaching-physics-workspace", teachingLaboratory.Id,
                    teachingFacility.Id, "teaching-bench-room"),
                new LaboratoryCampusWorkspace(
                    "materials-research-workspace", materialsLaboratory.Id,
                    researchFacility.Id, "mechanical-characterization-room")
            ]);
    }
}
