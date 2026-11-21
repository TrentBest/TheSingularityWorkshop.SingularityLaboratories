# Scientific Integrity, Validation, and Safety

## Scientific integrity is a product feature

Singularity Laboratories must distinguish an engaging visualization, an educational approximation, an engineering model, and a research-grade simulation. These categories must never be conflated.

Every published experiment should disclose:
- the scientific question and intended use;
- governing assumptions and known omissions;
- equations or model references where appropriate;
- units, valid parameter ranges, and coordinate conventions;
- numerical method, precision, and stability limitations;
- validation evidence and known error bounds;
- uncertainty and sensitivity considerations;
- the model and experiment versions used for each result.

## Validation levels

Use explicit review states rather than a vague claim that a simulation is “accurate.”

- **Concept demonstration:** illustrates a principle; not intended for quantitative prediction.
- **Educational model:** suitable for guided learning within documented limits.
- **Validated model:** compared against appropriate analytical solutions, benchmark cases, or trusted experimental data.
- **Research-reviewed model:** methods, provenance, limitations, and validation have undergone documented domain-expert review.

Validation is model-specific. A laboratory's visual polish or commercial tier must never elevate its scientific validation status.

## Verification practices

Where appropriate, tests should include:
- dimensional consistency and unit conversion checks;
- analytical or known-answer benchmark cases;
- conservation laws and invariants;
- convergence and timestep sensitivity;
- boundary-condition and extreme-value tests;
- deterministic replay checks for deterministic models;
- statistical characterization for stochastic models;
- regression suites tied to model versions;
- independent review for high-consequence applications.

Numerical failure, unsupported parameter combinations, and out-of-domain inputs should produce explicit warnings or fail closed. They must not silently generate authoritative-looking results.

## Hazardous phenomena as educational simulation

The platform may support scientifically grounded exploration of hazardous physical phenomena, including energetic chemical reactions, nuclear processes, radiation, blast effects, and environmental consequences.

The supported experience is simulation, education, and consequence analysis — not real-world construction or operational enablement.

For example, a nuclear-event scenario may let learners examine aggregate outputs such as modeled thermal exposure, overpressure contours, radiation transport, shielding comparisons, and environmental impacts, subject to validated model scope and prominent uncertainty disclosures. It must not provide weapon construction, design optimization, material acquisition, or operational targeting instructions.

Similarly, chemistry experiences may demonstrate energetic reaction behavior and consequences without actionable recipes, synthesis procedures, or device construction instructions.

## Consequence-model safeguards

High-consequence scenarios require:
- documented and reviewed scientific sources;
- conservative interpretation of model uncertainty;
- clear distinction between hypothetical simulation and real-world forecast;
- no false precision or unsupported claims of predictive accuracy;
- review by qualified subject-matter experts before research-grade or public claims;
- controlled publication and access appropriate to the scenario.

The platform should make limitations visible in the experience itself, not hide them in legal text.

## Responsible communication

Results should be presented with context, units, scales, and uncertainty. Avoid sensationalized claims. Dramatic visualizations can make learning memorable, but the scientific interpretation must remain primary.
