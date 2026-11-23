<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Singularity Laboratories

[Singularity Laboratories Pages preview](https://trentbest.github.io/TheSingularityWorkshop.SingularityLaboratories/) — the site source and Actions deployment are in place; the repository's Pages setting still needs to be enabled for the first deployment.

**The Workshop's laboratory, experimentation, education, and research platform — from the subatomic to the astronomical.**

Singularity Laboratories provides abstractions, authoring tools, runtime workflows, instruments, and scientific services for creating and operating laboratories. It is where people configure experiments, investigate models, collect measurements, teach scientific methods, conduct research, and share reproducible work.

A laboratory is portable. It can appear directly in The Singularity Workshop, as a facility or campus inside Singularity City, in an online classroom, a private research workspace, or a game. The host provides navigation, social presence, world geometry, and presentation; the laboratory platform owns its scientific definitions, measurement semantics, and run provenance.

## 1. Domain ownership

- **Singularity City** owns general character, resident (Digiten), visitor, and NPC identity and agency. See [Nonplayer Characters — LOD 1](https://github.com/TrentBest/TheSingularityWorkshop.SingularityCity/blob/development/docs/books/nonplayer-characters.md).
- **Singularity Laboratories** owns portable laboratory and experiment semantics, facility and instrument definitions, scientific workflows, and laboratory-specific staffing and governance.
- **Physics** owns mathematical and physical models and numerical methods.
- **FSM_Execution** owns workflow lifecycle, cancellation, and execution.
- **FSM_COS** composes laboratory experiences from MicroBundles.
- **Trusted hosts** own authentication, authorization, credentials, and enforcement.
- **Renderer / Visualize** own presentation and visual manifestation.

[Browse the laboratory sourcebook catalogue](docs/books/README.md). The sourcebook hierarchy now continues through three levels: [Laboratory NPCs — LOD 2](docs/books/laboratory-npcs.md) specializes the City people model for laboratory positions, qualifications, staffing, shifts, participant relationships, safety governance, and independent review; [Flagship Laboratory Facilities — LOD 3](docs/books/flagship-laboratory-facilities.md) composes complete institutions, campuses, infrastructure, programs, and operations. These levels extend the same domain rather than creating duplicate people or scientific models.

## 2. What this package owns

- **Create and configure:** laboratories, facilities, experiment templates, instruments, apparatus, parameters, materials, environments, and workflows.
- **Operate:** initialize, validate, execute, pause, resume, cancel, checkpoint, and reproduce experiments.
- **Measure:** instruments, sensors, sampling, observation channels, units, uncertainty, and data capture.
- **Teach:** guided labs, assignments, instructor-led demonstrations, classroom sessions, and learner progression.
- **Research:** hypotheses, runs, provenance, collaboration, analysis, comparison, and reproducible work.
- **Staff and govern:** laboratory-specific positions and responsibilities, qualification context, safety review, and auditable governance.
- **Extend:** compose disciplines, instruments, models, lessons, and lab experiences from versioned reusable components.

The laboratory layer must not duplicate core physics or invent unverified Physics APIs. A facility building is a possible City manifestation, not the laboratory domain object itself.

## 3. Scientific domains

| Domain | Example capabilities |
|---|---|
| Particle physics | Beam and collider studies, detector configuration, event visualization |
| Chemistry | Molecular modeling, reaction studies, thermodynamics |
| Nuclear science | Fission/fusion models, radiation transport, shielding, consequence studies |
| Astronomy and cosmology | Orbital systems, stellar evolution, planetary science, galaxies |
| Engineering | Materials, structures, fluids, heat transfer, mechanical systems |
| Earth and environmental science | Atmospheres, oceans, climate processes, impact and hazard scenarios |
| Biology and ecology | Cells, populations, ecosystems, interacting biological systems |

Hazardous phenomena may be explored as bounded educational simulations of processes and consequences, not as actionable construction or operational guidance. High-consequence claims require appropriate validation and expert review.

## 4. Current implementation

The initial runtime is a host-neutral foundation with versioned laboratory/experiment definitions, configuration validation, authoring templates, Physics and FSM_Execution adapters, MicroBundle composition, immutable run records, persistence, replay, result views, safety metadata, validation assessments, admission policy, and resource-budget estimates.

The `LaboratoryStaffAssignment` contract currently records a host-issued opaque principal reference, laboratory identity, responsibility, assigning principal, assignment time, and optional expiry. Its `PrincipalId` is a digital host principal reference, **not** a City NPC/person ID. It does not prove a qualification, define a stable institutional position, or grant digital permissions. A future character-to-principal mapping requires an explicit trusted identity-link contract.

This is a foundation, not yet the complete authoring, instrument, classroom, research, or hosted runtime platform. The repository's Pages setting still needs to be enabled for the first deployment; the source and workflow are not proof of a live published site.

## 5. Development and release

- Active work: `development`
- Release-aligned branch: `master`
- Pages workflow: `.github/workflows/pages.yml`
- No NuGet package is published by this repository workflow.
- Changes are developed and validated on `development`; release synchronization and publication remain explicit gated activities.

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="240" />
</p>
