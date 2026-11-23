![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Scientific Inquiry and the Scientific Method

**Domain:** Singularity Laboratories  
**Scope:** LOD 1 foundations across disciplines, education, and research

## 1. There is no single universal recipe

Singularity Laboratories must teach the scientific method without teaching a falsehood about science. There is no one fixed sequence that every scientist, discipline, or historical culture follows. Science is a family of disciplined inquiry practices that share commitments to evidence, explicit reasoning, criticism, uncertainty, and correction. Methods differ because questions, objects of study, instruments, ethical limits, and kinds of evidence differ.

A controlled experiment is powerful when variables can be manipulated. It is not the only scientific method. Astronomy, geology, evolutionary biology, paleontology, ecology, epidemiology, climate science, and historical sciences often infer causes from observations, traces, natural experiments, comparative evidence, models, and converging independent lines of evidence.

The familiar cycle—question, hypothesis, experiment, analysis, conclusion—is a useful teaching scaffold, not a universal law or guarantee of truth. Real inquiry is iterative, social, creative, historically situated, and nonlinear.

## 2. A reusable inquiry cycle

Treat these as connected activities, not mandatory one-way gates:

1. **Encounter a phenomenon:** notice an observation, anomaly, pattern, practical problem, or disagreement.
2. **Establish context:** review prior work, theory, terminology, evidence, provenance, and unknowns.
3. **Frame a question:** specify scope, target system or population, time and scale, and intended inference.
4. **Choose an approach:** experiment, observation, survey, field study, comparison, simulation, proof, archival work, or a combination.
5. **Develop candidate explanations:** hypotheses, models, mechanisms, predictions, or competing interpretations.
6. **Design the investigation:** define variables or comparison dimensions, sampling, controls where possible, instruments, analysis, stopping conditions, and ethical constraints.
7. **Collect evidence:** preserve raw observations, metadata, calibration state, units, timestamps, missingness, and deviations.
8. **Analyze and challenge:** apply suitable quantitative, qualitative, computational, mathematical, or interpretive methods.
9. **Interpret proportionately:** state what evidence supports, what it does not, uncertainty, limitations, and alternatives.
10. **Expose work to criticism:** share methods and evidence as permitted; invite independent review and attempts to reproduce or replicate.
11. **Revise and continue:** correct errors, update models, refine questions, publish negative or inconclusive outcomes, or design the next study.
12. **Preserve the record:** archive methods, data lineage, software/model versions, decisions, and publication history.

A project may enter at any point and revisit earlier decisions. Exploratory discovery, confirmatory testing, and synthesis are related but distinct activities.

## 3. Reasoning modes

- **Induction:** infer broader patterns from observations.
- **Deduction:** derive consequences that should follow if premises or a model hold.
- **Abduction:** identify plausible explanations for surprising observations.
- **Analogy:** transfer insight from a known system while testing where the analogy fails.
- **Mathematical and computational reasoning:** prove properties, derive consequences, construct simulations, and examine model behavior.
- **Causal reasoning:** distinguish association from causation using interventions, natural experiments, temporal structure, and domain assumptions.
- **Interpretive reasoning:** analyze meaning, context, testimony, documents, and human practices with explicit methods and reflexivity.

No reasoning mode is sufficient by itself. A mathematically elegant model still needs justified premises and appropriate contact with its target; a large dataset does not interpret itself.

## 4. Investigation designs

| Design family | Typical question | Evidence pattern |
|---|---|---|
| Controlled experiment | What changes when a factor is manipulated? | Intervention, controls, measured outcomes |
| Randomized study | What is the effect of assigned intervention? | Random assignment, outcome comparison |
| Quasi-experiment / natural experiment | What can be inferred without random assignment? | Comparison groups, assumptions |
| Observational / cohort / case-control | What patterns occur in a population? | Systematic observation, sampling |
| Field study | How does a system behave in context? | In-situ observations, field metadata |
| Comparative method | How do systems, groups, sites, or periods differ? | Structured comparisons |
| Longitudinal study | How does a system change? | Repeated measures, temporal provenance |
| Survey / interview / ethnography | What do people report, experience, or do? | Instruments, transcripts, coding |
| Historical / archival study | What happened, and what supports the claim? | Provenance-bearing records, source criticism |
| Paleontological / geological inference | What processes explain preserved traces? | Stratigraphy, dating, comparative evidence |
| Astronomical observation | What distant processes explain received signals? | Instrument observations, calibration, models |
| Mathematical / theoretical study | What follows from stated assumptions? | Proofs, derivations, counterexamples |
| Computational experiment | How does a formal model behave? | Versioned code, parameters, seeds, diagnostics |
| Systematic review / meta-analysis | What does the evidence base collectively indicate? | Search strategy, selection rules, synthesis |

A method is not superior merely because it resembles a school laboratory experiment. Evaluate whether the design can answer the question and whether its assumptions are defensible.

## 5. Hypotheses, models, theories, and laws

These terms are not a ladder where a hypothesis becomes a theory and then a law.

- A **question** identifies what is investigated.
- A **hypothesis** is a testable proposed answer or relationship, where appropriate.
- A **prediction** is an expected observation under specified conditions.
- A **model** is a purposeful representation with scope, assumptions, parameters, and limitations.
- A **theory** is a well-supported explanatory framework that organizes evidence and generates further questions.
- A **law** describes a regularity within a stated domain; it does not automatically explain why it holds.
- A **classification or description** can be a substantial result without a causal hypothesis.

Not every investigation needs a falsifiable hypothesis at the outset. Exploratory, descriptive, qualitative, mathematical, and historical work may have different structures. State what kind of claim is being made and what evidence could support, weaken, or revise it.

## 6. Design quality and protection against self-deception

Before collecting evidence, document decisions that could otherwise be adjusted after seeing results:

- primary question and intended target claim;
- target system, population, site, time window, and sampling frame;
- variables, operational definitions, units, and comparison groups;
- controls, randomization, blinding, and allocation where relevant;
- sample-size rationale and stopping rules;
- exclusions, missing-data handling, and outlier policy;
- primary and secondary outcomes;
- analysis plan and sensitivity checks;
- confounders, biases, and alternative explanations;
- ethical review, consent, privacy, environmental impact, and harm minimization;
- deviations and amendments, time-stamped with reasons.

Preregistration is useful for some confirmatory studies, not a universal requirement. Exploratory analyses should be labeled honestly rather than retroactively presented as specified in advance.

## 7. Measurement is an argument, not just a number

Every observation should carry enough context to interpret it:

- measured, derived, simulated, imputed, estimated, or synthetic status;
- quantity and unit, coordinate frame, reference datum, and sign convention;
- instrument identity, method, calibration evidence, and configuration;
- sampling rate, integration window, resolution, precision, and detection limits;
- timestamp and clock basis;
- uncertainty, variability, and known systematic effects;
- environmental conditions and operator/context metadata;
- transformations, filters, corrections, and software versions;
- missing, censored, saturated, rejected, or anomalous observations.

Precision is not accuracy; resolution is not certainty; calibration does not prove a complete model valid. Derived values must retain lineage to their inputs and transformations.

## 8. Evidence, uncertainty, and inference

Distinguish observation, measurement, analysis result, interpretation, communicated claim, evidence strength, uncertainty, limitation, bias, and error. Statistical significance is not practical importance, causal proof, or the probability that a hypothesis is true. Confidence intervals, credible intervals, prediction intervals, standard errors, and measurement uncertainty answer different questions.

## 9. Criticism, replication, and correction

- **Reproducibility:** obtain consistent computational results using the same data, methods, code, and conditions, as defined by the relevant community.
- **Replicability:** obtain consistent findings in a new study designed to address the same question.
- **Robustness:** test whether conclusions survive reasonable alternative assumptions or methods.
- **Generalizability / transportability:** assess whether findings apply beyond the studied system or conditions.
- **Independent review:** evaluate methods, evidence, analysis, conflicts, and claim scope.
- **Correction and retraction:** update the record when evidence, error, or misconduct warrants it.

Terminology varies by discipline. Define the intended meaning rather than relying on labels alone.

## 10. Platform responsibilities

A future investigation record should link the research question, method/design and protocol version, hypotheses or interpretive framework where applicable, variables and sampling plan, instruments and calibration, ethics and safety evidence, raw-data references, derived-data lineage, run/configuration/model/software versions, deviations and exclusions, analysis outputs, uncertainty, review, replication, correction, publication, and archive records.

Do not collapse all of this into one experiment definition. A field observation, interview study, proof, and computational experiment should share provenance principles without being forced into identical procedures. Introduce typed method-specific contracts only after real use cases establish their invariants.

## 11. Teaching the method

Teach learners to ask: What exactly do we know, and how? What did we observe versus infer? What other explanation could fit? What would change our mind? What did we not measure? Who or what is missing from the sample? How reliable is the instrument or source? What is the uncertainty? Can someone else inspect or repeat the work? What is an honest conclusion if the result is inconclusive?

The goal is not to memorize a diagram. It is to build disciplined curiosity, transparent reasoning, and intellectual humility.

## 12. Existing implementation and next specimens

Existing foundations include versioned experiment definitions, disclosed assumptions and limitations, scientific validation levels, version-bound validation assessments, run provenance, instrument channels, unit metadata, and replay lineage. These are useful pieces, not a complete scientific-investigation management system.

Build concrete specimens before generalizing contracts. Current design specimens include a [field-observation watershed transect](specimens/field-observation-watershed.md), a [historical and archival inquiry](specimens/historical-archival-inquiry.md), and a [computational parameter-sensitivity study](specimens/computational-study.md). The existing electrical-to-thermal Physics experiment supplies the controlled-experiment counterpart. Survey/qualitative inquiry and systematic evidence review remain planned specimens. Their differences should shape the abstractions.

## Further reading

- [Stanford Encyclopedia of Philosophy — Scientific Method](https://plato.stanford.edu/entries/scientific-method/)
- [National Academies — Scientific Methods and Knowledge](https://www.ncbi.nlm.nih.gov/books/NBK547541/)
- [National Academies — Reproducibility and Replicability](https://www.ncbi.nlm.nih.gov/books/NBK547531/)
- [National Academies — Broadening Access to Research Results](https://www.ncbi.nlm.nih.gov/books/NBK525412/)
