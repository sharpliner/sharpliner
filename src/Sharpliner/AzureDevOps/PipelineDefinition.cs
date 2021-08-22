using Sharpliner.Definition;

namespace Sharpliner.AzureDevOps
{

    public abstract class PipelineDefinitionBase<TPipeline> : AzureDevOpsDefinitions where TPipeline : PipelineBase
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract TPipeline Pipeline { get; }

        public override void Validate() => Pipeline.Validate();

        public override string Serialize() => PrettifyYaml(SharplinerSerializer.Serialize(Pipeline));
    }

    public abstract class PipelineDefinition : PipelineDefinitionBase<Pipeline>
    {
    }

    public abstract class SingleStagePipelineDefinition : PipelineDefinitionBase<SingleStagePipeline>
    {
    }
}
