﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Validation;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Base model for Azure DevOps pipelines.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public abstract record PipelineBase
{
    private Conditioned<Resources>? _resources;

    /// <summary>
    /// Name of the pipline in the build numbering format
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/run-number?view=azure-devops&amp;tabs=yaml">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 100)]
    [DisallowNull]
    public string? Name { get; init; }

    /// <summary>
    /// Specifies when the pipeline is supposed to run
    /// </summary>
    [YamlMember(Order = 200)]
    [DisallowNull]
    public Trigger? Trigger { get; init; }

    /// <summary>
    /// A pull request trigger specifies which branches cause a pull request build to run.
    /// If you specify no pull request trigger, pull requests to any branch trigger a build.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/build/triggers?tabs=yaml&amp;view=azure-devops#pr-triggers">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 300)]
    [DisallowNull]
    public PrTrigger? Pr { get; init; }

    /// <summary>
    /// Scheduled triggers for the pipeline which specify which branches are built when periodically.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/build/triggers?tabs=yaml&amp;view=azure-devops#scheduled-trigger">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 350)]
    [DisallowNull]
    public List<ScheduledTrigger> Schedules { get; init; } = new();

    /// <summary>
    /// A resource is any external service that is consumed as part of your pipeline
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 400)]
    [DisallowNull]
    public Conditioned<Resources>? Resources { get => _resources; init => _resources = value?.GetRoot(); }

    /// <summary>
    /// Specifies variables at the pipeline level
    /// You can add hard-coded values directly, reference variable groups, or insert via variable templates.
    /// </summary>
    [YamlMember(Order = 500)]
    public ConditionedList<VariableBase> Variables { get; init; } = new();

    internal abstract IReadOnlyCollection<IDefinitionValidation> Validations { get; }
}

/// <summary>
/// Model for a full Azure DevOps pipeline.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public record Pipeline : PipelineBase
{
    [YamlMember(Order = 600)]
    public ConditionedList<Stage> Stages { get; init; } = new();

    internal override IReadOnlyCollection<IDefinitionValidation> Validations => new List<IDefinitionValidation>
    {
        new StageDependsOnValidation(Stages),
        new JobsDependsOnValidation(Stages),
    };

    internal static void ValidateName(string name)
    {
        if (!DefinitionValidator.NameRegex.IsMatch(name))
        {
            throw new FormatException($"Invalid identifier '{name}'! Only A-Z, a-z, 0-9, and underscore are allowed.");
        }
    }
}

/// <summary>
/// Model for a single-stage AzureDevOps pipeline.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public record SingleStagePipeline : PipelineBase
{
    [YamlMember(Order = 600)]
    public ConditionedList<JobBase> Jobs { get; init; } = new();

    internal override IReadOnlyCollection<IDefinitionValidation> Validations => new List<IDefinitionValidation>
    {
        new JobsDependsOnValidation(Jobs),
    };
}
