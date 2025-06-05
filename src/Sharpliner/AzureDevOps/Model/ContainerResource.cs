using System;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#container-resource">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record ContainerResource
{
    /// <summary>
    /// Identifier (A-Z, a-z, 0-9, and underscore)
    /// </summary>
    [YamlMember(Alias = "container")]
    public AdoExpression<string>? Identifier { get; }

    /// <summary>
    /// Container image name
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Image { get; init; }

    /// <summary>
    /// Arguments to pass to container at startup
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Options { get; init; }

    /// <summary>
    /// Reference to a service connection for the private registry
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Endpoint { get; init; }

    /// <summary>
    /// A map of environment variables that are available to all steps of the jobs. When more than one variable
    /// with the same name is used, the latter one will be used.
    /// </summary>
    public DictionaryExpression Env { get; init; } = [];

    /// <summary>
    /// Ports to expose on the container
    /// </summary>
    public ConditionedList<string> Ports { get; init; } = [];

    /// <summary>
    /// Volumes to mount on the container
    /// </summary>
    public ConditionedList<string> Volumes { get; init; } = [];

    /// <summary>
    /// Whether to map in the Docker daemon socket; defaults to true
    /// </summary>
    [DisallowNull]
    public AdoExpression<bool>? MapDockerSocket { get; init; }

    /// <summary>
    /// Volumes to mount read-only - all default to false
    /// </summary>
    [DisallowNull]
    public ContainerMountSettings? MountReadOnly { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerResource"/> class with the specified identifier.
    /// </summary>
    /// <param name="identifier">The alias or name of the container. Acceptable values: [-_A-Za-z0-9]*.</param>
    public ContainerResource(string identifier)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }
}

/// <summary>
/// Volumes to mount read-only, the default is all false.
/// </summary>
public record ContainerMountSettings
{
    /// <summary>
    /// Components required to talk to the agent
    /// </summary>
    public AdoExpression<bool>? Externals { get; init; }

    /// <summary>
    ///  tasks required by the job
    /// </summary>
    public AdoExpression<bool>? Tasks { get; init; }

    /// <summary>
    /// Installable tools like Python and Ruby
    /// </summary>
    public AdoExpression<bool>? Tools { get; init; }

    /// <summary>
    /// The work directory
    /// </summary>
    public AdoExpression<bool>? Work { get; init; }
}
