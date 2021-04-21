using Sharpliner.Model.Definition;

namespace Sharpliner.Model.AzureDevOps
{
    public abstract class AzureDevOpsPipelineDefinition : PipelineDefinitionBase
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract AzureDevOpsPipeline Pipeline { get; }

        protected static ConditionedDefinition<Variable> Variable(string name, string value) => new(new SingleVariable(name, value), null);
        protected static ConditionedDefinition<Variable> Variable(string name, int value) => new(new SingleVariable(name, value), null);
        protected static ConditionedDefinition<Variable> Variable(string name, bool value) => new(new SingleVariable(name, value), null);
        protected static ConditionedDefinition<Variable> Group(string name) => new(new VariableGroup(name), null);
        protected static ConditionBuilder<Variable> If => new();
    }
}
