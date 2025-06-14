using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Any resources (repos or pools) required by this job that are not already referenced.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#job">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Uses
{
    /// <summary>
    /// Repositories required by this job that are not already referenced.
    /// </summary>
    public AdoExpressionList<string> Repositories { get; init; } = [];

    /// <summary>
    /// Pools required by this job that are not already referenced.
    /// </summary>
    public AdoExpressionList<string> Pools { get; init; } = [];
}
