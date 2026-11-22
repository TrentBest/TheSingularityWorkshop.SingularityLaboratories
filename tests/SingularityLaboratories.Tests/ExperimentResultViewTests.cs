using NUnit.Framework;
using TheSingularityWorkshop.SingularityLaboratories.Contracts;

namespace TheSingularityWorkshop.SingularityLaboratories.Tests;

[TestFixture]
public sealed class ExperimentResultViewTests
{
    [Test]
    public void Constructor_ProjectsRunIdentityContentAndDisclosuresWithoutCopyingRecord()
    {
        var started = DateTimeOffset.Parse("2026-11-22T10:00:00+00:00");
        var completed = started.AddSeconds(12);
        var run = new ExperimentRunRecord(
            Guid.NewGuid(),
            "thermal-lab", "1.0.0",
            "thermal-coupling", "2.1.0",
            "physics.thermal-model", "3.0.0",
            1, "application/json", "{\"voltageV\":12}",
            ScientificValidationLevel.EducationalModel,
            started, completed,
            "application/json", "{\"energyJ\":240}",
            ["Ideal fixed-voltage source."],
            ["No spatial temperature gradients."]);

        var view = new ExperimentResultView(run);

        Assert.That(view.Run, Is.SameAs(run));
        Assert.That(view.RunId, Is.EqualTo(run.RunId));
        Assert.That(view.LaboratoryId, Is.EqualTo(run.LaboratoryId));
        Assert.That(view.LaboratoryVersion, Is.EqualTo(run.LaboratoryVersion));
        Assert.That(view.ExperimentId, Is.EqualTo(run.ExperimentId));
        Assert.That(view.ExperimentVersion, Is.EqualTo(run.ExperimentVersion));
        Assert.That(view.ModelId, Is.EqualTo(run.ModelId));
        Assert.That(view.ModelVersion, Is.EqualTo(run.ModelVersion));
        Assert.That(view.ValidationLevel, Is.EqualTo(run.ValidationLevel));
        Assert.That(view.ContentType, Is.EqualTo(run.ResultContentType));
        Assert.That(view.Payload, Is.SameAs(run.ResultPayload));
        Assert.That(view.Elapsed, Is.EqualTo(TimeSpan.FromSeconds(12)));
        Assert.That(view.Assumptions, Is.SameAs(run.Assumptions));
        Assert.That(view.Limitations, Is.SameAs(run.Limitations));
    }

    [Test]
    public void Constructor_RejectsNullRun()
    {
        Assert.Throws<ArgumentNullException>(() => new ExperimentResultView(null!));
    }
}
