using System.Text.Json;

namespace TheSingularityWorkshop.SingularityLaboratories.Contracts;

/// <summary>
/// A versioned, host-neutral schema for the scalar parameters of a JSON experiment configuration.
/// The schema describes accepted input; scientific model code remains authoritative for physical validity.
/// </summary>
public sealed record ExperimentParameterSchema
{
    public ExperimentParameterSchema(
        string experimentId,
        string experimentVersion,
        int schemaVersion,
        IEnumerable<ExperimentParameterDefinition> parameters)
    {
        ExperimentId = Required(experimentId, nameof(experimentId));
        ExperimentVersion = Required(experimentVersion, nameof(experimentVersion));
        ArgumentOutOfRangeException.ThrowIfLessThan(schemaVersion, 1);
        SchemaVersion = schemaVersion;
        ArgumentNullException.ThrowIfNull(parameters);
        var snapshot = parameters.ToArray();
        if (snapshot.Length == 0)
            throw new ArgumentException("A parameter schema must declare at least one parameter.", nameof(parameters));
        if (snapshot.Any(parameter => parameter is null))
            throw new ArgumentException("A parameter schema cannot contain null entries.", nameof(parameters));
        if (snapshot.Select(parameter => parameter.Id).Distinct(StringComparer.Ordinal).Count() != snapshot.Length)
            throw new ArgumentException("Parameter identifiers must be unique.", nameof(parameters));
        Parameters = Array.AsReadOnly(snapshot);
    }

    public string ExperimentId { get; }
    public string ExperimentVersion { get; }
    public int SchemaVersion { get; }
    public IReadOnlyList<ExperimentParameterDefinition> Parameters { get; }

    /// <summary>Rejects malformed JSON, missing required values, unknown keys, and type/range violations.</summary>
    public void ValidateJson(string payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        using var document = JsonDocument.Parse(payload);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            throw new ArgumentException("Experiment configuration must be a JSON object.", nameof(payload));

        var declared = Parameters.ToDictionary(parameter => parameter.Id, StringComparer.Ordinal);
        var supplied = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (!supplied.Add(property.Name))
                throw new ArgumentException($"Configuration parameter '{property.Name}' appears more than once.", nameof(payload));
            if (!declared.TryGetValue(property.Name, out var definition))
                throw new ArgumentException($"Unknown configuration parameter '{property.Name}'.", nameof(payload));
            definition.Validate(property.Value, nameof(payload));
        }

        foreach (var parameter in Parameters)
        {
            if (parameter.Required && !supplied.Contains(parameter.Id))
                throw new ArgumentException($"Required configuration parameter '{parameter.Id}' is missing.", nameof(payload));
        }
    }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>One named, typed parameter accepted by an experiment configuration schema.</summary>
public sealed record ExperimentParameterDefinition
{
    public ExperimentParameterDefinition(
        string id,
        string name,
        string description,
        ExperimentParameterType type,
        bool required = true,
        string? unitSymbol = null,
        double? minimum = null,
        double? maximum = null,
        IEnumerable<string>? allowedValues = null)
    {
        Id = Required(id, nameof(id));
        Name = Required(name, nameof(name));
        Description = Required(description, nameof(description));
        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type));
        if (minimum is { } min && !double.IsFinite(min))
            throw new ArgumentOutOfRangeException(nameof(minimum));
        if (maximum is { } max && !double.IsFinite(max))
            throw new ArgumentOutOfRangeException(nameof(maximum));
        if (minimum is { } lower && maximum is { } upper && lower > upper)
            throw new ArgumentException("Minimum cannot exceed maximum.");
        if ((minimum is not null || maximum is not null) && type is not (ExperimentParameterType.Number or ExperimentParameterType.Integer))
            throw new ArgumentException("Numeric bounds require a numeric parameter type.");
        if (unitSymbol is not null && string.IsNullOrWhiteSpace(unitSymbol))
            throw new ArgumentException("A unit symbol must be non-empty when supplied.", nameof(unitSymbol));

        var values = allowedValues?.Select(value => Required(value, nameof(allowedValues))).ToArray() ?? [];
        if (values.Distinct(StringComparer.Ordinal).Count() != values.Length)
            throw new ArgumentException("Allowed values must be unique.", nameof(allowedValues));
        if (values.Length > 0 && type is not (ExperimentParameterType.String or ExperimentParameterType.Enumeration))
            throw new ArgumentException("Allowed values require a string or enumeration parameter type.", nameof(allowedValues));
        if (type == ExperimentParameterType.Enumeration && values.Length == 0)
            throw new ArgumentException("Enumeration parameters must declare allowed values.", nameof(allowedValues));
        if (type != ExperimentParameterType.Enumeration && values.Length > 0 && type != ExperimentParameterType.String)
            throw new ArgumentException("Only string and enumeration parameters can declare allowed values.", nameof(allowedValues));

        Type = type;
        Required = required;
        UnitSymbol = unitSymbol?.Trim();
        Minimum = minimum;
        Maximum = maximum;
        AllowedValues = Array.AsReadOnly(values);
    }

    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public ExperimentParameterType Type { get; }
    public bool Required { get; }
    public string? UnitSymbol { get; }
    public double? Minimum { get; }
    public double? Maximum { get; }
    public IReadOnlyList<string> AllowedValues { get; }

    internal void Validate(JsonElement value, string parameterName)
    {
        var valid = Type switch
        {
            ExperimentParameterType.String or ExperimentParameterType.Enumeration => value.ValueKind == JsonValueKind.String,
            ExperimentParameterType.Number => value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out var number) && double.IsFinite(number),
            ExperimentParameterType.Integer => value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out _),
            ExperimentParameterType.Boolean => value.ValueKind is JsonValueKind.True or JsonValueKind.False,
            _ => false
        };
        if (!valid)
            throw new ArgumentException($"Configuration parameter '{Id}' must be {Type}.", parameterName);

        if (Type == ExperimentParameterType.Number)
        {
            var number = value.GetDouble();
            if (Minimum is { } min && number < min || Maximum is { } max && number > max)
                throw new ArgumentException($"Configuration parameter '{Id}' is outside its declared numeric range.", parameterName);
        }
        else if (Type == ExperimentParameterType.Integer)
        {
            var number = value.GetInt64();
            if (Minimum is { } min && number < min || Maximum is { } max && number > max)
                throw new ArgumentException($"Configuration parameter '{Id}' is outside its declared numeric range.", parameterName);
        }
        else if (Type is ExperimentParameterType.String or ExperimentParameterType.Enumeration &&
                 AllowedValues.Count > 0 && !AllowedValues.Contains(value.GetString()!, StringComparer.Ordinal))
        {
            throw new ArgumentException($"Configuration parameter '{Id}' is not one of its declared allowed values.", parameterName);
        }
    }

    private static string Required(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        return value.Trim();
    }
}

/// <summary>Primitive JSON value kinds supported by the portable parameter schema.</summary>
public enum ExperimentParameterType
{
    String = 0,
    Number = 1,
    Integer = 2,
    Boolean = 3,
    Enumeration = 4
}
