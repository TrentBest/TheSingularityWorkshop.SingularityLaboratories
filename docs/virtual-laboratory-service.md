# Virtual Laboratory Service

## Purpose

Singularity Laboratories should make it practical to investigate an idea before spending money, consuming scarce equipment time, or exposing people and facilities to physical risk.

The service promise is:

> **Break it in concept before you try it in reality.**

A scientist, engineer, educator, or research team should be able to discover a laboratory capability, reserve a virtual workspace, assemble a proposed experiment, challenge it with controlled scenarios, inspect the evidence, and decide what deserves a physical test.

This is a domain capability and product direction. It is not a claim that hosted rental, payment, physical equipment control, or research-grade simulation is already operational.

## What is being rented?

The customer rents access to a defined capability and its operating envelope, not ownership of scientific truth.

A virtual laboratory offering can bundle:

- A laboratory workspace with a declared discipline, purpose, and supported workflows.
- Versioned models and their documented validation levels.
- Virtual apparatus, instruments, sensors, and measurement channels.
- Compute, memory, storage, and execution time allowances.
- Private experiment configurations, run history, and reproducibility records.
- Collaboration seats, expert review, or instructor support where offered.
- Optional access to a physical facility or human-operated instrument, only when a separately governed provider offers it.

A listing must distinguish a purely virtual model, a digital representation calibrated against physical measurements, a remotely operated physical instrument, and a hybrid service. These are materially different products and must never be presented as interchangeable.

## Customer journey

1. **Discover** — browse laboratories by discipline, capability, model fidelity, instruments, validation evidence, and supported scale.
2. **Evaluate** — inspect assumptions, limitations, safety disclosures, dependencies, sample runs, resource estimates, and pricing basis before committing.
3. **Reserve** — request a time-bounded workspace or compute allocation. A reservation is a commercial/host operation, not a scientific run.
4. **Prepare** — create or import an experiment configuration, select model versions, define objectives, and record acceptance criteria.
5. **Preflight** — validate schemas, dependencies, parameter ranges, resource budgets, safety disclosures, and admission policy before execution.
6. **Challenge** — execute baseline, boundary, sensitivity, and explicitly selected failure scenarios. Record every configuration and outcome independently.
7. **Analyze** — compare observations, uncertainty, deviations, resource use, and model limitations. Identify where evidence is insufficient.
8. **Decide** — revise, reject, repeat, request expert review, or prepare a physical validation plan.
9. **Export** — retain a reproducible evidence package containing exact versions, configurations, run records, assumptions, limitations, and analysis references.

The system should make it easy to discover that an idea fails cheaply and early. A failed virtual run is useful evidence when its conditions and limitations are preserved.

## The experiment campaign

A single run answers one configured execution. A campaign organizes a question and a deliberate set of runs.

A campaign should eventually describe:

- **Objective:** the question, engineering decision, or behavior under investigation.
- **Baseline:** the reference configuration and expected observations.
- **Factors:** parameters or environmental conditions varied between runs.
- **Scenario set:** nominal, boundary, sensitivity, stress, and failure-oriented cases.
- **Acceptance criteria:** measurable conditions for success, failure, or inconclusive results.
- **Constraints:** model-validity envelope, safety boundaries, resource ceilings, and prohibited uses.
- **Run matrix:** each scenario's exact configuration and model identity.
- **Analysis:** comparisons, uncertainty, deviations, and unresolved questions.
- **Disposition:** accepted for next-stage review, revise and repeat, rejected, or inconclusive.

Campaigns must not silently mutate a completed run. A changed parameter, model, experiment definition, or acceptance criterion creates a new version or run relationship with explicit lineage.

### Current implementation boundary

The first domain contracts now exist for immutable campaign definitions and individual scenarios. A campaign requires one baseline, at least one additional scenario, unique scenario identifiers, measurable acceptance criteria, and configurations bound to the exact experiment identity and version. These contracts describe a campaign; they do not yet execute scenario matrices, generate probes, persist campaign history, or provide hosted reservations.

## Scenario design: how to break an idea

Scenario generation must be transparent and bounded. The platform should support deliberate probes such as:

- **Nominal case:** the intended operating point.
- **Boundary case:** values at or near declared model and parameter limits.
- **Sensitivity case:** controlled changes to one or more inputs to reveal influential factors.
- **Tolerance case:** declared manufacturing, measurement, or environmental variation.
- **Disturbance case:** a documented perturbation such as a changed load or ambient condition.
- **Failure case:** a modeled fault or degraded component, only where the model explicitly supports it.
- **Adversarial case:** an intentionally unfavorable but scientifically meaningful combination of supported conditions.

A scenario is not valid merely because it is dramatic. Each one needs a rationale, source for its parameter bounds, model support statement, and a clear label for whether it is measured, derived, assumed, or hypothetical. Out-of-domain inputs should be rejected or explicitly classified as exploratory extrapolation; they must not be presented as reliable predictions.

