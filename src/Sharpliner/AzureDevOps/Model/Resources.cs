using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Resources
{
    /// <summary>
    /// List of pipelines resources referenced by the pipeline.
    /// </summary>
    public AdoExpressionList<PipelineResource> Pipelines { get; init; } = [];

    /// <summary>
    /// List of build resources referenced by the pipeline.
    /// </summary>
    public AdoExpressionList<BuildResource> Builds { get; init; } = [];

    /// <summary>
    /// List of repository resources referenced by the pipeline.
    /// </summary>
    public AdoExpressionList<RepositoryResource> Repositories { get; init; } = [];

    /// <summary>
    /// List of container images referenced by the pipeline.
    /// </summary>
    public AdoExpressionList<ContainerResource> Containers { get; init; } = [];

    /// <summary>
    /// List of packages resources referenced by the pipeline.
    /// </summary>
    public AdoExpressionList<PackageResource> Packages { get; init; } = [];

    /// <summary>
    /// List of webhooks resources referenced by the pipeline.
    /// </summary>
    public AdoExpressionList<WebhookResource> Webhooks { get; init; } = [];
}
