using Sharpliner.Model.Definition;

namespace Sharpliner.Model.AzureDevOps
{
    public abstract class AzureDevOpsPipelineDefinition : PipelineDefinitionBase
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract AzureDevOpsPipeline Pipeline { get; }

        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new SingleVariable(name, value), null);
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new SingleVariable(name, value), null);
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new SingleVariable(name, value), null);
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name), null);
        protected static ConditionBuilder<VariableBase> If => new();
    }
}
