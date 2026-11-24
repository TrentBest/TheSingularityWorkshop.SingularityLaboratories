![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Singularity Laboratories — SURPS Sourcebook Catalogue

> This page is the local catalogue of SURPS-governed domain sourcebooks. It is not the primary package manual. Begin with [Singularity Laboratories — The Living Laboratory, Core Manual](../laboratory-manual.md), then use the [Technical Companion](../laboratory-technical-companion.md) for implementation and integration details.


This is the canonical local catalogue for Laboratory-owned SURPS sourcebooks. The [SURPS Master Rulebook Map](https://github.com/TrentBest/TheSingularityWorkshop.SingularityUniversalRoleplayingSystem/blob/development/docs/rulebook-backlog/index.md) is authoritative for global identity, ownership sign-in, and repository participation.

**Repository status:** integrated, not enrolled. The three sourcebooks below are draft declarations awaiting independent SURPS ownership review. Their presence does not claim that every described capability has a completed runtime implementation.

## SURPS living rulebook and behavior-adapter standard

This repository is adopting the shared SURPS [Living Rulebook and Experience Standard](https://github.com/TrentBest/TheSingularityWorkshop.SingularityUniversalRoleplayingSystem/blob/development/docs/living-rulebook-standard.md) and [Behavior Model and Adapters](https://github.com/TrentBest/TheSingularityWorkshop.SingularityUniversalRoleplayingSystem/blob/development/docs/behavior-and-adapters.md). Laboratories owns experiments, apparatus, research procedures, and educational experience data. Illustrations should show the experiment and its observable state; distinguish a teaching model from a tested physical evaluator.

The standard is the target for new work; full conformance remains in progress until the linked issue is accepted. It requires a big-picture-first guide, concise Player path, deep Game Master/Workshop path, living pictures for major concepts, worked demonstrations for mechanics, and honest status labels. The tabletop adapter is derived from underlying behavior during workshop calibration; the digital adapter uses compiled behavior with live state. Neither is a separate source of truth. Adoption work: [repository issue](https://github.com/TrentBest/TheSingularityWorkshop.SingularityLaboratories/issues/6).

| Sourcebook | LOD | Descriptor | Cover | Domain |
|---|---:|---|---|---|
| [Scientific Inquiry](scientific-inquiry.md) | 1 | [Descriptor](scientific-inquiry.book.json) | [Cover](covers/scientific-inquiry.svg) | Scientific methods, research history, evidence, uncertainty, reproducibility |
| [Laboratory NPCs](laboratory-npcs.md) | 2 | [Descriptor](laboratory-npcs.book.json) | [Cover](covers/laboratory-npcs.svg) | Laboratory-specific positions, qualifications, staffing, shifts, and governance |
| [Flagship Laboratory Facilities](flagship-laboratory-facilities.md) | 3 | [Descriptor](flagship-laboratory-facilities.book.json) | [Cover](covers/flagship-laboratory-facilities.svg) | Complete institutions, campuses, departments, infrastructure, operations, and experiences |

## Ownership boundaries

- **Singularity Laboratories:** portable laboratory semantics, experiment definitions, apparatus and instrument descriptions, measurement context, research workflows, laboratory staffing, facility composition, and provenance.
- **SURPS:** shared entity and roleplaying ontology, CORE rules, global book identity, LOD semantics, and opt-in composition.
- **Singularity City:** general character, resident, visitor, and NPC identity and agency. Laboratory NPCs extend City-owned people rather than redefining them.
- **Physics:** physical and mathematical models, numerical methods, and physical simulation.
- **FSM_Execution:** workflow lifecycle, cancellation, checkpointing, and execution.
- **Trusted hosts:** authentication, authorization, credentials, and enforcement.
- **StoryBuilder:** campaign continuity and authored narrative when laboratory experiences are used in a roleplaying campaign.

## Descriptive knowledge versus executable capability

A sourcebook may describe an apparatus, procedure, safety constraint, expected observation, or scientific method without asserting that the corresponding capability is executable. An executable capability requires its own stable identity and version, typed inputs and outputs, explicit preconditions, provenance, and validation evidence. Gauntlet may enroll and test declared capabilities, but a descriptive sourcebook entry alone is not proof of an executable provider.

## Validation

The canonical machine-readable declaration is [`.surps/participation.json`](../../.surps/participation.json). Run the local integrity gate before changing the catalogue:

```bash
python scripts/validate_surps_participation.py
```

The [SURPS Sourcebook Validation workflow](../../.github/workflows/surps-sourcebook-validation.yml) runs this check for relevant changes on `development`. A passing local check validates identity and asset consistency; it does not accept ownership claims or enroll the repository.
