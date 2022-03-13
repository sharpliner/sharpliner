using System.Collections.Generic;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Common internal ancestor. Do not inherit from this class, use PipelineDefinition or SingleStagePipelineDefinition.
/// </summary>
/// <typeparam name="TPipeline">Type of the pipeline (single stage, full..)</typeparam>
public abstract class PipelineDefinitionBase<TPipeline>
    : AzureDevOpsDefinition, ISharplinerDefinition where TPipeline : PipelineBase
{
    /// <summary>
    /// Path to the YAML file where this pipeline will be exported to.
    /// When you build the project, the pipeline will be saved into a file on this path.
    /// Example: "pipelines/ci.yaml"
    /// </summary>
    public abstract string TargetFile { get; }

    public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// 
    /// Leave empty array to omit file header.
    /// </summary>
    public virtual string[]? Header => ISharplinerDefinition.GetDefaultHeader(GetType());

    /// <summary>
    /// Define the pipeline by implementing this field.
    /// </summary>
    public abstract TPipeline Pipeline { get; }

    /// <summary>
    /// Returns the list of validations that should be run on the definition (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    public IReadOnlyCollection<IDefinitionValidation> Validations => Pipeline.Validations;

    /// <summary>
    /// Serializes this pipeline into YAML.
    /// </summary>
    public string Serialize() => SharplinerSerializer.Serialize(Pipeline);

    /// <summary>
    /// Internal, cannot inherit from this.
    /// Please override Sharpliner.AzureDevOps.PipelineDefinition or Sharpliner.AzureDevOps.SingleStagePipelineDefinition.
    /// </summary>
    internal PipelineDefinitionBase()
    {
    }
}
