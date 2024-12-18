using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Base class for all jobs in the pipeline.
/// </summary>
public abstract record JobBase : IDependsOn
{
    /// <summary>
    /// The name of the job.
    /// </summary>
    [YamlIgnore]
    public string Name { get; init; }

    /// <summary>
    /// Friendly name to display in the UI
    /// </summary>
    [YamlMember(Order = 100)]
    [DisallowNull]
    public Conditioned<string>? DisplayName { get; init; }

    /// <summary>
    /// List of names of other jobs this job depends on
    /// </summary>
    [YamlMember(Order = 200)]
    public DependsOn DependsOn { get; init; } = [];

    /// <summary>
    /// Specifies which pool to use for a job of the pipeline
    /// A pool specification also holds information about the job's strategy for running.
    /// </summary>
    [YamlMember(Order = 300)]
    public Conditioned<Pool>? Pool { get; init => field = value?.GetRoot(); }

    /// <summary>
    /// Container to run this job inside of.
    /// </summary>
    [YamlMember(Order = 500)]
    [DisallowNull]
    public Conditioned<ContainerReference>? Container { get; init => field = value?.GetRoot(); }

    /// <summary>
    /// Specifies variables at the job level
    /// You can add hard-coded values directly, reference variable groups, or insert via variable templates.
    /// </summary>
    [YamlMember(Order = 600)]
    public ConditionedList<VariableBase> Variables { get; init; } = [];

    /// <summary>
    /// How long to run the job before automatically cancelling
    /// </summary>
    [YamlIgnore]
    [DisallowNull]
    public Conditioned<TimeSpan>? Timeout { get; init; }

    /// <summary>
    /// Time to wait for this job to complete before the server kills it.
    /// </summary>
    [YamlMember(Order = 800)]
    public Conditioned<int>? TimeoutInMinutes => Timeout?.Definition != null ? (int)Timeout.Definition.TotalMinutes : null;

    /// <summary>
    /// How much time to give 'run always even if cancelled tasks' before killing them
    /// </summary>
    [YamlIgnore]
    [DisallowNull]
    public Conditioned<TimeSpan>? CancelTimeout { get; init; }

    /// <summary>
    /// Time to wait for the job to cancel before forcibly terminating it.
    /// </summary>
    [YamlMember(Order = 900)]
    public Conditioned<int>? CancelTimeoutInMinutes => CancelTimeout?.Definition != null ? (int)CancelTimeout.Definition.TotalMinutes : null;

    /// <summary>
    /// What to clean up before the job runs
    /// </summary>
    [YamlMember(Order = 1000)]
    public JobWorkspace? Workspace { get; init; } = null;

    /// <summary>
    /// Container resources to run as a service container.
    /// </summary>
    [YamlMember(Order = 1100)]
    public Dictionary<string, string> Services { get; init; } = [];

    /// <summary>
    /// Gets the condition expression to determine whether to run this job.
    /// </summary>
    [YamlMember(Order = 1100)]
    [DisallowNull]
    public Conditioned<InlineCondition>? Condition { get; init; }

    /// <summary>
    /// 'true' if future jobs should run even if this job fails
    /// defaults to 'false'
    /// </summary>
    [YamlMember(Order = 1200)]
    public Conditioned<bool>? ContinueOnError { get; init; }

    /// <summary>
    /// Instantiates a new instance of <see cref="JobBase"/> with the specified name and optional display name.
    /// </summary>
    /// <param name="name">The name of the job.</param>
    /// <param name="displayName">The friendly name to display in the UI.</param>
    /// <exception cref="ArgumentNullException">If the name is null.</exception>
    protected JobBase(string name, string? displayName = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        if (displayName != null)
        {
            DisplayName = displayName;
        }
    }
}
