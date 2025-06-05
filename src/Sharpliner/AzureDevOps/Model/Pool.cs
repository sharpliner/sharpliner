using System;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#pool">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Pool
{
    /// <summary>
    /// Name of this pool. (A-Z, a-z, 0-9, and underscore).
    /// </summary>
    [YamlMember(Order = 100)]
    public AdoExpression<string>? Name { get; init; }

    /// <summary>
    /// Creates a new instance of <see cref="Pool"/>.
    /// </summary>
    /// <param name="name">The name of the pool.</param>
    public Pool(string? name)
    {
        if (name is not null)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="Pool"/> instance.
    /// </summary>
    /// <param name="vmImage">The vmImage.</param>
    public static implicit operator Pool(string vmImage) => new HostedPool(vmImage: vmImage);
}

/// <summary>
///
/// </summary>
public record HostedPool : Pool
{
    /// <summary>
    /// Creates a new instance of <see cref="HostedPool"/>.
    /// </summary>
    /// <param name="name">The name of the pool.</param>
    /// <param name="vmImage">The VM image to use.</param>
    public HostedPool(string? name = null, string? vmImage = null) : base(name)
    {
        if (vmImage is not null)
        {
            VmImage = vmImage;
        }
    }

    /// <summary>
    /// The VM image to use.
    /// </summary>
    [YamlMember(Order = 105)]
    public AdoExpression<string>? VmImage { get; init; }

    /// <summary>
    /// The demands required by the private pool.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use demands to make sure that the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/agents/agents#capabilities">capabilities</see> your pipeline needs are present on the agents that run it. Demands are asserted automatically by tasks or manually by you.
    /// </para>
    /// <para>
    /// You can check for the presence of a capability (<see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/pool-demands?view=azure-pipelines#exists-operation">Exists operation</see>) or you can check for a specific string in a capability (<see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/pool-demands?view=azure-pipelines#equals-operation">Equals operation</see>). Checking for the existence of a capability (exists) and checking for a specific string in a capability (equals) are the only two supported operations for demands.
    /// </para>
    /// </remarks>
    [YamlMember(Order = 110)]
    [DisallowNull]
    public AdoExpressionList<string> Demands { get; init; } = [];
}

/// <summary>
/// The server pool specifies a server job.
/// Only server tasks like invoking an Azure function app can be run in a server job.
/// </summary>
public record ServerPool : Pool, IYamlConvertible
{
    /// <summary>
    /// Creates a new instance of <see cref="ServerPool"/>.
    /// </summary>
    public ServerPool() : base(null)
    {
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => emitter.Emit(new Scalar("server"));
}
