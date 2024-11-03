using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Use this data object to generate a list of definitions dynamically.
/// </summary>
/// <typeparam name="T">Type of the pipeline (full, single stage..)</typeparam>
/// <param name="TargetFile">Path to the YAML file/folder where this definition/collection will be exported to</param>
/// <param name="Pipeline">Definition of the template</param>
/// <param name="PathType">Override this to define where the resulting YAML should be stored (together with TargetFile)</param>
/// <param name="Header">Header that will be shown at the top of the generated YAML file. Leave empty array for no header, leave null for a default</param>
public record PipelineDefinitionData<T>(
    string TargetFile,
    T Pipeline,
    TargetPathType PathType = TargetPathType.RelativeToGitRoot,
    string[]? Header = null);

/// <summary>
/// <para>
/// Base class for defining a collection of pipelines,
/// e.g. <see cref="PipelineCollection"/>, <see cref="SingleStagePipelineCollection"/>.
/// </para>
/// Do not extend this class directly, use one of the more concrete classes instead.
/// </summary>
/// <typeparam name="TPipeline">The concrete pipeline type.</typeparam>
public abstract class PipelineDefinitionCollection<TPipeline>
    : AzureDevOpsDefinition, ISharplinerDefinitionCollection where TPipeline : PipelineBase
{
    /// <inheritdoc/>
    public IEnumerable<ISharplinerDefinition> Definitions => Pipelines.Select(data => new PipelineDefinitionWrapper<TPipeline>(data, GetType()));

    /// <summary>
    /// Override this with your dynamically generated list of definitions.
    /// </summary>
    public abstract IEnumerable<PipelineDefinitionData<TPipeline>> Pipelines { get; }

    // Only us inheriting from this
    internal PipelineDefinitionCollection()
    {
    }
}

internal class PipelineDefinitionWrapper<T>(PipelineDefinitionData<T> data, Type definitionType) : ISharplinerDefinition where T : PipelineBase
{
    public string TargetFile { get; } = data.TargetFile;

    public TargetPathType TargetPathType { get; } = data.PathType;

    public T Pipeline { get; } = data.Pipeline;

    public string[]? Header { get; private set; } = data.Header ?? SharplinerPublisher.GetDefaultHeader(definitionType);

    public IReadOnlyCollection<IDefinitionValidation> Validations => Pipeline.Validations;

    public string Serialize() => SharplinerSerializer.Serialize(Pipeline);
}
