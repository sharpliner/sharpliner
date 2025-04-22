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
    public object Value { get; init; }

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
    /// Creates a new instance of <see cref="Variable"/> with a string value.
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
    /// Creates a new instance of <see cref="Variable"/> with an enum value.
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    public Variable(string name, Enum value)
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
    /// Converts this <see cref="Variable"/> to a <see cref="IfExpression"/> by getting the reference to the variable.
    /// </summary>
    public override string ToString() => new VariableReference(Name);

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

/// <summary>
/// Define an output variable created by a job, which can be referenced in other jobs in Azure DevOps pipelines. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/variables?view=azure-devops&amp;tabs=yaml%2Cbatch#use-outputs-in-a-different-job"/>
/// </summary>
public record DependencyVariable : VariableBase
{
    /// <summary>
    /// The name of the variable.
    /// </summary>
    public string VariableName { get; }

    /// <summary>
    /// The name of the step that created this variable.
    /// </summary>
    public string StepName { get; }

    /// <summary>
    /// The name of the job that created this variable.
    /// </summary>
    public string JobName { get; }

    /// <summary>
    /// Creates a new instance of <see cref="DependencyVariable"/> with a string value.
    /// </summary>
    /// <param name="variableName">The name of the variable</param>
    /// <param name="stepName">The name of the task that created this variable</param>
    /// <param name="jobName">The name of the job that created this variable</param>
    public DependencyVariable(string variableName, string stepName, string jobName)
    {
        VariableName = variableName;
        StepName = stepName;
        JobName = jobName;
    }

    /// <summary>
    /// Converts this <see cref="DependencyVariable"/> to a <see cref="IfExpression"/> by getting the reference to the variable.
    /// </summary>
    public override string ToString() => new DependencyVariableReference(VariableName, StepName, JobName);

    /// <summary>
    /// Converts a <see cref="DependencyVariable"/> to a <see cref="IfExpression"/> by getting the reference to the variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    public static implicit operator string(DependencyVariable variable) => new DependencyVariableReference(variable.VariableName, variable.StepName, variable.JobName);

    /// <summary>
    /// Converts a <see cref="DependencyVariable"/> to a <see cref="IfExpression"/> by getting the reference to the variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    public static implicit operator IfExpression(DependencyVariable variable) => new DependencyVariableReference(variable.VariableName, variable.StepName, variable.JobName);

    /// <summary>
    /// Converts a <see cref="DependencyVariable"/> to a <see cref="InlineExpression"/> by getting the reference to the variable.
    /// </summary>
    /// <param name="variable">The variable.</param>
    public static implicit operator InlineExpression(DependencyVariable variable) => new DependencyVariableReference(variable.VariableName, variable.StepName, variable.JobName);
}
