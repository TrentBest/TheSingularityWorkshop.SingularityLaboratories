# Laboratory and Experiment Model

A laboratory is a versioned, composable environment combining a scientific model, instruments, experiment workflows, educational context, and presentation modes.

## Laboratory definition

A laboratory definition identifies its discipline and purpose; supported experiment templates and model versions; instruments and measurements; units, ranges, and coordinate systems; required MicroBundles and optional integrations; fidelity and resource profiles; safety classification and review status; licensing, visibility, and collaboration rules.

## Core concepts

| Concept | Meaning |
|---|---|
| Laboratory | Discoverable environment in which experiments are conducted |
| Experiment | Investigation with defined inputs and expected observations |
| Model | Versioned scientific representation used to calculate outcomes |
| Instrument | Measurement or interaction capability |
| Configuration | Complete inputs and initial conditions for one run |
| Run | One execution of a specific configuration |
| Observation | Measured or derived value, with units and provenance |
| Result | Durable outputs, diagnostics, and model metadata |
| Lesson | Guided educational context |
| Publication | Versioned, reviewable release of a laboratory or experiment |

## Example experiences

### Particle physics
A collider laboratory can teach beam concepts, detector geometry, collision visualization, and interpretation of simulated event data. State what is represented and what is simplified. A large virtual collider is compelling, but its visual circumference does not establish accelerator fidelity.

### Chemistry
Learners can assemble molecules, vary conditions within a model's supported domain, observe energy changes, and compare reaction pathways. Energetic reactions may be represented through bounded educational models without actionable synthesis or explosive-construction instructions.

### Nuclear science
Explore fission and fusion concepts, radiation transport, shielding, and environmental consequences. High-consequence scenarios use vetted aggregate consequence models, disclose uncertainty, and avoid weapon-design or optimization functionality.

### Astronomy
Configure planetary systems, observe orbital behavior, investigate stellar lifecycles, and explore galaxy-scale visualizations. Educational demonstrations and research-grade simulations must be labeled distinctly.

## Reproducibility

Capture experiment and laboratory identifiers and versions; scientific model and adapter versions; input values with units; initial and boundary conditions; random seeds and stochastic settings; solver, timestep, precision, and convergence settings; execution environment and resource profile; outputs, warnings, and diagnostics; and authorship / collaboration provenance.

## Educational progression

1. **Explore:** guided observation and safe parameter changes.
2. **Investigate:** formulate a question, control variables, compare runs.
3. **Analyze:** inspect measurements, units, uncertainty, assumptions.
4. **Design:** author experiments from approved templates and components.
5. **Research:** use reviewed models, documented methods, reproducible configurations, exportable results.

Teach how to reason about a model, not merely operate its controls.
