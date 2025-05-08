using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// A pull request trigger specifies which branches cause a pull request build to run.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#pr-trigger">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record PrTrigger
{
    /// <summary>
    /// Use this to explicitly set you want 'none' trigger.
    /// </summary>
    public static NonePrTrigger None { get; } = new NonePrTrigger();

    /// <summary>
    /// Indicates whether additional pushes to a PR should cancel in-progress runs for the same PR
    /// Defaults to true
    /// </summary>
    [DefaultValue(true)]
    public Conditioned<bool>? AutoCancel { get; init; } = true;

    /// <summary>
    /// For GitHub only, whether to build draft PRs
    /// Defaults to true
    /// </summary>
    [DefaultValue(true)]
    public Conditioned<bool>? Drafts { get; init; } = true;

    /// <summary>
    /// Branches to include or exclude for triggering a run.
    /// </summary>
    [DisallowNull]
    public InclusionRule? Branches { get; init; }

    /// <summary>
    /// Paths to include or exclude for triggering a run.
    /// </summary>
    [DisallowNull]
    public InclusionRule? Paths { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrTrigger"/> class.
    /// </summary>
    public PrTrigger()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrTrigger"/> class with the specified branches.
    /// </summary>
    /// <param name="branches">The branches to trigger the pipeline on.</param>
    public PrTrigger(params string[] branches)
    {
        Branches = new InclusionRule
        {
            Include = branches.ToList()
        };
    }
}

/// <summary>
/// Use this to explicitly set you want <c>none</c> trigger.
/// </summary>
public record NonePrTrigger : PrTrigger, IYamlConvertible
{
    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => emitter.Emit(new Scalar("none"));
}
