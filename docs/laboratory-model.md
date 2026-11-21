# Laboratory and Experiment Model

## A laboratory is more than a scene

A laboratory is a versioned, composable environment that brings together a scientific model, instruments, experiment workflows, educational context, and one or more presentation modes.

A laboratory definition should identify:
- its discipline and learning or research purpose;
- supported experiment templates and model versions;
- instruments and measurement capabilities;
- supported units, ranges, and coordinate systems;
- required MicroBundles and optional integrations;
- fidelity levels and resource profiles;
- safety classification and review status;
- licensing, visibility, and collaboration rules.

## Core concepts

| Concept | Meaning |
|---|---|
| Laboratory | A discoverable environment in which experiments can be conducted |
| Experiment | A question or investigation with defined inputs and expected observations |
| Model | A versioned scientific representation used to calculate outcomes |
| Instrument | A measurement or interaction capability exposed to an experiment |
| Configuration | The complete set of inputs and initial conditions for one run |
| Run | One execution of a specific experiment configuration |
| Observation | A measured or derived value, with units and provenance |
| Result | A durable record of outputs, diagnostics, and model metadata |
| Lesson | Guided educational context around an experiment |
| Publication | A versioned, reviewable release of a laboratory or experiment |

## Example experiences

### Particle physics

A collider laboratory can teach beam concepts, detector geometry, collision visualization, and interpretation of simulated event data. The model must state what is physically represented and what is simplified. A large virtual collider is a compelling environment, but its visual circumference does not itself establish accelerator fidelity.

### Chemistry

Learners can assemble molecules, vary reaction conditions within a model's supported domain, observe energy changes, and compare reaction pathways. Energetic reactions may be represented through bounded, educational models that explain energy transfer and consequences without providing actionable synthesis or explosive construction instructions.

### Nuclear science

Experiments can explore fission and fusion concepts, radiation transport, shielding, and environmental consequences. High-consequence event scenarios should use vetted aggregate consequence models, clearly disclose uncertainty and limitations, and avoid weapon-design or optimization functionality.

### Astronomy

Users can configure planetary systems, observe orbital behavior, investigate stellar lifecycles, and explore galaxy-scale visualizations. Model fidelity should be explicit: an educational N-body demonstration and a research-grade simulation are different offerings.

## Reproducibility

Every run should capture, as applicable:
- experiment and laboratory identifiers and versions;
- scientific model and adapter versions;
- all input values with units;
- initial and boundary conditions;
- random seeds and stochastic settings;
- solver, timestep, precision, and convergence settings;
- execution environment and resource profile;
- outputs, observations, warnings, and diagnostics;
- timestamps and authorship / collaboration provenance.

## Educational progression

Experiments should be approachable at multiple levels without fragmenting the scientific source of truth:
1. **Explore:** guided observation and safe parameter changes.
2. **Investigate:** formulate a question, control variables, and compare runs.
3. **Analyze:** inspect measurements, units, uncertainty, and model assumptions.
4. **Design:** author a new experiment from approved templates and components.
5. **Research:** use reviewed models, documented methods, reproducible configurations, and exportable results.

A lesson should teach how to reason about a model, not merely how to operate its controls.
