![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Field Observation Specimen — Watershed Transect

**Method family:** Observational / field research  
**Status:** Teaching and contract-design specimen; not a deployed field-data service.

## Research question

How do measured water temperature and visible stream conditions vary along a short watershed transect during a documented visit?

This is descriptive. It does not establish pollution, ecological health, causal effects, or long-term trends.

## Investigation design

A field team defines three fictional teaching stations:

| Station | Relative position | Intended observation |
|---|---|---|
| W-01 | Upstream reference | Water temperature, flow description, weather |
| W-02 | Mid-transect | Water temperature, flow description, nearby disturbance notes |
| W-03 | Downstream reference | Water temperature, flow description, weather |

The labels are not real locations. A real visit must record coordinates, access permissions, date, time zone, and sampling rationale.

## Evidence record

Each observation carries a stable observation and study ID; station ID and site description; observer or governed pseudonym; timestamp, time zone, and clock provenance; quantity, value, unit, and observation type; instrument and calibration reference when applicable; method/protocol version and sampling effort; environmental context; quality flags and uncertainty; missingness reason; and source attachment references.

A verbal description such as “flow looked fast” is a qualitative observation, not a numeric flow-rate measurement. A sensor reading is a measurement and retains instrument and calibration context. Never silently convert one evidence kind into another.

## Worked teaching record (synthetic)

All values below are invented solely to demonstrate record shape.

| Station | Water temperature | Flow description | Record status |
|---|---:|---|---|
| W-01 | 12.4 °C | Moderate, visibly turbulent | Observed |
| W-02 | 13.1 °C | Slow margin; faster central channel | Observed |
| W-03 | — | Access route blocked | Not observed |

W-03 is not zero and is not an imputed temperature. Preserve the missing observation and its reason. Do not interpolate unless a separate analysis justifies doing so.

## Analysis and limitations

A teaching analysis may compare the two available temperatures. It must disclose that only two of three planned stations yielded a temperature; one visit cannot establish a seasonal pattern; station conditions, time, weather, depth, sensor placement, and calibration may affect comparability; and the table is synthetic, not environmental evidence.

The defensible conclusion is limited to recorded observations. Claims about pollution, ecological condition, or causation require a different design and additional evidence.

## Safety and stewardship

Real fieldwork requires site permission, an appropriate risk assessment, safe water-edge practices, environmental-impact minimization, and required collection permits. Do not publish sensitive species locations, private land details, or community-held knowledge without appropriate governance.

## Acceptance checks

- [ ] Every observation resolves to a study and station.
- [ ] Measurements carry units and instrument/calibration context where applicable.
- [ ] Qualitative descriptions remain distinct from measurements.
- [ ] A missed station remains explicitly missing with a reason.
- [ ] Protocol, visit context, observer, and attachments are traceable.
- [ ] Analysis reports sampling limits and does not claim causation from description.

## Ownership

Singularity Laboratories owns the investigation record and evidence provenance. Physics may provide scientific models but does not own field-site sampling records. Hosts may present maps without becoming the authority for scientific evidence.
