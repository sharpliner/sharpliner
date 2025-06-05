using System;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// A step is a linear sequence of operations that make up a job.
/// Each step runs in its own process on an agent and has access to the pipeline workspace on a local hard drive.
/// This behavior means environment variables aren't preserved between steps but file system changes are.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#steps">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record Step
{
    /// <summary>
    /// Friendly name displayed in the UI.
    /// </summary>
    [YamlMember(Order = 100)]
    [DisallowNull]
    public AdoExpression<string>? DisplayName { get; init; }

    /// <summary>
    /// Identifier for this step (A-Z, a-z, 0-9, and underscore).
    /// </summary>
    [YamlMember(Order = 150)]
    [DisallowNull]
    public AdoExpression<string>? Name { get; init; }

    /// <summary>
    /// Whether to run this step; defaults to 'true'.
    /// </summary>
    [YamlMember(Order = 175)]
    public AdoExpression<bool>? Enabled { get; init; }

    /// <summary>
    /// Condition that must be met to run this step.
    /// </summary>
    [YamlMember(Order = 190)]
    [DisallowNull]
    public AdoExpression<InlineCondition>? Condition { get; init; }

    /// <summary>
    /// Whether future steps should run even if this step fails.
    /// Defaults to 'false'.
    /// </summary>
    [YamlMember(Order = 200)]
    public AdoExpression<bool>? ContinueOnError { get; init; }

    /// <summary>
    /// Timeout after which the step will be stopped.
    /// </summary>
    [YamlIgnore]
    [DisallowNull]
    public AdoExpression<TimeSpan>? Timeout
    {
        get;
        init
        {
            field = value;
            if (value?.Definition is not null)
            {
                TimeoutInMinutes = (int)value.Definition.TotalMinutes;
            }
            else if (value?.Condition is not null)
            {
                TimeoutInMinutes = new AdoExpression<int>(default, value.Condition);
            }
        }
    }

    /// <summary>
    /// Timeout after which the step will be stopped.
    /// </summary>
    [YamlMember(Order = 210)]
    public AdoExpression<int>? TimeoutInMinutes { get; init; }

    /// <summary>
    /// A list of additional items to map into the process's environment.
    /// For example, secret variables are not automatically mapped.
    /// </summary>
    [YamlMember(Order = 220)]
    [DisallowNull]
    public DictionaryExpression Env { get; init; } = [];

    /// <summary>
    /// Make step only run when a condition is met.
    /// </summary>
    public Step When(string condition) => this with
    {
        Condition = new InlineCustomCondition(condition)
    };

    /// <summary>
    /// Make step only run when previous steps succeeded ('succeeded()').
    /// </summary>
    public Step WhenSucceeded() => When("succeeded()");

    /// <summary>
    /// Make step run even when previous steps failed ('succeededOrFailed()').
    /// </summary>
    public Step WhenSucceededOrFailed() => When("succeededOrFailed()");

    /// <summary>
    /// Sets the displayName property.
    /// </summary>
    public Step DisplayAs(string displayName) => this with { DisplayName = displayName };

    /// <summary>
    /// Sets the name property.
    /// </summary>
    public Step WithName(string name) => this with { Name = name };
}
