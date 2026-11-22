namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// Assigns a host-identified person a laboratory responsibility within one laboratory.
/// </summary>
/// <remarks>
/// Principal identifiers are opaque references supplied by the host identity system. This
/// contract records responsibility; it does not authenticate the principal, grant permissions,
/// or prove that an external authorization check was performed.
/// </remarks>
public sealed record LaboratoryStaffAssignment
{
    public LaboratoryStaffAssignment(
        string assignmentId,
        string laboratoryId,
        string principalId,
        LaboratoryStaffRole role,
        DateTimeOffset assignedAtUtc,
        string? assignedByPrincipalId = null,
        DateTimeOffset? expiresAtUtc = null)
    {
        AssignmentId = Required(assignmentId, nameof(assignmentId));
        LaboratoryId = Required(laboratoryId, nameof(laboratoryId));
        PrincipalId = Required(principalId, nameof(principalId));
        if (!Enum.IsDefined(role))
            throw new ArgumentOutOfRangeException(nameof(role));
        if (assignedAtUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("Assignment timestamps must be UTC.", nameof(assignedAtUtc));
        if (assignedByPrincipalId is not null && string.IsNullOrWhiteSpace(assignedByPrincipalId))
            throw new ArgumentException("An assigning principal must be non-empty when supplied.", nameof(assignedByPrincipalId));
        if (expiresAtUtc is not null && expiresAtUtc.Value.Offset != TimeSpan.Zero)
            throw new ArgumentException("Expiry timestamps must be UTC.", nameof(expiresAtUtc));
        if (expiresAtUtc is not null && expiresAtUtc.Value <= assignedAtUtc)
            throw new ArgumentException("An assignment must expire after it is created.", nameof(expiresAtUtc));

        Role = role;
        AssignedAtUtc = assignedAtUtc;
        AssignedByPrincipalId = assignedByPrincipalId?.Trim();
        ExpiresAtUtc = expiresAtUtc;
    }

    /// <summary>Stable identity for this assignment record, not for the person.</summary>
    public string AssignmentId { get; }
    public string LaboratoryId { get; }
    /// <summary>Opaque host identity reference; never a password, token, or credential.</summary>
    public string PrincipalId { get; }
    public LaboratoryStaffRole Role { get; }
    public DateTimeOffset AssignedAtUtc { get; }
    public string? AssignedByPrincipalId { get; }
    public DateTimeOffset? ExpiresAtUtc { get; }

    /// <summary>Returns whether the assignment is active at the supplied UTC instant.</summary>
    public bool IsActiveAt(DateTimeOffset instantUtc)
    {
        if (instantUtc.Offset != TimeSpan.Zero)
            throw new ArgumentException("The evaluation timestamp must be UTC.", nameof(instantUtc));

        return instantUtc >= AssignedAtUtc &&
            (ExpiresAtUtc is null || instantUtc < ExpiresAtUtc.Value);
    }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>Laboratory responsibilities; hosts map these to their own authorization policies.</summary>
public enum LaboratoryStaffRole
{
    Owner = 0,
    Administrator = 1,
    SafetyOfficer = 2,
    PrincipalInvestigator = 3,
    Instructor = 4,
    Technician = 5,
    Researcher = 6
}
