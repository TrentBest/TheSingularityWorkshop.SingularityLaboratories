# Architecture

Singularity Laboratories is the Workshop's domain platform for creating, configuring, operating, teaching with, and conducting research in laboratories. It owns the laboratory abstraction and the complete tooling lifecycle around experiments. It is not a city, a game world, or a scientific engine.

## Foundational boundary: laboratory platform vs. host world

Singularity Laboratories is exposed directly within The Singularity Workshop. It may also be manifested inside Singularity City, which is a separate project and host experience. Singularity City may provide buildings, campuses, streets, spatial navigation, social presence, and diegetic context for laboratory complexes. It does not own laboratory definitions, experiment semantics, scientific models, instruments, measurement meaning, or run provenance.

A laboratory is portable across hosts. The same versioned laboratory and experiment should be usable through:
- Workshop-native pages and tools.
- A laboratory building or complex inside Singularity City.
- A game level, research facility, puzzle environment, or narrative location.
- Online classes and instructor-led sessions.
- Private research workspaces.
- Local or hosted execution environments.

Host-specific presentation must remain an adapter around the laboratory domain, not a dependency of it. The laboratory must not require a city, avatar, multiplayer session, game engine, narrative, or persistent world to function. A game can expose lab apparatus as interactables and experiment state as puzzle or quest state, but those are game-side projections of reusable laboratory capabilities.

## Responsibility boundaries

| System | Responsibility |
|---|---|
| Singularity Laboratories | Laboratory definitions, authoring/configuration tools, experiment workflows, instruments, measurements, lessons, research, provenance, result interpretation |
| Physics | Mathematical and physical models, numerical methods, simulation primitives |
| FSM_API | State and interaction foundations |
| FSM_COS | Resolve and compose laboratory MicroBundles into a `RuntimeAssembly`; composition is not workflow execution |
| FSM_Execution | Own workflow validation, registered-handler execution, retained run lifecycle, cancellation, timeouts, and structured outcomes |
| Laboratory.ExecutionAdapter | Register laboratory experiment invocation as an explicit FSM_Execution workflow handler; does not reimplement workflow lifecycle |
| Laboratory.PhysicsAdapter | Map a strict, versioned laboratory configuration to an existing Physics model and return unit-explicit observations |
| Laboratory.MicroBundleAdapter | Expose portable laboratory/facility catalogs as a typed MicroBundle provider; no host service locator or presentation dependency |
| Renderer | Rendering experience, consumed as a MicroBundle |
| Visualization | Visual, spatial, pixel/world representation, consumed as a MicroBundle |
| The Singularity Workshop | Direct host and discovery surface for laboratory tools and experiences |
| Singularity City | Separate spatial/diegetic host for laboratory facilities and complexes |
| Games and interactive experiences | Game-specific narrative, level design, quests, player progression, and mechanics that may compose laboratory capabilities |
| ServerApp | Optional persistence, identity, hosted sessions, collaboration |
| Economy | Optional commerce, entitlements, and virtual-economy integration |

Keep scientific computation in Physics. Do not invent Physics APIs before inspecting the package. Renderer and Visualization are MicroBundle dependencies, not ordinary NuGet consumption dependencies. Local educational experiments should not require ServerApp, Economy, or Singularity City.

## Laboratory domain model

The platform should support the following independently versioned concepts:

1. **Laboratory** — a reusable scientific environment with purpose, discipline, model capabilities, supported workflows, instruments, safety/review metadata, and dependencies.
2. **Facility / complex** — an optional grouping and presentation of laboratories. A host world may map a facility to a building, campus, or spatial location.
3. **Experiment** — a defined investigation with a question or objective, model, inputs, procedure, and expected observations.
4. **Apparatus and instruments** — configurable equipment, sensors, measurement channels, controls, and interaction capabilities.
5. **Configuration** — all inputs, initial/boundary conditions, units, and execution settings for a particular run.
6. **Run** — one execution of an immutable experiment/configuration snapshot.
7. **Observation and result** — measured or derived values with units, uncertainty, provenance, diagnostics, and interpretation.
8. **Lesson / course activity** — educational context, instructor instructions, learner tasks, assessment, and progression.
9. **Research workspace** — hypotheses, experiment variants, run comparisons, notes, datasets, collaboration, and reproducibility records.
10. **Publication** — versioned, reviewable distribution of a laboratory, experiment, instrument, or lesson.

