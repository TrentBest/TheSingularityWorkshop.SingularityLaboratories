<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Singularity Laboratories

**A virtual research city for building, running, teaching, and sharing scientific experiments — from the subatomic to the astronomical.**

Singularity Laboratories is the laboratory and experimentation experience of The Singularity Workshop. It turns the Workshop's Physics capabilities into interactive, composable environments where learners, educators, researchers, and creators can investigate systems across extraordinary ranges of scale.

A laboratory may be a single molecular reaction, a particle detector, a simulated hadron collider, a planetary system, or a solar-system-scale experiment. Scale is a property of the experiment, not a product limitation.

## 1. The Experience

Singularity Laboratories is envisioned as a navigable scientific city: a collection of specialized facilities, public demonstrations, teaching spaces, private research environments, and creator-operated laboratories.

- **Explore:** enter laboratories, inspect equipment, observe live simulations, and discover experiments.
- **Experiment:** configure initial conditions, parameters, instruments, boundaries, and measurement plans.
- **Understand:** connect visual outcomes to the governing models, equations, assumptions, and uncertainty.
- **Teach:** guide learners through hands-on investigations, from introductory concepts to advanced research.
- **Research:** run reproducible experiments, compare scenarios, capture results, and collaborate.
- **Create:** build and publish new laboratory experiences using reusable tools and experiment definitions.

## 2. Laboratory Domains

| Domain | Example experiences |
|---|---|
| Particle physics | Collider halls, particle interactions, detector visualization, field studies |
| Chemistry | Molecular construction, reaction pathways, thermodynamics, energetic reactions |
| Nuclear science | Fission and fusion models, radiation transport, shielding, consequence studies |
| Astronomy and cosmology | Orbits, stellar evolution, planetary systems, galaxies, large-scale structure |
| Engineering | Materials, structures, fluids, heat transfer, mechanical systems |
| Earth and environmental science | Atmospheres, oceans, climate processes, impact and hazard scenarios |
| Biology and ecology | Cells, populations, ecosystems, interacting biological systems |

The initial implementation should prove the laboratory model with a small number of coherent experiments before expanding into every discipline.

## 3. Architecture

Singularity Laboratories is an experience and platform layer, not a replacement for the scientific engine.

- **Physics** supplies mathematical and physical simulation capabilities.
- **FSM_API** supplies state and interaction foundations where appropriate.
- **FSM_COS** composes the laboratory experience from MicroBundles.
- **FSM_Foundry** provides execution.
- **Renderer** and **Visualization** are consumed through the MicroBundle architecture.
- **ServerApp** can provide persistent worlds, identity, collaboration, and hosted sessions.
- **Economy** can support commerce and virtual-economy integration.

The laboratory layer owns experiment definitions, orchestration, instruments, measurement workflows, result presentation, educational scaffolding, and commercial packaging. It must not duplicate core physics or invent unverified Physics APIs.

See [Architecture](ARCHITECTURE.md) and [Laboratory Model](LABORATORY-MODEL.md).

## 4. Scientific Integrity and Safety

A compelling visualization is not automatically a scientifically valid result. Every experiment must make its model, assumptions, units, parameter ranges, numerical limits, provenance, and uncertainty discoverable.

Hazardous phenomena may be explored as educational simulations of physical processes and consequences. Nuclear-event scenarios, for example, should focus on validated consequence modeling and interpretation — not weapon construction, optimization, or operational instructions. Simulation results must be clearly distinguished from real-world predictions, and high-consequence claims require appropriate validation and expert review.

See [Scientific Integrity and Safety](SCIENTIFIC-INTEGRITY.md).

## 5. Roadmap and Participation

The first milestone is a working laboratory shell and a complete, reproducible demonstration experiment backed by the existing Physics package. The next milestones establish reusable instruments, experiment packages, educational pathways, collaboration, and monetized hosted laboratories.

- [Roadmap](ROADMAP.md)
- [Commercial Model](COMMERCIAL-MODEL.md)

This repository follows the Workshop's two-branch policy: `master` represents the released state and `development` is the active source branch. Work begins on `development`; release alignment and publication remain explicit, gated activities.

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="240" />
</p>
