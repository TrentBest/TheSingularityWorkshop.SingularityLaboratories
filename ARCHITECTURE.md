# Architecture

Singularity Laboratories is the experience and platform layer for scientific experimentation. It owns laboratory discovery, experiment definitions, orchestration, instruments, measurement workflows, education, result presentation, and commercial packaging. It does not replace the scientific engine.

## Responsibility boundaries

| System | Responsibility |
|---|---|
| Physics | Mathematical and physical models, numerical methods, simulation primitives |
| FSM_API | State and interaction foundations |
| FSM_COS | Compose laboratory experiences from MicroBundles |
| FSM_Foundry | Execute composed experiences |
| Renderer | Rendering experience, consumed as a MicroBundle |
| Visualization | Visual, spatial, pixel/world representation, consumed as a MicroBundle |
| ServerApp | Optional persistence, identity, hosted sessions, collaboration |
| Economy | Optional commerce and virtual-economy integration |

Keep scientific computation in Physics. Do not invent Physics APIs before inspecting the package. Renderer and Visualization are MicroBundle dependencies, not ordinary NuGet consumption dependencies. Local educational experiments should not require ServerApp or Economy.

## Logical layers

1. **Laboratory City:** discovery, navigation, facilities, public demonstrations, classrooms, private research spaces.
2. **Laboratory Experience:** domain-specific environment, instruments, controls, guidance, presentation.
3. **Experiment Runtime:** lifecycle, configuration, execution, pause/resume, cancellation, checkpoints, result capture.
4. **Scientific Model Adapter:** narrow boundary to Physics with explicit units, model versions, and supported parameter contracts.
5. **Execution and Composition:** FSM_COS and FSM_Foundry with Renderer and Visualization MicroBundles.
6. **Platform Services:** optional identity, persistence, collaboration, storage, metering, entitlements, commerce.

## Experiment lifecycle

Draft → Configured → Validated → Queued → Running → Completed. Failed and Cancelled are terminal outcomes with retained reasons and diagnostics. A run preserves the exact experiment definition and configuration used; editing a definition never silently rewrites historical results.

## Scale and compute

Visual scale is not computational scale. A solar-system-sized scene may use analytical or reduced-order models; a tiny particle interaction may be computationally expensive. Each experiment declares model fidelity, supported scale, and resource profile.

Support unit-aware quantities, coordinate frames, explicit time and spatial scales, deterministic replay where permitted, bounded budgets, cancellation, checkpointing, and model-specific precision and stability reporting.

## Extension model

Laboratories, instruments, experiment templates, lessons, and visualizations should be independently composable through versioned contracts and declarative manifests. Manifest format and Physics adapter APIs remain open until verified against existing repositories.
