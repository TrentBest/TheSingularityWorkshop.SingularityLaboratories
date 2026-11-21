# Architecture

## Purpose

Singularity Laboratories converts scientific capabilities into usable laboratory experiences. It owns the domain model and workflows around experiments; it does not become a competing physics engine, renderer, operating system, or commerce backend.

## Responsibility boundaries

| System | Responsibility | Boundary |
|---|---|---|
| Physics | Mathematical models, physical systems, numerical methods, simulation primitives | Scientific computation belongs here; laboratory UI and commerce do not |
| FSM_API | State and interaction foundations | Use established contracts; do not create a parallel state framework |
| FSM_COS | Compose the laboratory experience from MicroBundles | Owns composition, not scientific truth |
| FSM_Foundry | Execute composed experiences | Owns execution responsibilities, not domain policy |
| Renderer | Textual / AI-side and structured rendering experiences | Consume as a MicroBundle |
| Visualization | Visual, spatial, and pixel/world representation | Consume as a MicroBundle |
| ServerApp | Persistent worlds, identity, hosted sessions, collaboration | Optional integration boundary; do not couple the core experiment model to hosting |
| Economy | Commerce and virtual-economy capabilities | Optional integration boundary; do not embed payment logic in scientific models |

## Logical platform layers

1. **Laboratory City** — discovery, navigation, facility identity, public demonstrations, classrooms, and private research spaces.
2. **Laboratory Experience** — a domain-specific environment containing instruments, controls, guidance, and presentation.
3. **Experiment Runtime** — lifecycle, configuration, execution requests, pause/resume, cancellation, checkpoints, and result capture.
4. **Scientific Model Adapter** — a narrow boundary to Physics capabilities, with explicit units, model versions, and supported parameter contracts.
5. **Execution and Composition** — FSM_COS and FSM_Foundry integration, with Renderer and Visualization supplied as MicroBundles.
6. **Platform Services** — optional identity, persistence, collaboration, storage, metering, entitlements, and commerce integrations.

## Experiment lifecycle

A reproducible experiment should move through explicit states:

- **Draft:** being authored; not yet eligible for publication or paid execution.
- **Configured:** inputs, units, initial conditions, model version, and resource limits are captured.
- **Validated:** schema, parameter bounds, compatibility, and required scientific checks pass.
- **Queued:** execution resources are assigned.
- **Running:** progress and resource consumption are observable.
- **Completed:** outputs and provenance are sealed.
- **Failed / Cancelled:** termination reason and available diagnostics are retained.

A run must preserve the experiment definition and configuration used to produce it. Editing a definition must not silently rewrite historical results.

## Scale and compute

Do not equate visual scale with computational scale. A solar-system-sized scene may use analytical or reduced-order models, while a tiny particle interaction may require expensive numerical work. Each experiment declares its model fidelity, resource profile, and supported scale.

The platform should support:
- unit-aware quantities and coordinate frames;
- explicit time and spatial scales;
- deterministic replay where the underlying model permits it;
- bounded resource budgets and cancellation;
- checkpointing for long-running work;
- model-specific precision and stability reporting;
- progressive fidelity, from educational approximations to validated research configurations.

## Extension model

Laboratories, instruments, experiment templates, lessons, and visualizations should be independently composable. Prefer declarative manifests and versioned contracts over hard-coded domain branching. The exact manifest format and Physics adapter APIs remain design decisions until validated against the existing repositories.

## Non-goals

- Reimplementing Physics inside this repository.
- Treating a visual effect as proof of a scientific result.
- Making Renderer or Visualization ordinary NuGet consumption dependencies.
- Requiring ServerApp or Economy for local/offline educational experiments.
- Promising arbitrary simulation scales without a model and resource budget.
