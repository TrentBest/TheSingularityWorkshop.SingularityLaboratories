# Laboratory Governance, Staffing, and Security

This document distinguishes laboratory-domain governance records from the security controls that a host and its infrastructure must enforce. A role name, review record, or safety declaration is not itself an authorization decision or a security guarantee.

## 1. People and responsibilities

A laboratory needs accountable people, not just experiment definitions. The domain distinguishes responsibilities such as:

| Role | Laboratory responsibility |
|---|---|
| Owner | Accountable for the laboratory's purpose, stewardship, and ownership decisions |
| Administrator | Maintains laboratory configuration, membership, and operational settings |
| SafetyOfficer | Maintains hazard disclosures, precautions, prohibited-use boundaries, and safety review coordination |
| PrincipalInvestigator | Leads a research direction, hypothesis, and scientific work |
| Instructor | Authors and supervises educational activities and learner assignments |
| Technician | Maintains apparatus, instruments, calibration, and operational readiness |
| Researcher | Conducts and documents research within approved laboratory boundaries |

The first staff contract, `LaboratoryStaffAssignment`, records a host-issued opaque principal reference, laboratory identity, responsibility, assigning principal, and optional UTC expiry. It is a durable domain fact to be stored and audited by a host.

Learners, students, guests, external reviewers, and observers are important participants, but they are not automatically staff. Their membership, course enrollment, guest access, and reviewer relationships should be modeled separately as those workflows are implemented. A principal may hold multiple assignments; role combinations and separation-of-duty rules belong in an explicit policy layer.

## 2. Identity, authentication, and authorization

- **Identity** answers who a principal is. A host identity provider supplies the stable opaque principal reference.
- **Authentication** establishes control of an identity (for example, session or credential verification). It is not implemented by the staff assignment contract.
- **Authorization** decides whether that principal may perform a specific action on a specific resource in a specific context. Hosts must enforce it at every relevant service boundary.
- **Staff assignment** records a laboratory responsibility. It does not authenticate a person, issue credentials, or grant access by itself.
- **Entitlement** concerns commercial or institutional access and must not be confused with scientific validation or safety approval.

No passwords, API keys, session tokens, recovery codes, or other secrets belong in laboratory definitions, assignments, experiment configurations, run records, or evidence descriptions.

## 3. Security responsibilities and boundaries

| Area | Laboratory domain responsibility | Host / platform responsibility |
|---|---|---|
| Membership and staffing | Record versioned/traceable responsibility assignments and review history | Verify identities, authorize assignment changes, revoke access, enforce separation of duties |
| Experiment execution | Declare risk, hazards, precautions, prohibited uses, validation evidence, resource estimates, and execution-admission requirements | Enforce policy, isolate workloads, constrain network/filesystem/process access, meter actual resource use, stop runaway work |
| Data and results | Preserve provenance, immutable run identity, configuration snapshots, and disclosure metadata | Encrypt and back up storage, scope access, manage retention/deletion, prevent cross-tenant access |
| Instruments and facilities | Describe apparatus, capabilities, calibration evidence, and operating boundaries | Secure physical devices, device credentials, firmware, network segments, and operator access |
| Collaboration and publication | Record authorship, reviewers, decisions, and publication versions | Protect sessions, invitations, sharing links, moderation, and external integrations |
| Audit | Provide domain events and attributable records for meaningful actions | Protect append-only audit storage, timestamps, access to logs, alerting, and incident response |

A host must not trust a client-side role label or UI visibility as an access control. Authorization belongs on the server or trusted execution boundary, and must be checked for each action and resource. Deny by default when policy is absent or cannot be evaluated.

## 4. Existing controls in this repository

The current foundation already provides several governance and safety-related mechanisms:

- Experiment definitions disclose assumptions, limitations, model identity/version, and scientific validation level.
- Optional safety metadata records a qualitative risk level, hazards, precautions, and prohibited uses.
- Version-bound validation assessments retain evidence, reviewer, decision state, and assessment time. An accepted review requires a named reviewer and evidence.
- Host-owned execution admission policy can require an accepted assessment and enforce a declared maximum risk level. Structured results report all applicable denial reasons.
- Resource profiles and host-owned budgets reject over-budget estimates before model invocation. Estimates are advisory; they are not runtime metering, process isolation, or enforcement.
- Run records preserve the configuration and scientific provenance used for execution; persistence and replay retain source relationships.

These mechanisms are useful domain controls, but none independently certifies an experiment as safe or secures the host environment.

## 5. Important gaps — do not imply these are already solved

The following capabilities remain future work unless and until separately implemented and verified:

- Host-backed identity, authentication, authorization policies, and permission checks.
- Staff assignment persistence, change history, revocation workflows, and role-combination/separation-of-duty policy.
- Learner, guest, reviewer, class-roster, and research-workspace membership models.
- Fine-grained resource scopes (laboratory, facility, apparatus, experiment, dataset, run, and publication).
- Runtime resource metering, sandboxing, isolation, network policy, and reliable cancellation enforcement.
- Data classification, encryption/key management, tenant isolation, retention, deletion, backup, and recovery.
- Instrument/device trust, calibration lifecycle, maintenance records, and physical access control.
- Append-only audit events, security monitoring, incident response, and publication/release approval workflow.
- Formal threat modeling and security testing for each host integration.

## 6. Design rules

1. Keep principal identifiers opaque and host-issued; never embed credentials in domain records.
2. Keep responsibility assignment separate from authentication and authorization.
3. Require explicit policy evaluation at trusted execution and data-access boundaries.
4. Fail closed when a required policy or safety disclosure is missing.
5. Keep scientific validation, peer review, operational safety, and commercial entitlement as separate concepts.
6. Record who changed staffing, safety, experiment, and publication state, when, and why.
7. Preserve historical run provenance when staff assignments or policies change.
8. Treat local, classroom, institutional, private, hosted, and game-hosted laboratories as different threat environments with explicit deployment controls.

## 7. Security posture of the current implementation

The current repository is a host-neutral domain and workflow foundation, not a hardened multi-tenant service. Its contracts can describe staff responsibility, safety disclosures, review evidence, and execution admission. A production host still needs to supply and verify identity, authorization, storage protection, workload isolation, audit infrastructure, and operational security before exposing sensitive or high-consequence laboratories.
