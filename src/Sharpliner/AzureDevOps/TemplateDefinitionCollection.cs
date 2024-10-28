using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Use this data object to generate a list of definitions dynamically.
/// </summary>
/// <typeparam name="T">Type of the template (one of stages, jobs, steps or variables)</typeparam>
/// <param name="TargetFile">Path to the YAML file/folder where this definition/collection will be exported to</param>
/// <param name="Definition">Definition of the template</param>
/// <param name="Parameters">List of template parameters</param>
/// <param name="PathType">Override this to define where the resulting YAML should be stored (together with <paramref name="TargetFile"/>)</param>
/// <param name="Header">Header that will be shown at the top of the generated YAML file. Leave empty array for no header, leave null for a default</param>
public record TemplateDefinitionData<T>(
    string TargetFile,
    ConditionedList<T> Definition,
    List<Parameter>? Parameters = null,
    TargetPathType PathType = TargetPathType.RelativeToGitRoot,
    string[]? Header = null);

public abstract class TemplateDefinitionCollection<T> : TemplateDefinition, ISharplinerDefinitionCollection
{
    /// <summary>
    /// Returns a sequence of templates that will be used to populate <see cref="Definitions"/>.
    /// </summary>
    public abstract IEnumerable<TemplateDefinitionData<T>> Templates { get; }

    internal abstract string YamlProperty { get; }

    internal abstract IReadOnlyCollection<IDefinitionValidation> GetValidations(TemplateDefinitionData<T> definition);

    /// <inheritdoc/>
    public IEnumerable<ISharplinerDefinition> Definitions
        => Templates.Select(data => new TemplateDefinitionWrapper<T>(data, YamlProperty, GetType(), GetValidations(data)));

    // Only us inheriting from this
    internal TemplateDefinitionCollection()
    {
    }
}

internal class TemplateDefinitionWrapper<T>(
    TemplateDefinitionData<T> data,
    string yamlMemberName,
    Type definitionType,
    IReadOnlyCollection<IDefinitionValidation> validations) : TemplateDefinition<T>
{
    private readonly string[]? _header = data.Header ?? SharplinerPublisher.GetDefaultHeader(definitionType);

    public override string TargetFile { get; } = data.TargetFile;

    public override TargetPathType TargetPathType { get; } = data.PathType;

    public override ConditionedList<T> Definition { get; } = data.Definition;

    public override List<Parameter> Parameters { get; } = data.Parameters ?? [];

    public override string[]? Header => _header;

    internal override string YamlProperty { get; } = yamlMemberName;

    public sealed override IReadOnlyCollection<IDefinitionValidation> Validations { get; } = validations;
}
