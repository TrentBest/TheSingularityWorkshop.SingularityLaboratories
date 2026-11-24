# Repository Work Queue

**Canonical task ledger:** this file is the single repository-level view of implementation status. The phase roadmap remains the long-range product direction; GitHub Issues hold detailed acceptance criteria and cross-repository coordination.

**Active branch:** `development`  
**Release-aligned branch:** `master`  
**Last status review:** 2026-11-24 (UTC)

## Status vocabulary

- **Implemented** — code or documentation exists in the repository. This does not imply CI verification, production integration, deployment, or release.
- **Verified** — the stated acceptance evidence has passed in a named CI run or repeatable test.
- **In progress** — implementation has started, but acceptance is incomplete.
- **Planned** — no complete implementation exists yet.
- **Blocked** — progress depends on an external host, decision, permission, or missing capability. State the blocker explicitly.

Keep these states separate. A feature may be implemented but unverified. Never check off a task merely because a contract or mock exists when its acceptance criteria require a live host integration.

## Current implementation

### Domain, experiments, and execution

- [x] Versioned laboratory, facility, instrument, apparatus, measurement-channel, experiment, and configuration contracts.
- [x] Physics adapter boundary and an end-to-end electrical-to-thermal experiment through FSM_Execution.
- [x] Laboratory catalog MicroBundle and FSM_COS composition integration.
- [x] Experiment execution records, atomic filesystem run persistence, result catalog, and version-locked replay.
- [x] Scientific experiment campaigns: immutable version-bound definitions, baseline/scenario execution, failure/cancellation/not-run outcomes, workflow handler, and atomic campaign-envelope persistence.
- [x] Model resource profiles and pre-run budget admission. Estimates are advisory; this is not runtime metering or process isolation.
- [x] Safety metadata, version-bound validation assessments, execution admission policy, and structured denial reasons.
- [x] Authoring templates, schema validation, comparison, compatibility assessment, and integrity-checked export/import.
- [x] Host-neutral experiment settings dashboard contract. Runtime observations require a concrete RunId and remain separate from declared dependencies.
- [x] Typed NIGHT SIGNAL narrative definition and immutable state machine, including fixed opening, source-backed evidence, current-evidence gate, route/evidence consistency, terminal outcomes, and append-only event history.
- [x] Durable NIGHT SIGNAL attempt persistence: atomic filesystem store, exact definition/evidence/event round-trip, duplicate-ID protection, identity checks, and validated terminal restoration. The NUnit persistence tests passed in .NET workflow run #388.
- [x] NIGHT SIGNAL FSM_Execution adapter: timestamped evidence/death/escape commands, JSON-safe attempt output, persisted terminal state, and active-snapshot preservation on cancellation. Workflow integration tests and all adapter builds passed in .NET workflow run #404.
- [x] NIGHT SIGNAL facility room/apparatus/instrument/channel binding: all six stable narrative location IDs map to unique version-bound `LaboratoryRoom` IDs; apparatus, fictional state instruments, and on-demand state channels are validated against their parent identities. Campaign/facility identity, room existence, equipment ownership, and fixed-start consistency are tested (full .NET workflow run #433).
- [x] NIGHT SIGNAL staff-role binding: the fixed campaign player role maps to a host-supplied `LaboratoryStaffAssignment`; exact laboratory identity, required role, and active interval are validated at a host-supplied UTC instant. This is responsibility metadata, not authentication or authorization (full .NET workflow run #443).
- [x] Browser-only NIGHT SIGNAL playable specimen and local event export. This is a prototype, not a runtime-backed execution.

### Documentation and experience

- [x] Core Laboratory Manual and Technical Companion.
- [x] Three declared SURPS-oriented sourcebooks with book metadata, reading paths, cover art, and living diagrams.
- [x] Pages source with capability atlas, campaign-draft workbench, fictional campus explorer, and NIGHT SIGNAL browser prototype.
- [x] Documentation validator for sourcebook structure, local links, HTML anchors, diagrams, SVG accessibility metadata, canonical work-queue sections, and README/docs-index work-queue links.
- [x] GitHub Actions workflows for .NET validation, documentation validation, and Pages deployment source.
- [ ] First live Pages deployment — workflow/source exists; repository Pages setting still needs enabling and deployment verification.

## Active work — next in execution order

1. **NIGHT SIGNAL live facility-status adapter — in progress.** Stable room, apparatus, fictional instrument, measurement-channel, and staff-role bindings are implemented and CI verified. Next project host-owned facility status snapshots onto these identities; authentication, authorization, and geometry remain host-owned.
2. **Host dashboard observation projection — [Issue #5](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/issues/5).** Project trustworthy host observations into the existing dashboard and separate timeline. ServerApp has an optional RunId/RuntimeId-correlated observer seam in PR #22; its CI was reported passing by the host repository. Loaded membership is not provider invocation. Per-provider dispatch instrumentation remains a separate evidence gate. Keep the Laboratories core free of ServerApp dependencies.
3. **SURPS living-book adoption — [Issue #6](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/issues/6).** Finish the dual-adapter authoring standard, authoritative domain ownership/provenance, worked mechanics, meaningful living illustrations, and independent SURPS review. Current books are authored and structured; do not call the repository enrolled or independently approved.
4. **Pages activation.** Once the owner enables GitHub Pages with GitHub Actions as source, verify a successful deployment and update the site status. Do not change repository Pages settings without explicit owner authorization.

## Planned platform work

### Runtime, governance, and reliability

- [ ] Runtime resource metering/enforcement, measured resource history, cancellation integration, and evidence-based model estimates.
- [ ] Durable staff assignment storage, change history, revocation, and separation-of-duty policy.
- [ ] Fine-grained authorization scopes for laboratories, facilities, apparatus, experiments, datasets, runs, and publications.
- [ ] Demonstrate the same laboratory through multiple hosts without changing scientific identity.
- [ ] Cross-store transaction/recovery policy for campaign envelopes and individual run records.
- [ ] Host-backed tenant isolation, protected audit, retention/deletion, sandboxing, and incident response.

### Education and research

- [ ] Guided lessons, instructor-authored sequences, learner workspaces, assignments, and accessible progression.
- [ ] Research workspaces, hypothesis/variant studies, batch execution, checkpointing, and long-running jobs.
- [ ] Dataset lineage, comparison, annotation, collaboration, reproducible exports, and expert review/publication.

### Hosts and experiences

- [ ] Minimal game-host adapter for facilities, apparatus interactions, experiment events, and discoverable research records.
- [ ] Workshop and Singularity City host integrations, preserving the portable laboratory identity.
- [ ] Customer-facing capability catalog with model fidelity, validation evidence, assumptions, limits, dependencies, and resource estimates.
- [ ] Reservation lifecycle, tenant workspaces, quotas, metering, billing reconciliation, and virtual-to-physical handoff governance.
- [ ] Connect the browser campaign prototype to a trusted host only after the C# persistence and workflow boundaries are complete.

## Cross-repository coordination

- **ServerApp:** optional observation seam is implemented in PR #22 according to the ServerApp CI report. Follow up on per-provider invocation evidence before projecting providers as Executing/Completed.
- **SURPS:** canonical living-book and adapter standards are upstream. This repository's participation manifest currently reports `integrated-not-enrolled`; independent review remains pending.
- **Physics:** owns physical models, equations, numerical methods, and model-specific scientific validity.
- **FSM_COS:** owns composition and authoritative loaded-bundle membership.
- **FSM_Execution:** owns workflow lifecycle, cancellation, and workflow run identity.
- **Hosts:** own identity, authorization, tenancy, enforcement, runtime observations, and provider invocation evidence.

## Verification and release gates

**Latest observed CI snapshot (2026-11-24 UTC):**

- **.NET:** full workflow passed — [run #443](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/actions/runs/45330787760), commit `5f0ed3d0fed81b3d4490a040c188394fec09304d`. This includes narrative persistence round-trip and identity checks, FSM_Execution safe-escape/death/cancellation tests, version-bound room/apparatus/instrument/channel mapping, active/expired/wrong-role staff assignment tests, and successful persistence, workflow, Physics, and MicroBundle builds. Subsequent commits are documentation/status updates.
- **Documentation/SURPS:** passed after the staff-role documentation, README, roadmap, and task-ledger updates — [run #72](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/actions/runs/45330984857), commit `56087c60`. The validator checks the canonical work-queue sections and README/docs-index links, and the workflow runs when `TODO.md` changes.
- **Pages deployment:** latest attempt failed on commit `e888c7bc` — [run #113](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/actions/runs/45330969356). The failure details were not exposed by the available job-log endpoint. The site is not verified live; the repository Pages setting still requires owner enablement and a successful deployment.
- **Branches:** last inventory returned exactly `development` and `master`.

- [x] .NET tests and all adapter builds passed in run #443.
- [x] Documentation/SURPS validation passed after staff-role and task-ledger updates (run #72).
- [ ] Pages deployment verified after Pages is enabled.
- [x] Exactly two active branches remain: `development` and `master` (last checked 2026-11-24 UTC).
- [x] No promotion to `master`, NuGet publication, or release action performed in this workstream.

## Maintenance rule

When implementation changes, update this file, the relevant phase roadmap, and the feature's technical documentation in the same workstream. Record the evidence (test name, CI run, or host integration) that justifies each status change. If a task becomes blocked, state the external dependency instead of leaving an ambiguous unchecked box.
