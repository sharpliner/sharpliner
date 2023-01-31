using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#variables
public abstract record VariableBase;

public record VariableGroup : VariableBase
{
    [YamlMember(Alias = "group")]
    public string Name { get; }

    public VariableGroup(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

public record VariableTemplate : VariableBase
{
    [YamlMember(Alias = "template")]
    public string Name { get; }

    public VariableTemplate(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

public record Variable : VariableBase
{
    [YamlMember(Alias = "name", Order = 1)]
    public string Name { get; }

    [YamlMember(Alias = "value", Order = 2, DefaultValuesHandling = DefaultValuesHandling.Preserve)]
    public object Value { get; }

    [YamlMember(Alias = "readonly", Order = 3, DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public bool Readonly { get; init; }

    private Variable(string name, object value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Variable(string name, string value)
        : this(name, (object)value)
    {
    }

    public Variable(string name, int value)
        : this(name, (object)value)
    {
    }

    public Variable(string name, bool value)
        : this(name, (object)value)
    {
    }

    public Variable ReadOnly() => this with
    {
        Readonly = true
    };
}