## Fidelity and evidence are visible product attributes

Every rentable laboratory/model listing should disclose:

- Model identifier and immutable version.
- Scientific validation level and separate review state.
- Evidence considered, including benchmark or physical-comparison references where available.
- Assumptions, limitations, supported parameter ranges, and unsupported phenomena.
- Numerical method and relevant precision/stability information when supplied by the model.
- Determinism and replay guarantees, including known sources of nondeterminism.
- Resource estimate and whether it is advisory or enforced.
- The intended use class: education, exploratory engineering, research support, or validated operational use.

A reviewer decision does not upgrade the scientific validation level. A model that is educational remains educational even if a host accepts it for a classroom. A virtual result does not certify a real device, material, process, or facility.

## From virtual results to physical experiments

The strongest long-term workflow is a traceable handoff:

1. Preserve the virtual campaign and its unresolved risks.
2. Identify which predictions require physical measurement.
3. Produce a proposed physical test plan with instrumentation needs, sampling, controls, and acceptance criteria.
4. Require independent human review and the physical facility's own safety and authorization process.
5. Record actual physical observations as a distinct evidence source linked to, but never substituted for, the virtual campaign.
6. Compare predicted and measured outcomes; update model evidence through a governed versioned review.

Singularity Laboratories may help prepare and compare a plan. It does not authorize hazardous physical work, operate real equipment by implication, or replace qualified professionals, institutional review, legal requirements, or facility procedures.

## Reservation, tenancy, and operations

The scientific domain describes what a laboratory and experiment mean. Trusted platform services own customer identity, tenancy, access enforcement, scheduling, payment, billing, secrets, and infrastructure isolation.

A future reservation should bind:

- Tenant/workspace reference issued by the host.
- Laboratory and offering version.
- Requested capability and time window.
- Quota or resource allocation.
- Price estimate and billing basis shown before commitment.
- Reservation state and cancellation terms.
- Audit references for acceptance and changes.

Do not use a City character identifier as a tenant identity or permission. A laboratory staff assignment is not a commercial entitlement. A reservation is not an execution admission decision. Execution admission is not a safety certification.

Metering must distinguish reserved capacity from actual consumption, and actual consumption from billable consumption. Show estimates before a run, report measured usage after it, disclose failed-run billing rules, and make cancellation behavior explicit. The host must enforce quotas and isolate workloads; client-side estimates are not enforcement.

## Offering types

| Offering | Intended customer value |
|---|---|
| Public demonstration | Explore a capability and learn its limits at no charge |
| Individual virtual bench | Private saved workspace and a modest run allowance |
| Reserved compute session | Time-bounded access to higher-cost model capacity |
| Research campaign | Multiple scenarios, comparisons, retained evidence, and exports |
| Instrument/model specialist | Access to a specialized capability with clear dependencies and support |
| Institutional laboratory | Managed workspaces, teams, governance, and retention controls |
| Hybrid validation service | Virtual preparation plus a separately contracted physical measurement service |

These are candidate offering shapes, not commitments to price, service levels, or availability.

## Marketplace and creator economy

Researchers, educators, instrument specialists, and developers may eventually publish laboratory offerings, model adapters, instruments, scenario packs, and lessons. A listing must include version, ownership, license, dependencies, supported model versions, evidence disclosures, safety metadata, resource profile, maintenance status, and compatibility.

Creator payments, commissions, subscriptions, refunds, tax handling, and entitlements belong to Economy and trusted platform services. Scientific review and version provenance remain laboratory-domain concerns. Payment must never buy a favorable scientific assessment.

## Initial acceptance gates

Before calling this a rentable virtual laboratory service, prove:

- A customer can inspect a complete capability listing and its limitations before reservation.
- A reservation can be created, changed, expired, and cancelled without changing scientific records.
- A tenant workspace is isolated and its authorization is enforced by trusted services.
- A campaign can define baseline and scenario configurations with stable lineage.
- Every run records exact model, experiment, configuration, validation disclosures, and result provenance.
- Unsupported and out-of-domain scenarios are clearly rejected or labeled as exploratory.
- Resource estimates are separated from enforced runtime metering.
- A reproducibility export can be independently inspected.
- The handoff to physical testing is clearly advisory and requires external qualified authorization.

## Related domain documents

- [Architecture](../ARCHITECTURE.md)
- [Commercial model](../COMMERCIAL-MODEL.md)
- [Governance and security](../GOVERNANCE-AND-SECURITY.md)
- [Scientific integrity](../SCIENTIFIC-INTEGRITY.md)
- [Reproducibility and open science](reproducibility-and-open-science.md)
- [Roadmap](../ROADMAP.md)
