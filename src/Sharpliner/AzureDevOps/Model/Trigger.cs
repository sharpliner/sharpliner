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

    /// <summary>
    /// Branches to include or exclude.
    /// </summary>
    [DisallowNull]
    public InclusionRule? Branches { get; init; }

    /// <summary>
    /// Tags to include or exclude.
    /// </summary>
    [DisallowNull]
    public InclusionRule? Tags { get; init; }

    /// <summary>
    /// Paths to include or exclude.
    /// </summary>
    [DisallowNull]
    public InclusionRule? Paths { get; init; }

    /// <summary>
    /// Created a new instance of the <see cref="Trigger"/> class.
    /// </summary>
    public Trigger()
    {
    }

    /// <summary>
    /// Created a new instance of the <see cref="Trigger"/> class with an explicit list of branches. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/trigger#trigger-string-list">trigger: string list</see>.
    /// </summary>
    /// <param name="branches">The branches to include.</param>
    public Trigger(params string[] branches)
    {
        Branches = new InclusionRule
        {
            Include = branches.ToList()
        };
    }
}

/// <summary>
/// Represents a trigger that disable CI triggers. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/trigger#trigger-none">trigger: none</see>.
/// </summary>
public record NoneTrigger : Trigger, IYamlConvertible
{
    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => emitter.Emit(new Scalar("none"));
}
