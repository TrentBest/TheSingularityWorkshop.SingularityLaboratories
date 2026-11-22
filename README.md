<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Singularity Laboratories

**The Workshop's complete laboratory, experimentation, education, and research platform — from the subatomic to the astronomical.**

Singularity Laboratories provides the abstractions, authoring tools, runtime workflows, instruments, and scientific services for creating and operating laboratories. It is where people configure experiments, investigate models, collect measurements, teach scientific methods, conduct research, and share reproducible work.

**Singularity Laboratories is not Singularity City.** The laboratory platform is exposed directly within The Singularity Workshop and can also be manifested as laboratory facilities and complexes inside Singularity City. Singularity City is a separate world/experience that may host laboratories; it does not own or define the laboratory system. The same laboratory should be usable in a Workshop page, a city facility, an online class, a private research workspace, or another compatible host.

## 1. What This Package Owns

This repository is everything about the laboratory abstraction and its tooling:

- **Create and configure:** define laboratories, facilities, experiment templates, instruments, apparatus, parameters, materials, environments, and workflows.
- **Operate:** initialize, validate, execute, pause, resume, cancel, checkpoint, and reproduce experiments.
- **Measure:** configure instruments, sensors, sampling, observation channels, units, uncertainty, and data capture.
- **Teach:** create guided labs, assignments, instructor-led demonstrations, classroom sessions, and learner progression.
- **Research:** compare hypotheses and runs, manage provenance, collaborate, analyze results, and publish reproducible work.
- **Extend:** compose new disciplines, instruments, models, lessons, and lab experiences from versioned reusable components.
- **Host and govern:** support local, classroom, institutional, private, and hosted laboratory modes with explicit resource and access boundaries.

A laboratory may investigate a molecular process, particle detector, collider, material specimen, ecosystem, planetary system, or solar-system-scale phenomenon. Extreme visual and physical scales are first-class design considerations, but model fidelity and compute cost must always be explicit.

## 2. The Laboratory Is Portable

The laboratory definition and its scientific identity must not depend on the environment in which it appears.

| Host | How it uses Singularity Laboratories |
|---|---|
| The Singularity Workshop | Direct discovery, authoring, lessons, experiments, and research tools |
| Singularity City (separate project) | Physical/diegetic manifestation: laboratory buildings, campuses, complexes, public exhibits |
| Games and interactive fiction | Reusable research facilities, experiment mechanics, apparatus, puzzles, environmental storytelling, and scientifically grounded interactions |
| Online classroom | Instructor-assigned labs, shared sessions, learner workspaces, assessment |
| Private research environment | Restricted labs, controlled collaboration, reproducible runs |
| Hosted service | Remote execution, storage, metering, institutional and premium access |

A host can provide navigation, social presence, avatars, world geometry, narrative, game rules, and presentation. It must not become the source of truth for experiment definitions, scientific models, measurement semantics, or run provenance. A game may adapt a laboratory into a level, quest, puzzle, or story location while reusing the same underlying laboratory and experiment capabilities.

## 3. Games and Interactive Experiences

Laboratories are common and versatile game settings. They can function as research hubs, abandoned facilities, containment zones, experimental test sites, puzzle spaces, exploration environments, or narrative locations. A survival-horror experience in the spirit of *Resident Evil*, for example, might present interconnected research wings, access-controlled rooms, apparatus puzzles, specimen records, experiment logs, containment failures, and environmental clues.

Singularity Laboratories should provide reusable laboratory capabilities that games can compose: facility and room manifests, apparatus interactions, instrument behaviors, experiment state, observations, records, and model-backed phenomena. The game remains responsible for pacing, combat, quests, characters, level design, and fictional framing. A game-facing adapter must not turn game-specific assumptions into the scientific source of truth. Fictional or deliberately simplified behavior must be labeled as such.

This makes a laboratory usable in a classroom, research workspace, Workshop page, or game without requiring separate copies of its underlying domain definition.

## 4. Scientific Domains

| Domain | Example laboratory capabilities |
|---|---|
| Particle physics | Beam and collider studies, detector configuration, event visualization, field experiments |
| Chemistry | Molecular modeling, reaction studies, thermodynamics, safe energetic-process simulation |
| Nuclear science | Fission and fusion models, radiation transport, shielding, consequence studies |
| Astronomy and cosmology | Orbital systems, stellar evolution, planetary science, galaxies, large-scale structure |
| Engineering | Materials, structures, fluids, heat transfer, mechanical systems |
| Earth and environmental science | Atmospheres, oceans, climate processes, impact and hazard scenarios |
| Biology and ecology | Cells, populations, ecosystems, interacting biological systems |

## 5. Architecture

- **Physics** owns mathematical and physical models and numerical methods.
- **FSM_API** provides state and interaction foundations where appropriate.
- **FSM_COS** composes laboratory experiences from MicroBundles.
- **FSM_Execution** executes composed experiences and owns workflow lifecycle.
- **Renderer** and **Visualization** are consumed through the MicroBundle architecture.
- **ServerApp** may provide optional identity, persistence, collaboration, and hosted sessions.
- **Economy** may provide optional commerce, entitlements, and virtual-economy integration.
- **Singularity City** is a separate host experience and must integrate through stable laboratory-facing contracts.

