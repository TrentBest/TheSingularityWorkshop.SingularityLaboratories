# Scientific Integrity, Validation, and Safety

Scientific integrity is a product feature. Distinguish an engaging visualization, an educational approximation, an engineering model, and a research-grade simulation. Never conflate them.

## Required disclosures

Every published experiment should disclose its scientific question and intended use; governing assumptions and omissions; equations or model references where appropriate; units and valid parameter ranges; numerical method, precision, and stability limitations; validation evidence and error bounds; uncertainty and sensitivity; and model / experiment versions.

## Validation levels

- **Concept demonstration:** illustrates a principle; not intended for quantitative prediction.
- **Educational model:** suitable for guided learning within documented limits.
- **Validated model:** compared against analytical solutions, benchmarks, or trusted experimental data.
- **Research-reviewed model:** methods, provenance, limitations, and validation have documented domain-expert review.

Validation is model-specific. Visual polish or a commercial tier must never elevate scientific validation status.

## Verification

Where appropriate, test dimensional consistency and unit conversions; analytical or known-answer benchmarks; conservation laws and invariants; convergence and timestep sensitivity; boundary conditions and extreme values; deterministic replay; statistical behavior for stochastic models; versioned regression suites; and independent review for high-consequence applications.

Numerical failure, unsupported parameters, and out-of-domain inputs should warn or fail closed, never silently produce authoritative-looking results.

## Hazardous phenomena as educational simulation

Support scientifically grounded exploration of energetic chemical reactions, nuclear processes, radiation, blast effects, and environmental consequences as simulation, education, and consequence analysis — not real-world construction or operational enablement.

A nuclear-event scenario may examine aggregate outputs such as modeled thermal exposure, overpressure contours, radiation transport, shielding comparisons, and environmental impacts, subject to validated model scope and prominent uncertainty disclosures. It must not provide weapon construction, design optimization, material acquisition, or operational targeting instructions.

Chemistry experiences may demonstrate energetic reaction behavior and consequences without actionable recipes, synthesis procedures, or device construction instructions.

High-consequence scenarios require reviewed scientific sources, conservative uncertainty interpretation, distinction between hypothetical simulation and real-world forecast, no false precision, qualified expert review before research-grade or public claims, and appropriate publication controls.

Make limitations visible in the experience itself. Dramatic visualization can make learning memorable, but scientific interpretation remains primary.