## Logical layers

1. **Authoring and administration:** create laboratories, configure facilities, compose instruments and experiments, manage versions and permissions.
2. **Laboratory experience:** domain-specific environment, apparatus, controls, guidance, teaching and research workflows.
3. **Experiment runtime:** lifecycle, validation, execution, pause/resume, cancellation, checkpoints, result capture.
4. **Scientific model adapter:** narrow boundary to Physics with explicit units, model versions, and supported parameter contracts.
5. **Composition and execution:** FSM_COS resolves and loads MicroBundles; FSM_Execution executes host-registered workflow handlers and owns accepted/running/terminal lifecycle state. Renderer and Visualization remain optional MicroBundle presentation capabilities.
6. **Host adapters:** Workshop surfaces, Singularity City facilities, classroom interfaces, and other compatible clients.
7. **Platform services:** optional identity, persistence, collaboration, storage, metering, entitlements, and commerce.

## Experiment lifecycle

Draft → Configured → Validated → Queued → Running → Completed. Failed and Cancelled are terminal outcomes with retained reasons and diagnostics. A run preserves the exact experiment definition and configuration used; editing a definition never silently rewrites historical results.

## Games are a first-class integration

Games may use laboratories as settings, mechanics, and narrative infrastructure: research campuses, underground complexes, abandoned labs, containment facilities, experimental test chambers, or scientific puzzle spaces. A game-facing adapter can map laboratory facilities to levels/rooms, apparatus to interactables, instruments to player tools, experiment lifecycle to scripted or systemic events, and observations/publications to discoverable records. Horror and survival games in the spirit of *Resident Evil* are an obvious example, but the model should also support simulation, adventure, strategy, and educational games.

The platform owns reusable laboratory semantics and model-backed behaviors; the game owns fictional framing, pacing, level geometry, combat, quests, player progression, and game-specific rules. Game state must not silently rewrite scientific run provenance. Where a game deliberately fictionalizes a model, that boundary should be explicit. Games should be able to use a curated subset of laboratory features without taking a dependency on classroom, research, or hosted-service features.

## Education and research are core workflows

An online class is not merely a city tour or a multiplayer feature. The platform must support instructor-authored lab activities, learner-specific configurations and runs, shared demonstrations, controlled parameter ranges, assignment state, observations, assessment evidence, and reproducible submission records. Classroom identity and roster services may be supplied by a host integration, but experiment execution and scientific records belong here.

Research workflows extend the same foundations with hypothesis tracking, parameter sweeps, run comparison, datasets, notes, provenance, collaboration, export, and review. Teaching and research should share the same experiment model rather than becoming unrelated subsystems.

## Scale and compute

Visual scale is not computational scale. A solar-system-sized scene may use analytical or reduced-order models; a tiny particle interaction may be computationally expensive. Each experiment declares model fidelity, supported scale, and resource profile.

Support unit-aware quantities, coordinate frames, explicit time and spatial scales, deterministic replay where permitted, bounded budgets, cancellation, checkpointing, and model-specific precision and stability reporting.

## Extension and portability

Laboratories, facilities, instruments, experiment templates, lessons, and host adapters should be independently composable through versioned contracts and declarative manifests. Host adapters may add spatial placement and presentation without changing the underlying lab definition. The initial implementation includes versioned laboratory/experiment/configuration/run contracts, portable facility/instrument contracts, a strict Physics adapter for the electrical-to-lumped-thermal fixture, an FSM_Execution workflow adapter, and a typed laboratory catalog MicroBundle. The first Physics model is explicitly educational; it is not a validated appliance or material prediction. Parameter schemas, result views, durable replay, resource profiles, host presentation, and reproducible replay acceptance remain open work.