The laboratory layer owns laboratory/experiment definitions, orchestration, instruments, measurement workflows, educational scaffolding, research workflows, provenance, result presentation, authoring tools, and laboratory-specific commercial packaging. It must not duplicate core physics or invent unverified Physics APIs.

See [Architecture](ARCHITECTURE.md), [Laboratory Model](LABORATORY-MODEL.md), and [Roadmap](ROADMAP.md).

## 6. Initial Runtime and Composition Foundation

The first implementation slice lives in `src/SingularityLaboratories` and is deliberately host-neutral:

- Versioned laboratory and experiment definitions.
- Explicit model identity/version, validation level, assumptions, and limitations.
- Versioned, immutable configuration snapshots.
- An experiment-model adapter contract and asynchronous model invocation.
- An FSM_Execution workflow adapter that lets FSM_Execution own workflow lifecycle, cancellation, and timeout semantics.
- A Physics adapter that invokes the existing `ElectricalThermalCouplingExperiment` without duplicating its equations.
- Immutable run records that retain the exact configuration, model provenance, scientific disclosures, timestamps, and serialized result.
- A laboratory MicroBundle adapter that exposes immutable laboratory and facility catalogs through a typed provider interface.

The core does not implement scientific equations. `Laboratory.PhysicsAdapter` invokes the existing Physics `ElectricalThermalCouplingExperiment` implementation and returns its unit-explicit observation through `IExperimentModel`. `Laboratory.ExecutionAdapter` registers that capability as a host-approved FSM_Execution workflow handler; it does not replace FSM_Execution's lifecycle coordinator. The initial electrical-to-lumped-thermal candidate is an **educational model**, not a validated appliance or material prediction. Its declared absorbed fraction is an input; it is not a predicted efficiency.

The MicroBundle provider declaration is metadata. A composition host resolves the loaded `IMicroBundle` instance and explicitly casts it to `ILaboratoryCatalogProvider`; the neutral MicroBundle load context is not a service locator. The adapter references MicroBundleDomain source rather than assuming an unpublished package is available.

For a local cross-repository build, check out the matching sources into the ignored dependency directory, then run the tests:

```sh
mkdir -p dependencies
git clone --branch development https://github.com/TrentBest/TheSingularityWorkshop.FSM_Execution.git dependencies/FSM_Execution
git clone --branch development https://github.com/TrentBest/TheSingularityWorkshop.Physics.git dependencies/Physics
git clone --branch development https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS.git dependencies/FSM_COS
git clone --branch development https://github.com/TrentBest/FSM_API.git dependencies/FSM_API
git clone --branch development https://github.com/TrentBest/FSM_UserIO.git dependencies/FSM_UserIO
git clone --branch development https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain.git dependencies/TheSingularityWorkshop.MicroBundleDomain
dotnet test tests/SingularityLaboratories.Tests/SingularityLaboratories.Tests.csproj --configuration Release
```

The GitHub Actions workflow performs these source checkouts automatically and runs focused Physics tests, the laboratory suite, and all adapter builds.

The optional `Laboratory.Persistence` adapter provides atomic, non-overwriting JSON run storage and retrieval through `IExperimentRunStore`. `LaboratoryWorkflowAdapter.Register` accepts that store optionally and persists completed runs before exposing them as workflow output. `ExperimentRunReplayService` reloads the original configuration, requires the exact laboratory, experiment, and model versions plus unchanged scientific disclosures, then executes a new run linked to its source through `ReplayedFromRunId`. The storage location remains host-owned; this does not introduce a database, hosted service, or mandatory persistence dependency. The core also defines version-bound `ExperimentResourceProfile` estimates (execution time, peak memory, and compute units) and host-owned `ExecutionResourceBudget` ceilings. The runner rejects mismatched model profiles and estimates that exceed the supplied budget before invoking the model. These are admission estimates, not runtime metering or enforced process isolation; hosts remain responsible for selecting defensible estimates and enforcing actual resource limits. `ExperimentResultView` provides a host-neutral, read-only projection over the authoritative immutable run record. It exposes run, laboratory, experiment, model, validation, timing, result-content, and scientific-disclosure metadata without introducing UI, renderer, or world dependencies and without duplicating the result payload. Workshop, Singularity City, and game hosts can build their own presentation adapters over this shared contract. This is still a foundation, not yet the complete authoring, instrument, classroom, research, or hosted runtime platform.

## 7. Scientific Integrity and Participation

A compelling visualization is not automatically a scientifically valid result. Every experiment must make its model, assumptions, units, parameter ranges, numerical limits, provenance, and uncertainty discoverable. Educational approximations, exploratory models, validated models, and research-reviewed models must be distinguishable.

Hazardous phenomena may be explored as educational simulations of physical processes and consequences, not as actionable construction or operational guidance. High-consequence claims require appropriate validation and expert review.

This repository follows the Workshop's two-branch policy: `master` represents the released state and `development` is the active source branch. Work begins on `development`; release alignment and publication remain explicit, gated activities.

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="240" />
</p>
