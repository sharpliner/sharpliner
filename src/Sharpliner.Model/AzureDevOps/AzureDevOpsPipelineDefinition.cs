using Sharpliner.Model.Definition;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner.Model.AzureDevOps
{
    public abstract class AzureDevOpsPipelineDefinition : PipelineDefinitionBase
    {
        /// <summary>
        /// Define the pipeline by implementing this field.
        /// </summary>
        public abstract AzureDevOpsPipeline Pipeline { get; }

        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new Variable(name, value));
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name));
        protected static ConditionBuilder<VariableBase> If => new();

        public sealed override string Publish()
        {
            // TODO: Build just once somewhere else
            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull);

            return serializerBuilder.Build().Serialize(Pipeline);
        }
    }
}
