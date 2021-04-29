using System.IO;
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

        protected static ConditionedDefinition<VariableBase> Variable(string name, string value) => new(new Variable(name, value), null);
        protected static ConditionedDefinition<VariableBase> Variable(string name, int value) => new(new Variable(name, value), null);
        protected static ConditionedDefinition<VariableBase> Variable(string name, bool value) => new(new Variable(name, value), null);
        protected static ConditionedDefinition<VariableBase> Group(string name) => new(new VariableGroup(name), null);
        protected static ConditionBuilder<VariableBase> If => new();

        public sealed override string Publish()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();

            return serializer.Serialize(Pipeline);
        }
    }
}
