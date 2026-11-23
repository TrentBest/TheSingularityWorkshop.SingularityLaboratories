# Experiment Physics Settings Dashboard

**Status:** Contract foundation implemented; live host instrumentation and visual UI remain integration work.

Every experiment must expose a settings dashboard that answers two different questions:

1. **What Physics does this experiment declare that it consumes?**
2. **Which Physics MicroBundles did the composition/execution host actually resolve, load, and execute for this run?**

These are separate facts. An experiment definition is an authoring declaration. A MicroBundle execution observation is runtime evidence supplied by the trusted host. A declared dependency must never be displayed as loaded or executing without a corresponding host observation.

## Dashboard sections

### Experiment identity

Show the stable experiment ID, version, display name, model ID/version, and selected configuration schema version.

### Physics consumed

For each declared Physics dependency, show:
- capability ID and discipline/domain;
- model ID and exact model version;
- the experiment purpose for consuming it;
- declared assumptions and limitations from the experiment/model definition;
- configuration inputs relevant to that capability, with units and constraints where available.

The laboratory describes consumption. The Physics repository remains authoritative for physical laws, equations, models, and numerical methods.

### Physics MicroBundles

Show required MicroBundle capabilities separately from runtime observations. For each bundle, display its stable bundle ID, version, provider ID, and purpose.

The runtime panel reports only host-supplied observations: resolved, loaded, executing, completed, failed, or skipped. An empty observation set means **not observed**, not “not required,” “not installed,” or “failed.” Include timestamps and host diagnostics when available. Show unexpected runtime bundles too; do not silently hide them because they were not declared by the experiment.

### Configuration

Expose the actual immutable configuration snapshot used by the selected run:
- parameter name, value, unit, schema, and validation constraints;
- initial and boundary conditions;
- solver/integrator and tolerances when the Physics model supports them;
- random seed and environment details when relevant;
- safety constraints and resource budgets.

Do not invent controls that the underlying model does not expose. Distinguish editable draft settings from the immutable configuration used by an existing run.

### Run and provenance

Bind the dashboard to a selected run identity. Preserve exact experiment/model versions, configuration payload or canonical digest, MicroBundle versions and observed lifecycle, timestamps, outputs, and errors. A later settings change creates a new configuration/run; it must not rewrite history.

## Contract boundaries

- **Singularity Laboratories:** experiment declarations, settings/dashboard projection, scientific context, and run provenance.
- **Physics:** authoritative physical models, equations, assumptions, parameter schemas, and numerical methods.
- **FSM_COS / composition host:** actual MicroBundle resolution and loaded assembly identity.
- **FSM_Execution / execution host:** lifecycle observations, cancellation, and run execution events.
- **Trusted host:** authorization, secrets, and enforcement.
- **Renderer / Visualize:** dashboard presentation only.

## Current implementation boundary

The ExperimentDefinition contract now carries typed Physics dependencies and required MicroBundle capabilities. The ExperimentSettingsDashboard contract combines those declarations with separately supplied MicroBundleExecutionObservation records. This gives every experiment a stable, host-neutral dashboard model without pretending the Laboratory currently sees the live FSM_COS assembly or execution lifecycle.

The next integration step is for the composition/execution host to publish authoritative observations keyed to the experiment run. Then a UI can render the dashboard without guessing from package references or static manifests.