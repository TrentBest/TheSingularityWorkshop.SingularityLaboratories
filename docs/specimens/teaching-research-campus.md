![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Teaching and Research Campus — first facility specimen

**Domain:** Singularity Laboratories  
**Level:** LOD 3 facility composition  
**Fidelity:** Original, fictional teaching-and-research campus  
**Status:** Implementation specification; not yet a runtime facility or executable demo

This specimen is the first implementation target for the flagship-facility sourcebook. It is deliberately fictional so the Workshop can demonstrate a complete, coherent campus without claiming to reconstruct a real institution or exposing sensitive site details.

The campus must prove that one portable laboratory definition can be hosted inside a larger institutional experience. The facility composes laboratories, people-in-context, instruments, education, research, public interpretation, and operations. It does not replace their domain owners.

## 1. Demonstration goals

A visitor should be able to understand the campus at a glance, enter public spaces, discover what each research group investigates, and observe clearly labeled demonstrations.

A learner should be able to join a supervised teaching session, run an approved experiment, inspect observations and uncertainty, and retain a reproducible record.

A researcher should be able to inspect a research workspace, compare model versions, request shared instrument time, and follow the provenance of a result.

An operator should be able to see facility state, maintenance notices, staffing coverage, and resource requests without treating a dashboard label as a security decision.

The specimen succeeds only when these are connected experiences over coherent records—not disconnected room illustrations.

## 2. Campus composition

The initial campus is compact enough to implement and broad enough to exercise the LOD 3 model.

| Campus element | Purpose | Initial experience |
|---|---|---|
| Welcome and interpretation centre | Public entry, wayfinding, orientation | Campus map, mission gallery, guided tour |
| Teaching laboratory | Supervised learning and repeatable experiments | Instructor-led lesson and individual learner runs |
| Physics and materials laboratory | Model-backed scientific investigation | Parameter studies, measurement displays, model limitations |
| Instrument and calibration hall | Shared instrument services | Instrument status, calibration evidence, booking request |
| Research offices | Planning, collaboration, and interpretation | Whiteboards, notebooks, project boards, simulated workstations |
| Data and visualization centre | Explain observation-to-result workflows | Provenance explorer and synthetic dataset visualizer |
| Safety and operations office | Operational coordination and safety governance | Notices, review queue, maintenance and incident records |
| Director's office and council room | Institutional direction and accountable decisions | Mission, resource allocation, review decisions |
| Workshop and service yard | Maintenance and fabrication context | Equipment lifecycle and work orders |
| Archive and library | Preserve methods, results, and institutional memory | Searchable records and version history |

The initial campus should not require a full city simulation. It can be a self-contained host experience with a clear future placement seam for Singularity City.

## 3. Facility zones and access intent

Zones describe the experience and intended audience. They are not authorization rules.

- **Public:** welcome centre, public gallery, designated observation areas, and visitor interpretation.
- **Supervised learning:** teaching spaces where learner activities are bounded by an instructor's configured session.
- **Research workspace:** project rooms, offices, and research displays.
- **Instrument service:** instrument bays and calibration context.
- **Operations:** facility coordination and maintenance views.
- **Governance:** review, director, and institutional decision spaces.
- **Archive:** published and appropriately shared records.

A trusted host must independently authenticate principals, evaluate authorization, and enforce access. A zone label, character role, staff assignment, badge graphic, or client-side state is never sufficient to grant access.

## 4. People and institutional responsibilities

Use the City LOD 1 character identity as the continuing person identity. Use the Laboratories LOD 2 contracts for laboratory responsibilities. Do not create a second person/NPC system inside the campus.

Initial cast roles for the specimen:

- Campus director — institutional mission and resource decisions.
- Laboratory administrator — operational coordination.
- Safety officer — review coordination and stop-work escalation.
- Principal investigator — scientific direction for the research program.
- Instructor — supervised learner sessions.
- Instrument technician — instrument readiness and calibration coordination.
- Researchers — questions, methods, observations, and interpretation.
- Learners — enrolled participants, not automatically staff.
- Visitors — bounded public participants.
- Service principals — non-human automation identities, never represented as people.

The current `LaboratoryStaffAssignment` records an opaque host-issued principal, laboratory ID, role, assigning principal, assignment time, and optional expiry. It does not link a City character, prove a qualification, define shift coverage, or grant permissions. Do not imply those capabilities in the demo. If the specimen needs a character display name, resolve it through an explicit host/City integration rather than overloading `PrincipalId`.

## 5. Scientific content and fidelity

The first physics/materials demonstration should use an existing supported experiment/model contract. Do not invent Physics APIs or duplicate equations in the facility layer.

Every experiment presented to a learner must expose:
- experiment and model identifiers and versions;
- scientific validation level;
- assumptions and limitations;
- parameter and unit definitions from the model adapter;
- observation values with units and provenance;
- uncertainty and diagnostics when supplied by the model;
- an explicit distinction between measured, derived, and synthetic data.

The current `ExperimentDefinition` requires at least one disclosed assumption and limitation and binds an optional validation assessment to the exact experiment and model versions. Use those contracts rather than a visual-only claim of scientific accuracy.

The data centre may initially use a clearly labeled synthetic dataset to teach provenance and visualization. It must not be described as live observatory, CERN, or laboratory data.

## 6. Resource requests and institutional decisions

The specimen should demonstrate an auditable request-and-decision experience, without pretending a complete procurement or budgeting service already exists.

Example requests:
- reserve an instrument time slot;
- request technician support;
- request a teaching-session allocation;
- request compute capacity for a bounded parameter study;
- request archive publication review.

Each displayed request should distinguish its lifecycle state (draft, submitted, under review, approved, declined, fulfilled, cancelled) and record who made the decision and when. The first iteration may present these as illustrative workflow records. Do not turn them into a production authorization system or infer real resource availability from mock values.

The director's office is a presentation of institutional governance, not a special permission bypass.

## 7. Safety, security, and cinematic effects

Safety notices, review evidence, staffing responsibilities, and operational state belong in the laboratory domain. Authentication, authorization, secrets, isolation, and enforcement belong to the trusted host.

Default presentation is calm and evidence-led. No explosions, uncontrolled electrical arcs, hazardous process instructions, or Hollywood disaster sequences are enabled by default. A later instructor-controlled cinematic mode may present fictional consequences as clearly labeled visualization, isolated from scientific results and never represented as an actual experiment.

For this first specimen, incident scenarios should focus on safe operational learning: instrument drift, an expired qualification notice, an unavailable utility, a cancelled session, or a dataset with missing provenance.

## 8. Interaction flows and acceptance criteria

### Visitor tour
- Start at the welcome centre.
- Navigate to at least four distinct campus functions.
- Explain what each facility does and who owns its scientific content.
- Label reconstructed or fictional geometry as such.

### Supervised lesson
- Instructor selects an existing experiment definition.
- Learner receives the experiment's actual assumptions, limitations, and supported parameter range.
- Execution goes through the existing execution adapter and FSM_Execution boundary.
- Result view presents returned observations and provenance; it does not recalculate physics.

### Research review
- Researcher compares two versioned runs.
- Each run retains its exact experiment/model/configuration provenance.
- Reviewer can identify missing evidence or limitations.
- A visual comparison never silently merges incompatible model versions.

### Instrument service
- Visitor sees a safe public status summary.
- Technician sees the instrument's declared service state and calibration evidence.
- Booking is a request until a real scheduling contract exists.
- A green icon is not proof of calibration or authorization.

### Resource decision
- Request, decision, and fulfillment are separate states.
- The record identifies the responsible actor and timestamp.
- A demonstration fixture is visibly marked as sample data.

### Security boundary
- A client-side role change cannot reveal restricted content or invoke a privileged action.
- A staff assignment is not treated as a host permission.
- Expiry is evaluated at a defined UTC instant using the existing assignment contract.

## 9. Implementation sequence

1. Inspect and reuse current contract, persistence, execution, Physics, and MicroBundle adapters. **Completed:** the existing facility manifest and adapter boundaries were inspected.
2. Add a host-neutral campus composition contract that keeps `LaboratoryDefinition` and `LaboratoryFacility` separate. **Completed:** `LaboratoryCampus` composes portable laboratories and facilities through explicit workspace-to-room placements.
3. Add unit tests for composition invariants and invalid references before adding presentation. **Completed:** tests cover immutable snapshots, valid placement, unknown laboratory/room references, duplicate placement IDs, and catalog lookup.
4. Compose one teaching lab and one research lab using existing laboratory/experiment definitions. **Next:** author the first campus data fixture against the new contract.
5. Add the sample campus fixture and a simple navigable Renderer/Visualize experience.
6. Add synthetic provenance and instrument-status examples, clearly marked as fixtures.
7. Integrate City character identity only through an explicit, reviewed boundary.
8. Add resource-request workflows only when their owner and persistence semantics are established.
9. Validate the visitor, lesson, research, instrument, resource, and security acceptance flows.

## 10. Explicit non-goals for the first specimen

- Reconstructing a real facility's restricted interior or security arrangements.
- A general-purpose HR, identity, credential, or authorization service.
- A complete procurement, finance, booking, or workforce scheduling platform.
- Reimplementing Physics, electrical simulation, beam dynamics, or data analysis.
- Claiming live data without a verified, attributable adapter.
- Requiring Singularity City or a game engine for the portable laboratory definition.

## 11. Cross-domain coordination

| Dependency | Required contribution |
|---|---|
| Physics | Existing model contracts, supported experiment domains, units, validation, and observation semantics |
| FSM_Execution | Execution lifecycle, cancellation, timeout, and result boundary |
| FSM_COS | Experience composition and MicroBundle integration |
| Singularity City | Character identity and optional campus spatial manifestation |
| Renderer / Visualize | Navigable facility presentation and live-state visualization |
| Trusted host | Authentication, authorization, enforcement, data isolation, and audit |

Do not add speculative references to these dependencies. If a required contract is missing, record the gap and request it from its owning repository.

---

**Implementation status:** The portable campus composition contract and catalog exposure are implemented on `development`, with invariant tests added and CI running. The flagship campus data fixture, spatial/runtime presentation, booking, resource allocation, and interactive demo are not yet implemented. This remains an acceptance specification, not a claim of a completed campus experience.
