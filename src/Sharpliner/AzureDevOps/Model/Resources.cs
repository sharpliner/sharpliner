using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Resources
{
    public ConditionedList<PipelineResource> Pipelines { get; init; } = [];

    public ConditionedList<BuildResource> Builds { get; init; } = [];

    public ConditionedList<RepositoryResource> Repositories { get; init; } = [];

    public ConditionedList<ContainerResource> Containers { get; init; } = [];

    public ConditionedList<PackageResource> Packages { get; init; } = [];

    public ConditionedList<WebhookResource> Webhooks { get; init; } = [];
}
