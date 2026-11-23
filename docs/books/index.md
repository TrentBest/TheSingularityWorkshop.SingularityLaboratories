![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Singularity Laboratories — SURPS Sourcebook Catalogue

This is the canonical local catalogue for Laboratory-owned SURPS sourcebooks. The [SURPS Master Rulebook Map](https://github.com/TrentBest/TheSingularityWorkshop.SingularityUniversalRoleplayingSystem/blob/development/docs/rulebook-backlog/index.md) is authoritative for global identity, ownership sign-in, and repository participation.

**Repository status:** integrated, not enrolled. The three sourcebooks below are draft declarations awaiting independent SURPS ownership review. Their presence does not claim that every described capability has a completed runtime implementation.

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
