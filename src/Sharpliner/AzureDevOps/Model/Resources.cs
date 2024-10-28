using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Resources
{
    /// <summary>
    /// List of pipelines resources referenced by the pipeline.
    /// </summary>
    public ConditionedList<PipelineResource> Pipelines { get; init; } = [];

    /// <summary>
    /// List of build resources referenced by the pipeline.
    /// </summary>
    public ConditionedList<BuildResource> Builds { get; init; } = [];

    /// <summary>
    /// List of repository resources referenced by the pipeline.
    /// </summary>
    public ConditionedList<RepositoryResource> Repositories { get; init; } = [];

    /// <summary>
    /// List of container images referenced by the pipeline.
    /// </summary>
    public ConditionedList<ContainerResource> Containers { get; init; } = [];

    /// <summary>
    /// List of packages resources referenced by the pipeline.
    /// </summary>
    public ConditionedList<PackageResource> Packages { get; init; } = [];

    /// <summary>
    /// List of webhooks resources referenced by the pipeline.
    /// </summary>
    public ConditionedList<WebhookResource> Webhooks { get; init; } = [];
}
