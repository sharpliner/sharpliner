using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Use this data object to generate a list of definitions dynamically.
/// </summary>
/// <typeparam name="T">Type of the template (one of stages, jobs, steps or variables)</typeparam>
/// <param name="TargetFile">Path to the YAML file/folder where this definition/collection will be exported to</param>
/// <param name="Definition">Definition of the template</param>
/// <param name="Parameters">List of template parameters</param>
/// <param name="PathType">Override this to define where the resulting YAML should be stored (together with TargetFile)</param>
/// <param name="Header">Header that will be shown at the top of the generated YAML file. Leave empty array for no header, leave null for a default</param>
public record TemplateDefinitionData<T>(
    string TargetFile,
    ConditionedList<T> Definition,
    List<TemplateParameter>? Parameters = null,
    TargetPathType PathType = TargetPathType.RelativeToGitRoot,
    string[]? Header = null);

public abstract class TemplateDefinitionCollection<T> : TemplateDefinition, ISharplinerDefinitionCollection
{
    public abstract IEnumerable<TemplateDefinitionData<T>> Templates { get; }

    internal abstract string YamlProperty { get; }

    public IEnumerable<ISharplinerDefinition> Definitions
        => Templates.Select(data => new TemplateDefinitionWrapper<T>(data, YamlProperty, GetType()));

    // Only us inheriting from this
    internal TemplateDefinitionCollection()
    {
    }
}

internal class TemplateDefinitionWrapper<T> : TemplateDefinition<T>
{
    private readonly string[]? _header;

    public TemplateDefinitionWrapper(TemplateDefinitionData<T> data, string yamlMemberName, Type definitionType)
    {
        TargetFile = data.TargetFile;
        Definition = data.Definition;
        TargetPathType = data.PathType;
        Parameters = data.Parameters ?? new List<TemplateParameter>();
        _header = data.Header ?? ISharplinerDefinition.GetDefaultHeader(definitionType);
        YamlProperty = yamlMemberName;
    }

    public override string TargetFile { get; }

    public override TargetPathType TargetPathType { get; }

    public override ConditionedList<T> Definition { get; }

    public override List<TemplateParameter> Parameters { get; }

    public override string[]? Header => _header;

    internal override string YamlProperty { get; }
}
