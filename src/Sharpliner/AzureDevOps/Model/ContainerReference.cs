using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Container to run the job inside of
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#container-reference">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record ContainerReference
{
    /// <summary>
    /// Container image name
    /// </summary>
    [DisallowNull]
    public Conditioned<string>? Image { get; init; }

    /// <summary>
    /// Arguments to pass to container at startup
    /// </summary>
    [DisallowNull]
    public Conditioned<string>? Options { get; init; }

    /// <summary>
    /// Endpoint for a private container registry
    /// </summary>
    [DisallowNull]
    public string? Endpoint { get; init; }

    /// <summary>
    /// A map of environment variables that are available to all steps of the jobs.
    /// When more than one variable with the same name is used, the latter one will be used.
    /// </summary>
    public ConditionedDictionary Env { get; init; } = new();
}
