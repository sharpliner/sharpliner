using Sharpliner.Model.Definition;

namespace Sharpliner.Model.AzureDevOps
{
    public abstract class AzureDevOpsPipelineDefinition : PipelineDefinitionBase
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract AzureDevOpsPipeline Pipeline { get; }

        protected static ConditionedDefinition<VariableBase> Template(string path)
            => new(new Variable("template", path));
        protected static ConditionedDefinition<T> Template<T>(string path, TemplateParameters parameters)
            => new Template<T>(path, parameters);

        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name));
        protected static ConditionBuilder<VariableBase> If => new();
        protected static ConditionBuilder<T> If_<T>() => new();

        public sealed override string Publish() => SharplinerSerializer.Serialize(Pipeline);
    }
}
