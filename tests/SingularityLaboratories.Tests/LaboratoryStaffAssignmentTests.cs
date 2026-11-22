using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class LaboratoryStaffAssignmentTests
{
    private static readonly DateTimeOffset AssignedAt =
        new(2026, 11, 22, 20, 0, 0, TimeSpan.Zero);

    [Test]
    public void Constructor_PreservesLaboratoryResponsibilityAndOpaquePrincipalReferences()
    {
        var assignment = new LaboratoryStaffAssignment(
            "assignment-1", "materials-lab", "host-user-42",
            LaboratoryStaffRole.SafetyOfficer, AssignedAt, "host-admin-7");

        Assert.That(assignment.AssignmentId, Is.EqualTo("assignment-1"));
        Assert.That(assignment.LaboratoryId, Is.EqualTo("materials-lab"));
        Assert.That(assignment.PrincipalId, Is.EqualTo("host-user-42"));
        Assert.That(assignment.Role, Is.EqualTo(LaboratoryStaffRole.SafetyOfficer));
        Assert.That(assignment.AssignedByPrincipalId, Is.EqualTo("host-admin-7"));
    }

    [Test]
    public void IsActiveAt_UsesInclusiveStartAndExclusiveExpiry()
    {
        var expiresAt = AssignedAt.AddHours(2);
        var assignment = new LaboratoryStaffAssignment(
            "assignment-1", "materials-lab", "host-user-42",
            LaboratoryStaffRole.Technician, AssignedAt, expiresAtUtc: expiresAt);

        Assert.That(assignment.IsActiveAt(AssignedAt), Is.True);
        Assert.That(assignment.IsActiveAt(expiresAt.AddTicks(-1)), Is.True);
        Assert.That(assignment.IsActiveAt(expiresAt), Is.False);
        Assert.That(assignment.IsActiveAt(AssignedAt.AddTicks(-1)), Is.False);
    }

    [Test]
    public void Constructor_RejectsInvalidRoleAndInvalidExpiry()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new LaboratoryStaffAssignment(
            "assignment-1", "materials-lab", "host-user-42",
            (LaboratoryStaffRole)999, AssignedAt));

        Assert.Throws<ArgumentException>(() => new LaboratoryStaffAssignment(
            "assignment-2", "materials-lab", "host-user-42",
            LaboratoryStaffRole.Researcher, AssignedAt,
            expiresAtUtc: AssignedAt));
    }

    [Test]
    public void Constructor_RequiresUtcTimestampsAndNonblankIdentityReferences()
    {
        Assert.Throws<ArgumentException>(() => new LaboratoryStaffAssignment(
            "assignment-1", "materials-lab", "host-user-42",
            LaboratoryStaffRole.Researcher,
            new DateTimeOffset(2026, 11, 22, 12, 0, 0, TimeSpan.FromHours(-8))));

        Assert.Throws<ArgumentException>(() => new LaboratoryStaffAssignment(
            "assignment-2", "materials-lab", " ",
            LaboratoryStaffRole.Researcher, AssignedAt));
    }

    [Test]
    public void IsActiveAt_RequiresUtcEvaluationTimestamp()
    {
        var assignment = new LaboratoryStaffAssignment(
            "assignment-1", "materials-lab", "host-user-42",
            LaboratoryStaffRole.Researcher, AssignedAt);

        Assert.Throws<ArgumentException>(() => assignment.IsActiveAt(
            new DateTimeOffset(2026, 11, 22, 12, 0, 0, TimeSpan.FromHours(-8))));
    }
}
