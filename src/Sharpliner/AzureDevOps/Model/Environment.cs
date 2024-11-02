﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// The environment or resource that is targeted by a deployment job of the pipeline.
/// An environment also holds information about the deployment strategy for running the steps defined inside the job.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#environment">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Environment
{
    /// <summary>
    /// Name of the environment to run this job on
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Name of the resource in the environment to record the deployments against
    /// </summary>
    public string? ResourceName { get; init; }

    /// <summary>
    /// Resource identifier
    /// </summary>
    [DisallowNull]
    public int? ResourceId { get; init; }

    /// <summary>
    /// Type of the resource you want to target
    /// </summary>
    public ResourceType? ResourceType { get; init; }

    /// <summary>
    /// Tag names to filter the resources in the environment
    /// </summary>
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Instantiates a new <see cref="Environment"/> with the specified name and optional resource name.
    /// </summary>
    /// <param name="name">Name of the environment.</param>
    /// <param name="resourceName">Name of the resource.</param>
    public Environment(string name, string? resourceName = null)
    {
        Name = name;
        ResourceName = resourceName;
    }
}

/// <summary>
/// Type of the resource to target.
/// </summary>
public enum ResourceType
{
    /// <summary>
    /// A Kubernetes resource. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/environments-kubernetes?view=azure-devops">Kubernetes resource</see>.
    /// </summary>
    [YamlMember(Alias = "virtualMachine")]
    VirtualMachine,

    /// <summary>
    /// A virtual machine resource. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/environments-virtual-machines?view=azure-devops">Virtual machine resource</see>.
    /// </summary>
    [YamlMember(Alias = "Kubernetes")]
    Kubernetes,
}
