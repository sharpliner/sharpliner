using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Use to define a lifecycle hook for a <see cref="DeploymentJob"/>.
/// </summary>
public record LifeCycleHook
{
    /// <summary>
    /// Specifies which pool to use for a job of the pipeline
    /// A pool specification also holds information about the job's strategy for running.
    /// </summary>
    [YamlMember(Order = 300)]
    public AdoExpression<Pool>? Pool { get; set => field = value?.GetRoot(); }

    /// <summary>
    /// A step is a linear sequence of operations that make up a job
    /// Each step runs in its own process on an agent and has access to the pipeline workspace on a local hard drive.
    /// This behavior means environment variables aren't preserved between steps but file system changes are.
    /// </summary>
    [YamlMember(Order = 700)]
    public ConditionedList<Step> Steps { get; init; } = [];
}
