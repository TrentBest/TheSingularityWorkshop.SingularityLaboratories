# Roadmap

This roadmap is intentionally outcome-driven. Each phase should produce a working experience and verifiable evidence before the next layer is expanded.

## Phase 0 — Repository and domain foundation

- [x] Establish repository identity and architectural boundaries.
- [x] Document laboratory, experiment, scientific-integrity, and commercial concepts.
- [ ] Inspect the current Physics package and identify concrete, reusable simulation capabilities.
- [ ] Inspect FSM_COS, FSM_Foundry, Renderer, and Visualization integration contracts.
- [ ] Define the first experiment and its acceptance criteria.

## Phase 1 — First living laboratory

- [ ] Build a navigable laboratory shell with a clear facility / experiment distinction.
- [ ] Implement one end-to-end experiment using existing Physics capabilities.
- [ ] Capture configuration, units, model version, run state, observations, and results.
- [ ] Present the experiment through the MicroBundle composition model.
- [ ] Demonstrate repeatability and documented limitations.

**Exit gate:** a user can enter the laboratory, configure an experiment, run it, inspect meaningful results, and reproduce the run from its recorded configuration.

## Phase 2 — Reusable laboratory tooling

- [ ] Establish versioned laboratory and experiment definitions.
- [ ] Add reusable instruments, parameter controls, measurement channels, and result views.
- [ ] Add validation states and scientific provenance.
- [ ] Add save, compare, export, and replay workflows.
- [ ] Establish model-specific resource profiles and cancellation.

## Phase 3 — Hands-on education

- [ ] Add guided lessons and instructor-authored experiment sequences.
- [ ] Support learner progression from observation to independent investigation.
- [ ] Add classroom / cohort workflows through optional platform integration.
- [ ] Include accessible explanations, units, and uncertainty in every lesson.

## Phase 4 — Research and collaboration

- [ ] Add private workspaces and collaboration.
- [ ] Add checkpointing and long-running execution support.
- [ ] Add reviewed model catalogs and reproducible research exports.
- [ ] Establish expert review and publication workflows for high-consequence models.

## Phase 5 — Hosted laboratories and monetization

- [ ] Define compute metering, budgets, quotas, and transparent pre-run estimates.
- [ ] Integrate identity, entitlements, and persistent sessions through platform boundaries.
- [ ] Introduce paid private laboratories and institutional offerings.
- [ ] Add creator publishing and licensing only after governance is operational.

## First experiment selection

The first experiment should be chosen after inspecting the actual Physics package, not by inventing a dependency contract. Prefer a bounded experiment that demonstrates the complete lifecycle and produces observable, testable results. The first vertical slice matters more than the spectacle of the first demo.
