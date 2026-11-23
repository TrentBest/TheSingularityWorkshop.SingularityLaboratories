# Experiment Provider Specimens

**Status:** architecture specimens. The electrical/thermal specimen is source-backed by the existing adapter and declaration. The secure-door specimen is a proposed, deliberately different experiment; it is not yet an implemented model or a registered MicroBundle.

These specimens demonstrate one reusable rule: an experiment declares what it needs, while the trusted host separately reports what actually resolved, loaded, and executed for a particular run.

## Specimen A — electrical-to-thermal coupling

**Kind:** physical model coupling.

The current adapter invokes Physics models directly. Its experiment declaration identifies the electrical and thermodynamic capabilities and exact model versions consumed. It does not claim that a Physics MicroBundle is executing, because this adapter path does not currently resolve those models through one.

Dashboard sections:
- Physics consumed: electrical resistive-load capability; lumped thermodynamic state capability.
- MicroBundle requirements: none declared for the current direct-reference adapter path.
- Runtime observations: empty until a host reports actual MicroBundle activity for a concrete run.
- Configuration: voltage, resistance/load, thermal mass/heat capacity, initial temperature, and model-specific parameters when exposed by the model.
- Provenance: exact experiment/model versions, immutable configuration, run identity, assumptions, limitations, outputs.

This is the baseline specimen for a scientific experiment that consumes Physics directly.

## Specimen B — secure laboratory door

**Kind:** security and gesture-recognition experiment.

This is intentionally not another Physics model. It evaluates whether a fictional laboratory access policy accepts a player's presented evidence under a configured challenge. It can consume multiple non-Physics providers:

| Capability category | Declared role | Dashboard evidence |
|---|---|---|
| Mechanical access challenge | Abstract lock interaction and door state | Challenge rules, attempts, elapsed simulation time, result |
| Gesture recognition | Interpret an in-game hand/body gesture | Gesture provider identity/version, confidence policy, accepted/rejected event |
| Identity verification | Compare a synthetic in-game identity claim with the authorized role | Identity provider identity/version, match result, reason code |
| Cryptographic freshness | Validate a fictional rotating challenge token | Challenge identifier, freshness result, replay rejection result |
| Physics (optional) | Model a physical door/actuator only if the experiment actually consumes such a model | Exact Physics capability/model/version and relevant parameters |

No stable production bundle IDs or provider IDs are assigned by this specimen. They must be sourced from the actual provider registries and composition manifests before being added to a production experiment definition.

### Example experiment progression

1. **Basic lock:** only the abstract mechanical challenge is active. No Gesture or biometric provider is implied.
2. **Gesture-assisted access:** add a registered Gesture provider requirement. The dashboard reports the configured gesture policy separately from the host's actual provider observations.
3. **Multi-factor fictional access:** add synthetic identity and rotating challenge-response providers. Retina/palm are fictional signals or generated tokens; never collect a player's real biometric data.
4. **Adaptive security:** add staff role, time window, facility zone, and risk policy. The experiment explains which evidence contributed to the decision.

The player-facing challenge is a game mechanic, not a real-world lock-picking, biometric-bypass, or cryptographic-attack tutorial.

### Dashboard distinction

- **Declared:** what this experiment requires by design.
- **Resolved:** what the composition host selected.
- **Loaded:** what the host actually loaded.
- **Executing:** what the execution host reports running.
- **Completed / failed / skipped:** terminal or intentionally bypassed lifecycle facts.
- **Not observed:** no authoritative runtime event has been supplied; this is not proof of absence.

Unexpected runtime providers must remain visible. A declared provider with no matching observation is not silently promoted to “loaded”; an observed but undeclared provider is not silently hidden.

## Acceptance rules shared by both specimens

- Every runtime observation belongs to one concrete non-empty RunId.
- A dashboard without a RunId is an authoring preview and contains no runtime observations.
- Declarations and observations retain exact version and provider identity.
- The dashboard does not infer runtime execution from a project reference, package dependency, manifest declaration, or experiment definition.
- A settings change creates a new configuration/run; historical run evidence remains immutable.
- The provider's owning repository remains authoritative for its behavior and version semantics.
- The host is authoritative for what it actually resolved, loaded, and executed.

## Ownership

- Singularity Laboratories owns experiment declarations, scientific/security experiment context, dashboard projection, and run provenance.
- Physics owns physical laws and physical models.
- Gestures owns gesture vocabulary, temporal recognition, and gesture-provider behavior.
- FSM_COS owns MicroBundle composition and actual loaded assembly identity.
- FSM_Execution owns execution lifecycle and run events.
- The trusted host owns access enforcement, secrets, and authorization.
- Renderer/Visualize own presentation, not evidence or authority.
