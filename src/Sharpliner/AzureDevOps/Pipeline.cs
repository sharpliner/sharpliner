using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Validation;
using Sharpliner.Common;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Base model for Azure DevOps pipelines.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public abstract record PipelineBase
{
    /// <summary>
    /// Name of the pipeline in the build numbering format
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/run-number?view=azure-devops&amp;tabs=yaml">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 100)]
    public string? Name { get; init; }

    /// <summary>
    /// Append the commit message to the build number. The default is true.
    /// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/pipeline?view=azure-pipelines#properties">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 110)]
    public bool? AppendCommitMessageToRunName { get; init; }

    /// <summary>
    /// LockBehavior of the pipeline
    /// </summary>
    [YamlMember(Order = 120)]
    public Conditioned<LockBehavior>? LockBehavior { get; init; }

    /// <summary>
    /// Specifies pipeline parameters
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/runtime-parameters?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 150)]
    public ConditionedList<Parameter> Parameters { get; init; } = [];

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
    public List<ScheduledTrigger> Schedules { get; init; } = [];

    /// <summary>
    /// A resource is any external service that is consumed as part of your pipeline
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 400)]
    [DisallowNull]
    public Conditioned<Resources>? Resources { get; init => field = value?.GetRoot(); }

    /// <summary>
    /// Specifies variables at the pipeline level
    /// You can add hard-coded values directly, reference variable groups, or insert via variable templates.
    /// </summary>
    [YamlMember(Order = 500)]
    public ConditionedList<VariableBase> Variables { get; init; } = [];

    /// <summary>
    /// Specifies which pool to use for a job of the pipeline
    /// A pool specification also holds information about the job's strategy for running.
    /// </summary>
    [YamlMember(Order = 550)]
    public Conditioned<Pool>? Pool { get; init => field = value?.GetRoot(); }

    /// <summary>
    /// Returns the list of validations that should be run on the definition (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    [YamlIgnore]
    public abstract IReadOnlyCollection<IDefinitionValidation> Validations { get; }
}

/// <summary>
/// Model for a full Azure DevOps pipeline.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public record Pipeline : PipelineBase
{
    /// <summary>
    /// Specifies the stages of the pipeline.
    /// </summary>
    [YamlMember(Order = 600)]
    public ConditionedList<Stage> Stages { get; init; } = [];

    /// <inheritdoc/>
    public override IReadOnlyCollection<IDefinitionValidation> Validations =>
    [
        .. Stages.GetStageValidations(Parameters),
        new RepositoryCheckoutValidation(this)
    ];
}

/// <summary>
/// Model for an Azure DevOps pipeline that extends a template.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public record PipelineWithExtends : PipelineBase
{
    /// <summary>
    /// Specifies the template that this pipeline extends.
    /// </summary>
    [YamlMember(Order = 600)]
    [DisallowNull]
    public required Extends Extends { get; init; }

    /// <inheritdoc/>
    public override IReadOnlyCollection<IDefinitionValidation> Validations => [];
}

/// <summary>
/// Model for a single-stage AzureDevOps pipeline.
/// This is a model only! To define a pipeline, inherit from one of the *PipelineDefinition classes.
/// </summary>
public record SingleStagePipeline : PipelineBase
{
    /// <summary>
    /// Specifies the jobs of the pipeline.
    /// </summary>
    [YamlMember(Order = 600)]
    public ConditionedList<JobBase> Jobs { get; init; } = [];

    /// <inheritdoc/>
    public override IReadOnlyCollection<IDefinitionValidation> Validations=>
    [
        .. Jobs.GetJobValidations(Parameters),
        new RepositoryCheckoutValidation(this)
    ];
}
