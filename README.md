<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Singularity Laboratories

[Explore the interactive Singularity Laboratories experience](https://trentbest.github.io/TheSingularityWorkshop.SingularityLaboratories/) — capability atlas, playable NIGHT SIGNAL containment campaign, campaign-design workbench, evidence boundaries, and research sourcebooks. The site source and Actions deployment are in place; the repository's Pages setting still needs to be enabled for the first deployment.

**Start with the books:** [The Living Laboratory — Core Manual](docs/laboratory-manual.md) · [Technical Companion](docs/laboratory-technical-companion.md) · [SURPS sourcebook catalogue](docs/books/index.md)

**The Workshop's laboratory, experimentation, education, and research platform — from the subatomic to the astronomical.**

Singularity Laboratories provides abstractions, authoring tools, runtime workflows, instruments, and scientific services for creating and operating laboratories. It is where people configure investigations, study models, collect measurements, teach scientific inquiry, conduct research, and share reproducible work.

Scientific work can lose its meaning when the original question, method, model version, configuration, execution, and result are separated. Singularity Laboratories keeps those identities connected so that a host can change the experience without silently changing the scientific basis of the investigation.

A laboratory is portable. It can appear directly in The Singularity Workshop, as a facility or campus inside Singularity City, in an online classroom, a private research workspace, or a game. The host provides navigation, social presence, world geometry, and presentation; the laboratory platform owns its scientific definitions, measurement semantics, and run provenance.

## 1. What — domain ownership and scope

- **Singularity City** owns general character, resident (Digiten), visitor, and NPC identity and agency. See [Nonplayer Characters — LOD 1](https://github.com/TrentBest/TheSingularityWorkshop.SingularityCity/blob/development/docs/books/nonplayer-characters.md).
- **Singularity Laboratories** owns portable laboratory and experiment semantics, facility and instrument definitions, scientific workflows, and laboratory-specific staffing and governance.
- **Physics** owns mathematical and physical models and numerical methods.
- **FSM_Execution** owns workflow lifecycle, cancellation, and execution.
- **FSM_COS** composes laboratory experiences from MicroBundles.
- **Trusted hosts** own authentication, authorization, credentials, and enforcement.
- **Renderer / Visualize** own presentation and visual manifestation.

[Explore the virtual laboratory service concept](docs/virtual-laboratory-service.md), including rentable virtual workspaces and preflight experiment campaigns. [Browse the laboratory sourcebook catalogue](docs/books/index.md). The sourcebook hierarchy includes [Scientific Inquiry — LOD 1](docs/books/scientific-inquiry.md), [Laboratory NPCs — LOD 2](docs/books/laboratory-npcs.md), and [Flagship Laboratory Facilities — LOD 3](docs/books/flagship-laboratory-facilities.md). These extend the same domain rather than creating duplicate people or scientific models.

## 2. Why — a portable research lifecycle

- **Inquire:** frame questions, select methods, disclose assumptions, preserve evidence, assess uncertainty, and support criticism and correction.
- **Create and configure:** laboratories, facilities, investigation templates, instruments, apparatus, parameters, materials, environments, and workflows.
- **Operate:** initialize, validate, execute, pause, resume, cancel, checkpoint, and reproduce experiments.
- **Measure:** instruments, sensors, sampling, observation channels, units, uncertainty, and data capture.
- **Teach:** guided investigations, assignments, instructor-led demonstrations, classroom sessions, and learner progression.
- **Research:** hypotheses where appropriate, field observations, runs, provenance, collaboration, analysis, comparison, and reproducible work.
- **Staff and govern:** laboratory-specific positions and responsibilities, qualification context, safety review, and auditable governance.
- **Extend:** compose disciplines, methods, instruments, models, lessons, and lab experiences from versioned reusable components.

The scientific method is not a universal five-step recipe. Different disciplines use controlled experiments, observational and field methods, mathematical proof, computational studies, historical inference, qualitative research, and evidence synthesis. The common foundation is transparent reasoning, appropriate evidence, uncertainty, criticism, and correction. Start with the [Scientific Inquiry sourcebook](docs/books/scientific-inquiry.md).

The laboratory layer must not duplicate core physics or invent unverified Physics APIs. A facility building is a possible City manifestation, not the laboratory domain object itself.

## 3. How — methods and scientific domains

| Domain | Example capabilities |
|---|---|
| Particle physics | Beam and collider studies, detector configuration, event visualization |
| Chemistry | Molecular modeling, reaction studies, thermodynamics |
| Nuclear science | Fission/fusion models, radiation transport, shielding, consequence studies |
| Astronomy and cosmology | Orbital systems, stellar evolution, planetary science, galaxies |
| Engineering | Materials, structures, fluids, heat transfer, mechanical systems |
| Earth and environmental science | Atmospheres, oceans, climate processes, impact and hazard scenarios |
| Biology and ecology | Cells, populations, ecosystems, interacting biological systems |
| Human and social sciences | Surveys, interviews, behavioral studies, longitudinal and comparative research |
| History and archaeology | Archival evidence, material culture, dating, provenance, historical inference |
| Mathematics and computing | Proof, formal methods, numerical experiments, statistical and computational studies |

Hazardous phenomena may be explored as bounded educational simulations of processes and consequences, not as actionable construction or operational guidance. High-consequence claims require appropriate validation and expert review.

## 4. See it — current implementation and evidence

The runtime includes versioned laboratory/experiment and campaign definitions, configuration validation, authoring templates, Physics and FSM_Execution adapters, MicroBundle composition, immutable run records, replay, result views, safety metadata, validation assessments, admission policy, and resource-budget estimates. Scientific experiment campaigns execute baseline and scenario configurations through the ordinary experiment runner, preserve per-scenario outcomes and run provenance, and can be submitted as FSM_Execution workflows with atomic campaign-execution persistence. NIGHT SIGNAL now also has a separate typed narrative campaign definition and evidence-gated state machine: fixed player role/start, source-backed evidence, immutable attempt snapshots, terminal death or verified safe escape, and append-only event history. Its browser proof of concept is still local-only and is not yet wired to this C# state machine or FSM_Execution.

The LaboratoryStaffAssignment contract currently records a host-issued opaque principal reference, laboratory identity, responsibility, assigning principal, assignment time, and optional expiry. Its PrincipalId is a digital host principal reference, **not** a City NPC/person ID. It does not prove a qualification, define a stable institutional position, or grant digital permissions. A future character-to-principal mapping requires an explicit trusted identity-link contract.

This is a foundation, not yet the complete authoring, instrument, classroom, research, or hosted runtime platform. The repository's Pages setting still needs to be enabled for the first deployment; the source and workflow are not proof of a live published site.

The publication set now includes the reader-facing [Core Manual](docs/laboratory-manual.md), [Technical Companion](docs/laboratory-technical-companion.md), and [SURPS sourcebook catalogue](docs/books/index.md). The three declared sourcebooks follow the shared living-book reading paths; their ownership claims remain pending independent SURPS review.

The Pages source now contains a browser-only campaign-draft workbench and a fictional campus explorer, backed by authored diagrams. These are teaching prototypes, not live Physics execution, instrument telemetry, reservations, or a hosted facility. A documentation CI gate checks the book structure, local links, in-page navigation, and accessible SVG metadata.

## 5. Choose your next step — develop and contribute

- Active work: development
- Release-aligned branch: master
- Pages workflow: .github/workflows/pages.yml
- No NuGet package is published by this repository workflow.
- Changes are developed and validated on development; release synchronization and publication remain explicit gated activities.

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="240" />
</p>
