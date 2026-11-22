# Roadmap

Each phase should produce a working capability and verifiable evidence. The platform is the laboratory abstraction and tooling; host worlds and city-scale presentation are separate integrations.

## Phase 0 — Repository and domain foundation

- [x] Establish repository identity and architectural boundaries.
- [x] Document laboratory, experiment, scientific-integrity, and commercial concepts.
- [x] Separate portable laboratory domain from Workshop and Singularity City host experiences.
- [x] Inspect Physics and identify concrete reusable simulation capabilities. The existing electrical-to-lumped-thermal coupling fixture is selected.
- [x] Inspect FSM_COS, FSM_Execution, Renderer, and Visualization integration contracts. FSM_COS composes MicroBundles; FSM_Execution owns workflow execution; Renderer and Visualization remain presentation capabilities.
- [x] Define the first experiment and acceptance criteria.

## Phase 1 — First living laboratory vertical slice

- [x] Establish initial versioned laboratory/experiment/configuration contracts, model adapter boundary, run provenance, and portable facility/instrument manifest contracts.
- [x] Implement and verify one end-to-end Physics-backed experiment through FSM_Execution. The actual Physics adapter runs through the registered FSM_Execution workflow handler; CI passed 5 focused Physics tests and 19 laboratory tests, with all three adapter builds clean.
- [x] Implement a typed laboratory catalog MicroBundle exposing immutable laboratory and facility definitions through its loaded provider instance.
- [x] Integrate the laboratory MicroBundle through an FSM_COS composition manifest and verify provider resolution from the resulting RuntimeAssembly. CI explicitly builds FSM_COS against the repository-source dependency graph and exercises the composition test.
- [x] Add durable run-record storage and retrieval. `Laboratory.Persistence` supplies an atomic filesystem implementation of `IExperimentRunStore`; the FSM_Execution adapter can persist each completed run when a host supplies the store. Round-trip, duplicate-ID protection, missing-run, and identity validation tests cover the storage contract.
- [ ] Complete re-execution replay and resource-profile acceptance. A stored record preserves the exact configuration and model/experiment versions, but resolving that historical model version and re-running it as a new linked run, plus resource budgets, remain.
- [ ] Prove that the same laboratory can be surfaced by more than one host without changing its scientific identity.
- [ ] Define a minimal game-host adapter for facilities, apparatus interactions, experiment events, and discoverable research records.
- [x] Demonstrate deterministic model behavior and documented limitations through the Physics fixture tests and adapter tests.

**Exit gate:** an author can configure a laboratory and experiment; a learner or researcher can run it, inspect results, and reproduce the run from its recorded configuration; a host can present it without owning its domain state.

## Phase 2 — Laboratory authoring toolkit

- [x] Establish versioned laboratory, facility, instrument, apparatus, measurement-channel, and experiment definitions.
- [ ] Add parameter schemas, result views, and reusable authoring templates.
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

Selected: `ElectricalThermalCouplingExperiment` from Physics `development`. It composes an ideal fixed-voltage resistive load with a lumped constant-specific-heat thermal body. The absorbed fraction is explicit input; the fixture is educational, not a validated appliance/material prediction. The adapter uses the existing source project in CI rather than assuming an unpublished NuGet package. The complete author → configure → validate → run → measure → analyze → reproduce lifecycle remains the acceptance target.
