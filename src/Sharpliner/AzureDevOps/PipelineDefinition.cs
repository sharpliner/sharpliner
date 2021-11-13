namespace Sharpliner.AzureDevOps;

/// <summary>
/// Common internal ancestor. Do not inherit from this class, use PipelineDefinition or SingleStagePipelineDefinition.
/// </summary>
/// <typeparam name="TPipeline">Type of the pipeline (single stage, full..)</typeparam>
public abstract class PipelineDefinitionBase<TPipeline> : AzureDevOpsDefinition where TPipeline : PipelineBase
{
    /// <summary>
    /// Define the pipeline by implementing this field.
    /// </summary>
    public abstract TPipeline Pipeline { get; }

    /// <summary>
    /// This method is run by the Sharpliner publish process to validate for
    /// some problems we can catch early and fail the publishing.
    /// </summary>
    public override void Validate() => Pipeline.Validate();

    /// <summary>
    /// Serializes this pipeline into YAML.
    /// </summary>
    public override string Serialize() => SharplinerSerializer.Serialize(Pipeline);

    /// <summary>
    /// Internal, cannot inherit from this.
    /// Please override Sharpliner.AzureDevOps.PipelineDefinition or Sharpliner.AzureDevOps.SingleStagePipelineDefinition.
    /// </summary>
    internal PipelineDefinitionBase()
    {
    }
}

/// <summary>
/// Inherit from this class to define a full Azure DevOps pipeline.
/// For a pipeline with only a single stage, consider using SingleStagePipelineDefinition.
/// </summary>
public abstract class PipelineDefinition : PipelineDefinitionBase<Pipeline>
{
}

/// <summary>
/// Inherit from this class to define a pipeline with a single stage where you only define jobs.
/// </summary>
public abstract class SingleStagePipelineDefinition : PipelineDefinitionBase<SingleStagePipeline>
{
}
