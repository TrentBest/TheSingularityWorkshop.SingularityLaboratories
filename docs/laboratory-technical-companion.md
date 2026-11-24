![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Singularity Laboratories — Technical Companion

**Implementation, contracts, evidence, and host integration**

**Development manuscript · Edition 0.1**

> This is the technical companion to [The Living Laboratory — Core Manual](laboratory-manual.md). It explains how the current implementation maps to the domain. It is not the primary reader-facing introduction.

---

## How to use this companion

- **Package consumers:** start with the domain boundary and the experiment lifecycle.
- **Model authors:** inspect experiment definitions, model adapters, parameter schemas, and disclosures.
- **Workflow integrators:** follow the FSM_Execution adapter and campaign persistence sections.
- **Host developers:** use the host responsibility matrix; do not infer security or metering from domain contracts.
- **Reviewers:** use the evidence and verification sections to distinguish source, test, runtime, and scientific claims.

This companion is source-oriented. Exact public types and APIs remain authoritative in the repository source and tests. Links are provided where the current implementation has a stable documentation contract.

## 01 — What: system boundary

Singularity Laboratories owns portable laboratory and experiment semantics: versioned laboratory, facility, instrument, apparatus, measurement-channel, experiment, configuration, campaign, and research-provenance concepts.

It does not own the underlying physical or mathematical models, the general workflow engine, host identity, tenant isolation, billing, physical access control, or presentation frameworks.

| Concern | Owner | Laboratory relationship |
|---|---|---|
| State and core contracts | FSM_API | Uses shared state foundations where required |
| Physical and mathematical models | Physics | Consumes versioned model capabilities through adapters |
| Workflow lifecycle and execution | FSM_Execution | Registers laboratory handlers and receives workflow context |
| Experience composition | FSM_COS | Composes laboratory providers and host experiences |
| Shared roleplaying ontology and LOD composition | SURPS | Optional, opt-in semantic composition; no mandatory runtime dependency |
| General residents and NPC identity | Singularity City | Laboratory-specific roles extend rather than duplicate people |
| Rendering and visualization | Renderer / Visualize / GUI | Presents laboratory state; does not own scientific truth |
| Identity, authorization, tenancy, billing, and enforcement | Trusted host | Supplies policy and enforces permissions and resource controls |

## 02 — Why: the engineering problem

Scientific work needs more than a function that returns a number. A usable platform must preserve the identity of the question, method, model, configuration, execution, and evidence. Otherwise a result can become detached from the conditions that produced it.

The design therefore separates immutable definitions from mutable execution state, model ownership from laboratory orchestration, and scientific admission from host enforcement.

A campaign is not a second experiment engine. It is a versioned plan that invokes the ordinary experiment runner for each scenario and preserves the aggregate outcome.

## 03 — How: implementation map

### 3.1 Definitions and version binding

Laboratory and experiment definitions carry stable identities and explicit versions. Experiment configurations bind to the experiment and model versions they were authored against. Parameter schemas and authoring templates provide reusable, version-bound configuration starting points.

A definition describes what may be run. It does not prove that a host can resolve the dependency, that a model is installed, or that an execution has occurred.

### 3.2 Model adapter boundary

The laboratory runner delegates scientific evaluation to a model adapter. The selected electrical-to-lumped-thermal fixture is owned by Physics; Singularity Laboratories supplies the experiment-level configuration and orchestration boundary.

The adapter must not silently replace a missing model with a locally invented approximation. A model identity/version mismatch is a rejection condition, not an invitation to guess.

### 3.3 Validation and admission

Validation metadata, review assessments, and execution admission are separate:

- **Validation metadata** describes the declared level, scope, assumptions, and limitations.
- **Review assessment** records a version-bound decision against stated criteria and evidence.
- **Admission policy** decides whether a host will allow a particular execution under its configured rules.
- **Scientific validity** remains a claim that must be supported by appropriate evidence.
- **Safety authorization** remains with qualified people, facilities, and trusted enforcement systems.

The runner can reject a run before model invocation when a supplied policy or resource budget is not satisfied. This does not create a general-purpose security sandbox.

### 3.4 Campaign execution lifecycle

A campaign definition contains an objective, an exact experiment-version binding, a required baseline, scenario definitions, measurable acceptance criteria, and declared constraints.

The campaign runner:

1. validates the campaign definition;
2. checks cancellation and host admission conditions;
3. executes the baseline and scenarios through the ordinary experiment runner;
4. preserves each scenario's configuration and explicit outcome;
5. continues after an individual scenario failure where policy permits;
6. records completed, failed, cancelled, and not-run outcomes;
7. returns an immutable campaign execution envelope linked to the original definition.

A successful scenario embeds its run record. A failed or cancelled scenario retains its own outcome without fabricating a successful run. Campaign aggregation does not turn a partial result set into a scientific conclusion.

### 3.5 FSM_Execution integration

The laboratory workflow adapter registers campaign execution using the stable handler and context keys:

| Contract | Value |
|---|---|
| Handler key | `laboratory.executeCampaign` |
| Campaign input key | `laboratory.campaign` |
| Campaign output key | `laboratory.campaignExecution` |

The adapter translates the workflow context into the campaign runner call and can persist the resulting campaign envelope when the host supplies an `IExperimentCampaignExecutionStore`.

FSM_Execution owns workflow lifecycle, cancellation, and execution semantics. The laboratory adapter does not replace that engine.

### 3.6 Durable evidence and replay

The filesystem campaign store writes a complete campaign envelope atomically and rejects duplicate execution identities. The envelope retains the original campaign definition, scenario configurations, successful run records, and explicit non-success outcomes.

Individual run records are persisted through the run store. The campaign and run stores do not currently provide a cross-store transaction. A host that requires atomic consistency across both catalogs must supply an integration strategy and recovery policy.

Version-locked replay reloads the recorded configuration, requires the exact laboratory, experiment, and model versions plus unchanged scientific disclosures, and executes through the ordinary runner. Replay creates a new run linked to its source; it does not overwrite history.

### 3.7 Resource profiles and budgets

Version-bound model resource profiles support pre-run estimates and host budget admission. These estimates are advisory. They are not measured consumption, runtime metering, process isolation, or an enforced cancellation guarantee.

Runtime metering, quotas, sandboxing, and enforceable resource controls remain host/platform work.

## 04 — See it: verification and evidence

The implementation should be evaluated at distinct evidence levels:

| Evidence | What it can establish | What it cannot establish alone |
|---|---|---|
| Source inspection | A contract or code path exists at a revision | That the path executes correctly |
| Unit/integration test | Behavior under the tested conditions | General scientific validity or all host conditions |
| CI workflow result | The exact checked revision passed the declared workflow | That unpublished packages or untested environments work |
| Runtime trace | A particular host observed a particular event | That the event proves scientific correctness |
| Independent validation | Agreement with declared reference evidence within a scope | Universal validity outside that scope |
| Physical measurement | Observations from a real measurement process | Correct interpretation without context and uncertainty |

The campaign and run records are provenance artifacts. Their existence is not, by itself, evidence that the underlying model is accurate.

### Current implementation evidence

The repository's development branch contains:
- versioned laboratory, experiment, configuration, facility, instrument, apparatus, and measurement-channel contracts;
- parameter schemas and reusable authoring templates;
- a Physics-backed experiment adapter and FSM_Execution workflow integration;
- immutable run and campaign records, durable stores, and version-locked replay;
- structured admission results, safety metadata, validation assessments, and advisory resource budgets;
- MicroBundle composition integration through FSM_COS.

The current repository CI is the authority for the exact commit's build and test status. Do not infer a passing build from a source file, README statement, or prior workflow run.

## 05 — Choose your next step

- [Core Manual](laboratory-manual.md) — reader-facing domain and research journey.
- [Virtual Laboratory Service](virtual-laboratory-service.md) — campaign and hosted-service concept.
- [Experiment Physics Settings Dashboard](experiment-physics-settings-dashboard.md) — dependency and runtime evidence boundary.
- [Sourcebook Catalogue](books/index.md) — deeper domain knowledge and publication status.
- [Roadmap](ROADMAP.md) — remaining implementation gates.

---

## Appendix A — Host obligations

A host that operates a laboratory service must define and enforce identity, authentication, authorization, tenant isolation, credential handling, protected audit events, retention/deletion, incident response, network and filesystem isolation, resource metering, cancellation, quotas, scheduling, billing, and physical access where relevant.

A laboratory domain contract does not provide those controls merely by naming them. The host must identify the responsible service, policy, evidence, and operator for each obligation.

## Appendix B — Integration rules

1. Use exact experiment and model identities and versions.
2. Preserve the original configuration and disclosures.
3. Route scientific execution through the owning model provider.
4. Keep workflow lifecycle in FSM_Execution.
5. Keep composition in FSM_COS.
6. Keep presentation out of scientific records.
7. Do not treat a host admission decision as scientific validation.
8. Do not treat virtual results as physical measurements or safety certification.
9. Do not publish packages or promote release branches without explicit release authorization.

## Appendix C — Open engineering questions

- What typed acceptance-criterion model supports comparable metrics, tolerances, and uncertainty without assuming every method is numeric?
- How should campaign analysis report missing, failed, cancelled, and incomparable scenarios?
- What export package preserves definitions, run records, evidence references, and integrity hashes without duplicating large datasets?
- How should runtime metering and cancellation be enforced by a host without confusing estimates with actual consumption?
- What is the smallest stable instrument protocol that supports both simulated and remotely operated devices while preserving the distinction?
- How can the same laboratory identity be presented by multiple hosts with independently verifiable provenance?

---

<p align="center">
  <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="220" />
</p>
