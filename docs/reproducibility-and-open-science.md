![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Reproducibility, Replication, and Open Science

## 1. Trust is built from inspectable work

A result should not be an isolated number, chart, or image. Trace the claim to the analysis, analysis to derived data, derived data to source observations, and observations to the method and context that produced them.

Scientific knowledge advances through discovery, confirmation, and correction. Reproducibility and replicability are central to reliability, but terminology differs among disciplines. Each study should define its operational meaning.

## 2. Preserve the research object graph

A complete research package may link project and question; protocol and method version; experiment, study, or observation definition; model, software, dependency, and instrument versions; configuration, parameters, units, and initial conditions; sample, specimen, site, participant, or population references; raw-data identifiers and provenance; derived datasets and transformation lineage; analysis code, environment, dependencies, and random seeds; results, uncertainty, diagnostics, and limitations; review, publication, correction, and replication records.

Not every investigation has a physical instrument or executable experiment. The graph must support method-specific nodes.

## 3. Computational reproducibility package

Preserve as appropriate source code and immutable revision identifiers; dependency lockfiles and toolchain versions; data references, checksums, licenses, and access conditions; configuration and parameters; random seeds and stochastic settings; operating system, hardware, accelerator, and precision details where material; solver, compiler, numerical library, and model versions; execution logs and warnings; regeneration instructions; expected tolerances and known nondeterminism.

A container or environment snapshot can help, but does not establish scientific validity or guarantee bit-for-bit identity across hardware.

## 4. Replication and robustness

A replication is a new investigation designed to address a prior finding. Document what is retained and what changes: population, sample, site, instrument, implementation, analysis, or context.

Robustness analysis asks whether a conclusion changes under reasonable alternatives: model specifications, exclusion rules, priors, preprocessing, uncertainty assumptions, and independent measurement techniques.

A failed replication is not automatically proof of fraud or proof that the original finding was false. Differences in power, context, implementation, measurement, and target population require investigation.

## 5. Open science is governed sharing

Openness is not indiscriminate publication. Some records must be restricted to protect people, communities, endangered species, sensitive locations, security, intellectual property, or legitimate confidentiality.

A publication should disclose what data and code are available; where and under what license; what is withheld and why; how access can be requested; whether data are synthetic, anonymized, aggregated, or restricted; and reuse limitations and attribution requirements.

Community-owned and Indigenous knowledge requires appropriate governance and consent. Open-by-default must not override community authority or cultural restrictions.

## 6. Negative, null, and corrective results

Preserve inconclusive outcomes, null results, failed experiments and technical failures, protocol deviations, unsuccessful replications, corrections to datasets and analysis, retractions, and superseded conclusions.

A failed run is not the same as a failed hypothesis. A null result is not proof of no effect. Termination reasons and limits of inference remain visible.

## 7. Proposed platform behaviors

Plan for immutable run snapshots and replay lineage; versioned protocols and model references; content hashes; data dictionaries and machine-readable metadata; export manifests describing dependencies and access constraints; correction and supersession links rather than destructive edits; review states distinct from scientific validation level; audit history for publication and access decisions.

Existing run records preserve experiment/model versions, configuration and result payloads, assumptions, limitations, UTC timestamps, and optional replay source. They are a foundation, not a complete research-object repository.

## Further reading

- [National Academies — Reproducibility and Replicability in Science](https://www.ncbi.nlm.nih.gov/books/NBK547531/)
- [National Academies — Broadening Access to the Results of Scientific Research](https://www.ncbi.nlm.nih.gov/books/NBK525412/)
