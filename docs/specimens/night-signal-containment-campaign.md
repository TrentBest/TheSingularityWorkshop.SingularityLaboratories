![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# NIGHT SIGNAL — Containment Breach Campaign

**Domain:** Singularity Laboratories  
**Experience:** First-person laboratory survival / systems investigation  
**Fidelity:** Original fictional scenario; abstract containment simulation  
**Status:** Browser-playable proof of concept plus typed narrative campaign contract/state machine, atomic filesystem attempt persistence, FSM_Execution workflow adapter, and version-bound narrative-to-facility room mapping; live facility-status adapter remains in progress

> The alarm is already sounding when the player wakes at their workstation. The facility is sealed. The incident has begun. There is no character creator, loadout screen, or alternate opening. The only question is whether the player can understand the facility well enough to leave it safely.

## 1. Campaign identity

| Field | Value |
|---|---|
| Campaign ID | `night-signal-containment` |
| Campaign version | `1.0.0` |
| Facility | Northstar Institute — Sublevel Research Annex (fictional) |
| Player role | On-duty research technician |
| Opening state | Inside the annex when an unexplained containment event is declared |
| Terminal outcomes | Player death; verified safe escape |
| Character creation | None |
| Campaign completion | One terminal outcome has been recorded |

This is an original work of fiction that uses the broad premise of a sealed research facility during an outbreak emergency. It does not reproduce the plot, characters, text, or setting of *The Stand*.

## 2. Design intent

This is the flagship end-to-end laboratory experience. It should make the player inhabit the institution: understand its layout, staff roles, instruments, research records, access boundaries, power and environmental state, emergency communications, and evidence trail.

The player is not asked to create a scientist. The scenario assigns a fixed role and starts play inside the laboratory. The laboratory is the protagonist's world and the system under investigation.

The campaign ends only when:
- the player's health reaches zero; or
- the player reaches the exterior through a route whose fictional containment state, authorization state, and egress state have all been independently verified.

Reaching a door is not the same as escaping safely. A successful outcome must preserve the evidence that justified the route.

## 3. Opening scene

The player is alone in a small research workroom when the building transitions to emergency mode. A facility announcement is interrupted. Several internal doors have changed state. The public map no longer reflects the live fictional state. A terminal displays an incident identifier but not an explanation.

The opening deliberately withholds context. The player must discover:
1. **What happened?** Recover the incident timeline and distinguish confirmed facts from speculation.
2. **What is the facility state?** Inspect the facility status record and identify which zones are declared isolated, unknown, or available.
3. **Where can a person go?** Compare the static floor plan with the current egress-status record.

These are evidence-retrieval tasks, not real-world pathogen-control instructions. The fictional interface exposes abstract state labels and deliberately omits operational biological procedures.

## 4. The facility as a system

| Area | Player-facing purpose | Systems exercised |
|---|---|---|
| Research workroom | Fixed starting location; first incident clues | Experiment records, workstation, local observations |
| Observation gallery | View the fictional research environment without entering it | Instrument metadata, observation provenance, uncertainty |
| Records room | Recover timeline, staff notes, and model disclosures | Archive, identity, versioned records |
| Facility operations | Inspect power, doors, zone state, and communications | Facility composition, status snapshots, host observations |
| Transit junction | Compare route options and route-state evidence | Navigation, access boundary, egress state |
| Exterior checkpoint | Terminal review of the escape record | Outcome evaluation and evidence export |

The facility map is a navigational model for this fictional scenario. It is not a real building plan. A typed `NarrativeCampaignFacilityBinding` maps six stable narrative location IDs to unique `LaboratoryRoom` IDs in the exact versioned Northstar facility manifest, and binds apparatus, fictional instrument, and measurement-channel IDs to those locations. The adapter validates campaign/facility identity, room existence, apparatus existence and room ownership, instrument-to-apparatus ownership, channel-to-instrument ownership, and the fixed starting-room display name. These instruments expose fictional on-demand state indicators (unit: `state`), not physical measurements or pathogen readings. The binding also maps the fixed player role to a host-supplied `LaboratoryStaffAssignment`, validating laboratory identity, required role, and active interval at a host-supplied UTC instant. This validates responsibility metadata only; it does not authenticate a principal or grant authorization. The adapter does not define geometry, navigation, or operational control.

## 5. Core campaign loop

`Observe → Form a hypothesis → Retrieve evidence → Compare facility state → Choose → Record consequence → Reassess`

The player can inspect evidence in any order. A fact is considered known only after the corresponding record has been read. The UI should distinguish:
- **Unknown:** no relevant record has been inspected.
- **Observed:** a source was inspected, but it may be stale or incomplete.
- **Verified for this scenario:** independent records agree on the required fictional state.
- **Contradicted:** records disagree; the route is not eligible for safe completion.

No narrative voiceover may silently convert unknown data into a verified fact.

## 6. Safe-escape gate

The browser proof of concept uses three deliberately abstract evidence tokens:

| Evidence token | Source | What it establishes |
|---|---|---|
| Incident timeline | Records room | The current incident boundary and the age of the information |
| Facility state | Operations console | The declared state of relevant zones and whether the snapshot is current |
| Egress record | Transit junction | A route is explicitly marked available for this scenario |

The safe-escape terminal gate requires all three tokens and a final consistency check. If any is missing, stale, or contradictory, the route remains unverified. The typed state machine additionally requires independent source identifiers for the required evidence categories and checks that the route being attempted matches the latest verified egress record. A player may still choose to gamble, but that is a deliberate unsafe ending—not a safe escape.

This is game logic only. These tokens are not a real-world checklist, procedure, or biosafety standard.

## 7. Death and safe-escape endings

### Ending A — Lost in the annex

The player's health reaches zero. The campaign closes immediately and preserves the last location, last known facility snapshot, collected evidence, decisions, and terminal cause. Death is not erased by restarting; a restart creates a new campaign attempt.

### Ending B — Verified exterior

The player reaches the exterior only after the three evidence tokens have been collected and the consistency gate passes. The campaign preserves the route decision, evidence references, final facility snapshot, and an explicit statement that this is a fictional scenario outcome.

A door opening, an animation, or a player reaching the map boundary cannot independently award this ending.

## 8. Systems coverage matrix

| Laboratory capability | Campaign use | Proof expected |
|---|---|---|
| Laboratory / facility identity | Northstar Institute and annex composition | Stable IDs and versions |
| Staff and responsibilities | Fixed protagonist, missing staff, recorded handoffs | Role and assignment records remain distinct from authorization |
| Experiment identity | Fictional research program and declared model | Exact experiment/model version references |
| Instruments and observations | Observation gallery and instrument snapshots | Timestamped source, unit/meaning, fidelity label |
| Campaign definition | Fixed opening, evidence objectives, terminal criteria | Immutable versioned definition |
| Scenario execution | Alternate facility states and decision paths | Completed, failed, cancelled, and not-run states retained |
| Workflow lifecycle | Alarm, investigation, route decision, terminal outcome | Explicit state transitions and cancellation |
| Persistence and replay | Attempt record and evidence trail | Reproducible campaign input and event sequence |
| Host composition | Rooms, navigation, identity, presentation | Laboratory semantics remain portable |
| Analysis | Compare collected evidence to safe-escape gate | Pass, fail, or not-evaluable; no hidden assumptions |

The scientific `ExperimentCampaignRunner` continues to execute experiment scenarios; it is not the narrative runner. The repository now has a separate typed `NarrativeCampaignDefinition` and `NarrativeCampaignStateMachine` with immutable attempt snapshots, evidence-source references, terminal death/safe-escape outcomes, and an append-only event sequence. A version-bound `NarrativeCampaignFacilityBinding` maps the six fictional campaign locations onto stable room IDs in a `LaboratoryFacility`, with apparatus, fictional instrument, and measurement-channel references validated against the room/instrument inventory; the adapter rejects campaign/facility version drift, missing rooms or equipment, duplicate assignments, apparatus in the wrong room, instruments attached to unmapped apparatus, channels exposed by unmapped instruments, and a starting-room mismatch. Host-supplied staff assignments are checked against the fixed player role, laboratory identity, role, and active interval; they remain responsibility records, not authorization decisions. `INarrativeCampaignAttemptStore` and `FileSystemNarrativeCampaignAttemptStore` persist the exact definition, outcome, complete evidence history, and ordered event history using atomic, non-overwriting JSON writes. Restoration validates event sequencing, evidence/event correspondence, terminal consistency, independent evidence sources, and the egress-route gate. `NarrativeCampaignWorkflowAdapter` registers the narrative-specific `laboratory.executeNarrativeCampaign` handler with FSM_Execution; a timestamped command sequence drives evidence, death, and escape transitions, and the final JSON-safe attempt snapshot is persisted. Workflow cancellation preserves the last complete active snapshot before cancellation propagates. NUnit tests cover the fixed opening, evidence gate, stale/contradicted evidence, terminal behavior, restart history, campaign identity, persistence round-trip, duplicate identities, corrupt/mismatched records, workflow safe escape, player death, cancellation, version-bound room/apparatus/instrument/channel mapping, and active/expired/wrong-role staff assignment checks. The browser experience remains a separate local proof of concept: it does not invoke this C# state machine/store or dispatch the workflow itself.

## 9. Acceptance tests

1. The player always starts in the same workroom with the same assigned role.
2. No character-creation or loadout step exists.
3. Every evidence token has a source, timestamp/status, and inspect action.
4. Unknown or contradictory facility state never satisfies the safe-escape gate.
5. The safe ending is unreachable until all required evidence is present and current.
6. Any death event closes the attempt and records the terminal cause.
7. Restart creates a fresh attempt and does not overwrite the prior attempt.
8. Every decision and state transition is exportable as a campaign event record.
9. Narrative prose never claims that a fictional result is a real pathogen, physical measurement, or validated safety outcome.
10. A browser-only run is labelled as a local demonstration and does not create a scientific execution record.

## 10. Implementation sequence

1. **Browser scenario:** playable state machine, fixed opening, evidence retrieval, terminal outcomes, and downloadable local event log.
2. **Typed campaign definition and domain state machine — implemented:** fixed role/start, required evidence categories, source-backed evidence records, immutable attempt snapshots, terminal outcomes, and append-only events; NUnit invariant tests added.
3. **Durable attempt persistence — implemented:** atomic filesystem store, duplicate-ID rejection, exact identity verification, complete evidence/event round-trip, and validated restoration of terminal outcomes; persistence tests added.
4. **FSM_Execution workflow adapter — implemented and CI verified:** deterministic timestamped command sequence, distinct narrative handler, safe-escape/death outcomes, JSON-safe output snapshot, and preservation of the last active snapshot on cancellation.
5. **Facility room, apparatus, instrument, and channel binding — implemented and CI verified:** version-bound mapping of all six narrative locations to stable room identities, with apparatus, fictional state instruments, and on-demand measurement channels validated against their parent identities.
6. **Staff role binding — implemented and CI verified:** the fixed player role maps to a host-supplied active `LaboratoryStaffAssignment` with exact laboratory and role validation; this is not an authorization decision.
7. **Live facility-status adapter — in progress:** project host-owned status snapshots onto the mapped identities.
7. **Analysis and replay:** evaluate terminal criteria from recorded evidence; replay attempts without rewriting history.
8. **Host experience:** compose the facility in a world/GUI host without moving laboratory semantics into the host.

## 11. Scientific and safety boundary

The outbreak is fictional and its biology is intentionally unspecified. No transmissibility model, pathogen protocol, containment procedure, decontamination sequence, or real facility instruction is being asserted. The scenario tests whether the platform can preserve identities, evidence, uncertainty, decisions, and terminal outcomes—not whether a real laboratory is safe.
