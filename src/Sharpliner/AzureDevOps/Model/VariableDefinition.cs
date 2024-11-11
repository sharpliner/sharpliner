using System;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// A Base class for referencing variables in Azure DevOps pipelines. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/variables"/>
/// </summary>
public abstract record VariableBase;

/// <summary>
/// <para>
/// A group of variables that can be referenced in Azure DevOps pipelines. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/variables-group"/>
/// </para>
/// For example:
/// <code>
/// - group: my-variable-group
/// </code>
/// </summary>
public record VariableGroup : VariableBase
{
    /// <summary>
    /// The name of the variable group.
    /// </summary>
    [YamlMember(Alias = "group")]
    public Conditioned<string> Name { get; }

    /// <summary>
    /// Creates a new instance of <see cref="VariableGroup"/>.
    /// </summary>
    /// <param name="name">The name of the variable group</param>
    /// <exception cref="ArgumentNullException">If the input is null</exception>
    public VariableGroup(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// <para>
/// A template for a variable that can be referenced in Azure DevOps pipelines. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/variables-template"/>
/// </para>
/// For example:
/// <code>
/// - template: variables/build.yml
/// </code>
/// </summary>
public record VariableTemplate : VariableBase
{
    /// <summary>
    /// The path to the template file.
    /// </summary>
    [YamlMember(Alias = "template")]
    public Conditioned<string> Name { get; }

    /// <summary>
    /// Creates a new instance of <see cref="VariableTemplate"/>.
    /// </summary>
    /// <param name="name">The path to the template</param>
    /// <exception cref="ArgumentNullException">If the input is null</exception>
    public VariableTemplate(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}

/// <summary>
/// <para>
/// Define variables using name/value pairs that can be referenced in Azure DevOps pipelines. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/variables"/>
/// </para>
/// For example:
/// <code>
/// - name: myVariable
///   value: myValue
///   readonly: true
/// </code>
/// </summary>
public record Variable : VariableBase
{
    /// <summary>
    /// The name of the variable.
    /// </summary>
    [YamlMember(Alias = "name", Order = 1)]
    public string Name { get; }

    /// <summary>
    /// The value of the variable.
    /// </summary>
    [YamlMember(Alias = "value", Order = 2, DefaultValuesHandling = DefaultValuesHandling.Preserve)]
    public object Value { get; }

    /// <summary>
    /// Whether the variable is read-only. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/security/inputs#variables">Securely use variables</see>.
    /// </summary>
    [YamlMember(Alias = "readonly", Order = 3, DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    public Conditioned<bool>? Readonly { get; init; }

    private Variable(string name, object value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Creates a new instance of <see cref="Variable"/> with s string value.
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    public Variable(string name, string value)
        : this(name, (object)value)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Variable"/> with an integer value.
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    public Variable(string name, int value)
        : this(name, (object)value)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Variable"/> with a boolean value.
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    public Variable(string name, bool value)
        : this(name, (object)value)
    {
    }

    /// <summary>
    /// Returns a new instance of this variable as read-only.
    /// </summary>
    public Variable ReadOnly() => this with
    {
        Readonly = true
    };

    /// <summary>
    /// Converts a <see cref="Variable"/> to a <see cref="IfExpression"/> by getting the reference to the variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    public static implicit operator string(Variable variable) => new VariableReference(variable.Name);

    /// <summary>
    /// Converts a <see cref="Variable"/> to a <see cref="IfExpression"/> by getting the reference to the variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    public static implicit operator IfExpression(Variable variable) => new VariableReference(variable.Name);

    /// <summary>
    /// Converts a <see cref="Variable"/> to a <see cref="InlineExpression"/> by getting the reference to the variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    public static implicit operator InlineExpression(Variable variable) => new VariableReference(variable.Name);
}
