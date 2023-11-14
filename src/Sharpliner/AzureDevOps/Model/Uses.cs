using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Any resources (repos or pools) required by this job that are not already referenced.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#job">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Uses
{
    public ConditionedList<string> Repositories { get; init; } = [];

    public ConditionedList<string> Pools { get; init; } = [];
}
