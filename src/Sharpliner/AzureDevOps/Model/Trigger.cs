using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#triggers">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record Trigger
{
    /// <summary>
    /// Use this to explicitly set you want 'none' trigger.
    /// </summary>
    public static NoneTrigger None { get; } = new NoneTrigger();

    /// <summary>
    /// Batch changes if true; start a new build for every push if false (default).
    /// </summary>
    public bool Batch { get; init; } = false;

    [DisallowNull]
    public InclusionRule? Branches { get; init; }

    [DisallowNull]
    public InclusionRule? Tags { get; init; }

    [DisallowNull]
    public InclusionRule? Paths { get; init; }

    public Trigger()
    {
    }

    public Trigger(params string[] branches)
    {
        Branches = new InclusionRule
        {
            Include = branches.ToList()
        };
    }
}

public record NoneTrigger : Trigger, IYamlConvertible
{
    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => emitter.Emit(new Scalar("none"));
}
