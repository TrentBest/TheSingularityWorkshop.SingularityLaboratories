namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>A portable, versioned scientific workspace definition independent of its host presentation.</summary>
public sealed record LaboratoryDefinition
{
    public LaboratoryDefinition(string id, string version, string name, string description)
    {
        Id = Required(id, nameof(id));
        Version = Required(version, nameof(version));
        Name = Required(name, nameof(name));
        Description = Required(description, nameof(description));
    }

    public string Id { get; }
    public string Version { get; }
    public string Name { get; }
    public string Description { get; }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}
