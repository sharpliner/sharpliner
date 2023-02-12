using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public string Identifier { get; }

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
    /// Reference to a service connection for the private registry
    /// </summary>
    [DisallowNull]
    public Conditioned<string>? Endpoint { get; init; }

    /// <summary>
    /// A map of environment variables that are available to all steps of the jobs. When more than one variable
    /// with the same name is used, the latter one will be used.
    /// </summary>
    public ConditionedDictionary Env { get; init; } = new();

    /// <summary>
    /// Ports to expose on the container
    /// </summary>
    public List<string> Ports { get; init; } = new();

    /// <summary>
    /// Volumes to mount on the container
    /// </summary>
    public List<string> Volumes { get; init; } = new();

    /// <summary>
    /// Whether to map in the Docker daemon socket; defaults to true
    /// </summary>
    [DisallowNull]
    [DefaultValue(true)]
    public bool MapDockerSocket { get; init; } = true;

    /// <summary>
    /// Volumes to mount read-only - all default to false
    /// </summary>
    [DisallowNull]
    public ContainerMountSettings? MountReadOnly { get; init; }

    public ContainerResource(string identifier)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        Pipeline.ValidateName(identifier);
    }
}

public record ContainerMountSettings
{
    /// <summary>
    /// Components required to talk to the agent
    /// </summary>
    public bool Externals { get; init; }

    /// <summary>
    ///  tasks required by the job
    /// </summary>
    public bool Tasks { get; init; }

    /// <summary>
    /// Installable tools like Python and Ruby
    /// </summary>
    public bool Tools { get; init; }

    /// <summary>
    /// The work directory
    /// </summary>
    public bool Work { get; init; }
}
