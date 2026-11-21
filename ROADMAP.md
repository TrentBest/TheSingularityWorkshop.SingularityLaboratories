# Roadmap

Each phase should produce a working capability and verifiable evidence. The platform is the laboratory abstraction and tooling; host worlds and city-scale presentation are separate integrations.

## Phase 0 — Repository and domain foundation

- [x] Establish repository identity and architectural boundaries.
- [x] Document laboratory, experiment, scientific-integrity, and commercial concepts.
- [x] Separate portable laboratory domain from Workshop and Singularity City host experiences.
- [ ] Inspect Physics and identify concrete reusable simulation capabilities.
- [ ] Inspect FSM_COS, FSM_Execution, Renderer, and Visualization integration contracts.
- [ ] Define the first experiment and acceptance criteria.

## Phase 1 — First living laboratory vertical slice

- [ ] Build laboratory authoring/configuration and runtime foundations.
- [ ] Implement one end-to-end experiment using verified Physics capabilities.
- [ ] Capture configuration, units, model version, run state, observations, and results.
- [ ] Present the laboratory through MicroBundle composition.
- [ ] Prove that the same laboratory can be surfaced by more than one host without changing its scientific identity.
- [ ] Define a minimal game-host adapter for facilities, apparatus interactions, experiment events, and discoverable research records.
- [ ] Demonstrate repeatability and documented limitations.

**Exit gate:** an author can configure a laboratory and experiment; a learner or researcher can run it, inspect results, and reproduce the run from its recorded configuration; a host can present it without owning its domain state.

## Phase 2 — Laboratory authoring toolkit

- [ ] Establish versioned laboratory, facility, instrument, and experiment definitions.
- [ ] Add reusable apparatus, instruments, parameter schemas, measurement channels, and result views.
- [ ] Add validation states, safety metadata, and scientific provenance.
- [ ] Add save, compare, export, and replay workflows.
- [ ] Establish model-specific resource profiles, budgets, and cancellation.
- [ ] Add reusable templates so new labs do not require bespoke application code.

## Phase 3 — Education and online classes

- [ ] Add guided lessons and instructor-authored experiment sequences.
- [ ] Support instructor and learner roles, assignments, and individual/shared runs.
- [ ] Add class demonstrations, learner workspaces, observations, and submission evidence.
- [ ] Support progression from observation to independent investigation.
- [ ] Integrate optional identity, roster, scheduling, and collaboration services through host boundaries.
- [ ] Include accessible explanations, units, assumptions, and uncertainty in every lesson.

## Phase 4 — Research and collaboration

- [ ] Add research workspaces, hypotheses, experiment variants, and parameter studies.
- [ ] Add batch execution, checkpointing, and long-running jobs.
- [ ] Add reviewed model catalogs and reproducible research exports.
- [ ] Add dataset lineage, comparison, annotation, and collaboration.
- [ ] Establish expert review and publication workflows for high-consequence models.

## Phase 5 — Host integrations, games, and laboratory complexes

- [ ] Expose laboratories directly in The Singularity Workshop.
- [ ] Define stable host contracts for external experiences.
- [ ] Integrate with Singularity City as a separate host for diegetic laboratory buildings and complexes.
- [ ] Support game integrations that compose laboratories into levels, research facilities, puzzles, quests, and narrative environments.
- [ ] Demonstrate a game-facing laboratory with reusable apparatus, experiment state, and discoverable records; keep game-specific rules and presentation in the game.
- [ ] Ensure city presentation, spatial layout, and social features remain optional host concerns.
- [ ] Support local, classroom, institutional, private, and hosted laboratory deployments.

## Phase 6 — Hosted laboratories and monetization

- [ ] Define compute metering, budgets, quotas, and pre-run estimates.
- [ ] Integrate identity, entitlements, persistence, and hosted sessions through platform boundaries.
- [ ] Introduce paid private laboratories and institutional offerings.
- [ ] Add creator publishing and licensing only after governance is operational.

## First experiment selection

Choose the first experiment only after inspecting the actual Physics package; do not invent a dependency contract. Prefer a bounded experiment demonstrating the complete author → configure → validate → run → measure → analyze → reproduce lifecycle. The first vertical slice matters more than the spectacle of the first demo.
