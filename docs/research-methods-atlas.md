![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Research Methods Atlas

**Purpose:** A broad map of how disciplines produce, test, interpret, and revise knowledge.  
**Companion:** [Scientific Inquiry and the Scientific Method](scientific-method.md)

## 1. Why an atlas?

A laboratory platform should not assume every investigation is a bench experiment. It must support the diversity of scientific work while preserving common provenance, evidence, and integrity principles. Entries are research directions, not claims that every capability exists in the current runtime.

## 2. Methods by evidence and activity

### Experimental
Controlled experiments; randomized studies; factorial and fractional-factorial designs; crossover and repeated-measures designs; dose-response and response-surface studies; design of experiments; parameter sweeps; benchmarks and reference materials; inter-laboratory comparisons; blind procedures where appropriate; instrument characterization; replication and robustness studies.

### Observational and field
Naturalistic observation; field sampling and transects; remote sensing; ecological monitoring; geological mapping and stratigraphy; astronomical observation; oceanographic and atmospheric measurement; archaeological survey; longitudinal cohorts; case-control and cross-sectional studies; natural experiments and quasi-experimental designs.

### Human and social research
Surveys; structured questionnaires; interviews; oral histories; focus groups; ethnography; participant observation; case studies; comparative historical analysis; participatory and community-based research; behavioral experiments; psychometrics; mixed methods; qualitative coding, thematic analysis, and discourse analysis.

Human-subject work requires appropriate ethics review, consent, privacy protections, risk assessment, and community governance. Software configuration does not grant ethical approval.

### Mathematical, theoretical, and computational
Mathematical proof; formal verification; analytical modeling; numerical simulation; agent-based modeling; discrete-event and system-dynamics modeling; statistical inference; Bayesian analysis; causal inference; optimization; sensitivity analysis; machine learning; computational experiments; digital twins with explicit validation scope; formal methods and model checking.

Computational outputs are conditional on model assumptions, implementation, data, parameters, numerical methods, and execution environment. A simulation is not a measurement of the world.

### Historical, archival, and documentary
Primary-source criticism; manuscript and textual analysis; provenance and chain-of-custody research; historical comparison; material culture analysis; archival sampling; oral-history methods; translation and edition comparison; instrument reconstruction; historiography.

### Synthesis and evidence review
Systematic literature review; scoping review; meta-analysis; evidence mapping; umbrella review; qualitative evidence synthesis; living review; standards and consensus review; reanalysis of published datasets.

A synthesis preserves search strategies, inclusion/exclusion criteria, source quality, heterogeneity, and conflicts of interest.

## 3. Method selection matrix

| Question | Candidate approaches | Common trap |
|---|---|---|
| Does intervention X change outcome Y? | Randomized or quasi-experimental design | Confounding, outcome switching |
| What occurs in this system? | Descriptive observation, field study, survey | Sampling bias, missing context |
| What caused this event? | Causal inference, process tracing, natural experiment | Correlation mistaken for causation |
| How does it change? | Longitudinal study, time series | Temporal confounding, instrument drift |
| What existed in the past? | Historical, geological, archaeological inference | Presentism, weak provenance |
| What is too distant to manipulate? | Astronomy, remote sensing, observational modeling | Model dependence, selection effects |
| What follows from assumptions? | Mathematics, logic, formal methods | Unstated premises, invalid proof |
| How does this model behave? | Computational experiment, numerical analysis | Model output treated as empirical truth |
| What do people experience? | Qualitative, ethnographic, interview methods | Extractive practice, reflexivity gaps |
| What does the evidence base show? | Systematic review, meta-analysis | Publication bias, selective search |

## 4. Discipline coverage matrix

| Discipline family | Representative methods and instruments |
|---|---|
| Physics | Controlled experiment, interferometry, spectroscopy, detectors, numerical simulation |
| Chemistry | Analytical chemistry, spectroscopy, chromatography, calorimetry, kinetics |
| Materials science | Mechanical testing, microscopy, diffraction, fatigue studies |
| Biology | Microscopy, field ecology, genetics, population studies |
| Medicine and health | Clinical trials, cohorts, diagnostic accuracy, epidemiology, qualitative research |
| Earth science | Stratigraphy, geochemistry, geophysics, paleoclimate proxies, field mapping |
| Climate and atmosphere | Sensor networks, satellite retrieval, coupled models, attribution |
| Oceanography | Moorings, CTD profiles, sampling campaigns, remote sensing |
| Astronomy | Imaging, spectroscopy, astrometry, time-series and survey science |
| Engineering | Design of experiments, reliability, metrology, stress analysis |
| Computer science | Formal proof, benchmarks, empirical software studies, algorithm analysis, HCI |
| Mathematics | Proof, counterexample, construction, computational exploration |
| Psychology | Experiments, surveys, psychometrics, longitudinal and qualitative methods |
| Sociology and economics | Natural experiments, surveys, administrative data, ethnography |
| Archaeology and history | Excavation, dating, archival research, material culture, source criticism |
| Linguistics | Corpus analysis, field methods, experiments, comparative and computational methods |
| Agriculture | Field trials, soil sampling, breeding studies, remote sensing, agroecology |
| Environmental science | Monitoring, toxicology, ecosystem assessment, life-cycle analysis |
| Geography | GIS, spatial statistics, field surveys, remote sensing, participatory mapping |
| Education research | Classroom observation, intervention studies, assessment validation, mixed methods |

## 5. Cross-cutting capabilities

Plan for protocol and method versioning; question and claim tracking; sample, specimen, participant, and site registries where appropriate; sampling plans; instrument and data-source provenance; unit, coordinate, temporal, and reference-system metadata; raw-to-derived data lineage; qualitative and quantitative artifacts; uncertainty, bias, missingness, and sensitivity reporting; ethics and governance; collaboration and review; reproducibility packages; replication links; corrections and retractions.

Not every method uses every field. Contracts should be composable and typed rather than forcing every study to fill irrelevant fields.

## 6. Validate before generalizing

Build six distinct specimens: (1) controlled physical experiment with instruments and repeatable runs; (2) field ecology study with sites, samples, observers, and seasonal visits; (3) human-subject interview study with consent and restricted records; (4) historical archive study with source provenance and interpretation; (5) computational parameter study with software and random-seed provenance; (6) systematic review with search and selection history.

Only then extract shared contracts for project identity, protocol versioning, evidence references, method-specific metadata, and publication lineage. The first concrete specimens are the [field-observation watershed transect](specimens/field-observation-watershed.md), [historical and archival inquiry](specimens/historical-archival-inquiry.md), and [computational parameter study](specimens/computational-study.md). The existing [electrical-to-thermal Physics experiment](../README.md) provides the controlled-experiment counterpart. Each specimen states its evidence model, limits, acceptance checks, and domain ownership.
