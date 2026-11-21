# Laboratory and Experiment Model

A laboratory is a versioned, portable, composable scientific environment. It combines one or more scientific models, apparatus, instruments, experiment workflows, educational or research context, and presentation modes. It is a domain object—not a room, building, scene, or city location.

## Laboratory definition

A laboratory definition identifies:
- Discipline, purpose, intended audience, and scientific questions it supports.
- Supported experiment templates and scientific model versions.
- Apparatus, instruments, measurement channels, and interaction capabilities.
- Units, coordinate systems, valid parameter ranges, and boundary conditions.
- Required MicroBundles and optional integrations.
- Fidelity classifications, compute/resource profiles, and execution constraints.
- Safety classification, validation evidence, review status, and known limitations.
- Version, authorship, licensing, visibility, access, and collaboration rules.
- Educational capabilities such as lessons, instructor controls, assignments, and learner progression.
- Research capabilities such as parameter studies, datasets, comparison workflows, and export.

## Core concepts

| Concept | Meaning |
|---|---|
| Laboratory | Portable scientific environment in which experiments are designed and conducted |
| Facility / complex | Optional collection or presentation of labs; a host may manifest it as a building or campus |
| Experiment | Investigation with a question/objective, defined inputs, procedure, and observations |
| Model | Versioned scientific representation used to calculate outcomes |
| Apparatus | Configurable experimental setup or equipment |
| Instrument | Measurement, sensing, visualization, or interaction capability |
| Configuration | Complete inputs and initial conditions for one run |
| Run | One execution of an immutable experiment/configuration snapshot |
| Observation | Measured or derived value, with units, uncertainty, and provenance |
| Result | Durable outputs, diagnostics, model metadata, and interpretation |
| Lesson | Guided educational context, instructions, and learning objectives |
| Course activity | Instructor-assigned activity with learner work and evidence |
| Research workspace | Hypotheses, variants, runs, datasets, notes, and collaboration |
| Publication | Versioned, reviewable release of a laboratory, instrument, experiment, or lesson |

## Laboratory authoring and configuration

The tooling must let an author create a laboratory without building a bespoke application for every scientific domain. Authors should be able to assemble:
- A domain identity and scientific purpose.
- Model adapters and declared model capabilities.
- Apparatus and instrument definitions.
- Parameter schemas with units, defaults, ranges, dependencies, and validation rules.
- Initial and boundary condition editors.
- Experiment procedures and state transitions.
- Measurement plans, sampling configuration, and output channels.
- Visual and analytical result views.
- Lessons, instructor notes, learner prompts, and assessment criteria.
- Resource budgets, fidelity choices, safety notices, and permissions.

Authoring should be declarative where possible, extensible where necessary, and validated before publication. Never expose unsupported model parameters merely because a UI control can be drawn for them.

## Classroom and online learning

An instructor should be able to select a laboratory, assign an experiment, choose allowed model fidelity and parameter ranges, prepare a shared demonstration, and guide a class through a scientific question. Each learner should be able to configure and run their own experiment, record observations, compare results, and submit evidence.

Classroom tooling should support instructor and learner roles, assignment and due-state metadata, shared versus individual runs, guided steps, checkpoints, discussion/annotation hooks, and assessment evidence. Roster, identity, video, chat, and classroom scheduling may come from external host services; the lab platform owns the scientific activity and its records.

## Research workflows

Research users need hypothesis and question tracking, experiment variants, controlled parameter sweeps, batch runs, comparison and analysis, dataset provenance, notes, checkpoints, collaboration, export, and publication. Every result must remain traceable to the exact laboratory, experiment, model, adapter, configuration, and execution environment that produced it.

## Example experiences

### Particle physics
A collider laboratory can teach beam concepts, detector geometry, collision visualization, and interpretation of simulated event data. State what is represented and what is simplified. A large virtual collider is compelling, but its visual circumference does not establish accelerator fidelity.

### Chemistry
Learners can assemble molecular models, vary conditions within a model's supported domain, observe energy changes, and compare reaction pathways. Energetic reactions may be represented through bounded educational models without actionable synthesis or explosive-construction instructions.

### Nuclear science
Explore fission and fusion concepts, radiation transport, shielding, and environmental consequences. High-consequence scenarios use vetted aggregate consequence models, disclose uncertainty, and avoid weapon-design or optimization functionality.

### Astronomy
Configure planetary systems, observe orbital behavior, investigate stellar lifecycles, and explore galaxy-scale visualizations. Educational demonstrations and research-grade simulations must be labeled distinctly.

## Reproducibility

Capture laboratory and experiment identifiers and versions; scientific model and adapter versions; input values with units; initial and boundary conditions; random seeds and stochastic settings; solver, timestep, precision, and convergence settings; execution environment and resource profile; outputs, warnings, and diagnostics; and authorship / collaboration provenance.

## Educational progression

1. **Explore:** guided observation and safe parameter changes.
2. **Investigate:** formulate a question, control variables, compare runs.
3. **Analyze:** inspect measurements, units, uncertainty, assumptions.
4. **Design:** author experiments from approved templates and components.
5. **Research:** use reviewed models, documented methods, reproducible configurations, exportable results.

Teach how to reason about a model, not merely operate its controls.
