![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Computational Study Specimen — Parameter Sensitivity

**Method family:** Computational / numerical investigation  
**Status:** Study-design specimen; no numerical result is claimed.

## Research question

How sensitive is a model's predicted output to a declared input parameter over a specified range?

The model belongs to its owning scientific domain. Singularity Laboratories records study design, model identity, execution provenance, outputs, and interpretation; it does not reimplement the model.

## Study definition

Record the question and target claim; immutable model identifier/version and source revision; model documentation, assumptions, validity domain, and limitations; parameter definitions, units, ranges, sampling strategy, and fixed inputs; solver/algorithm and numerical settings including tolerances; executable or container identity, runtime, operating system, and dependencies; random-number generator and seed where relevant; input dataset references, versions, licenses, and preprocessing lineage; execution identity, timestamps, diagnostics, exit status, and resource limits; output artifact references, schema, units, and checksums; and analysis method, uncertainty, and interpretation.

A seed alone does not guarantee bitwise-identical output across hardware, runtimes, parallel execution orders, or numerical libraries. Declare the reproducibility target.

## Worked acceptance scenario

Select an existing, explicitly versioned model from its owning domain. Choose one dimensionless input parameter p with documented valid interval [0.5, 1.5]. Hold all other inputs constant and evaluate five predeclared points:

| Run | p |
|---|---:|
| R-01 | 0.50 |
| R-02 | 0.75 |
| R-03 | 1.00 |
| R-04 | 1.25 |
| R-05 | 1.50 |

For each point, preserve exact configuration and model version, numerical settings, execution diagnostics, and output artifact. If a point fails, preserve the failed run and diagnostic; do not silently omit it.

This specimen deliberately does not prescribe the model equation or expected output. Those belong to the selected model's owning domain and validated test fixtures.

## Replay and comparison

A replay request identifies the original run and target:

- **Exact replay:** same inputs and execution environment, with bitwise comparison where supported.
- **Numerical replay:** same scientific configuration with declared numeric tolerances.
- **Methodological reproduction:** independently rerun the documented method, potentially in a different environment.
- **Robustness analysis:** vary justified assumptions or numerical settings and compare conclusions.

A replay links to the original run; it does not overwrite it. Report whether the replay met its declared target and preserve differences.

## Interpretation safeguards

- Simulation output is model output, not direct observation of nature.
- A successful process exit does not validate the model.
- A smooth curve does not establish numerical convergence.
- Sensitivity within the selected range does not establish behavior outside it.
- Failed, unstable, non-convergent, and out-of-domain runs remain discoverable.
- Derived plots and statistics link to source outputs and analysis version.

## Acceptance checks

- [ ] Model version and source revision are immutable and traceable.
- [ ] Every parameter has a declared meaning, unit, and allowed domain.
- [ ] Environment, dependencies, solver settings, and seeds are preserved where relevant.
- [ ] Failed runs and diagnostics remain in the study record.
- [ ] Output artifacts link to exact input configurations.
- [ ] Replay declares exact, numerical, methodological, or robustness intent.
- [ ] Comparison tolerance is explicit and justified.
- [ ] Conclusions stay within the model's documented validity domain.

## Ownership

The model-owning domain owns equations, numerical methods, and model validation. FSM_Execution owns workflow lifecycle and cancellation. Singularity Laboratories owns study plans, run grouping, provenance, replay relationships, and interpretation records.
