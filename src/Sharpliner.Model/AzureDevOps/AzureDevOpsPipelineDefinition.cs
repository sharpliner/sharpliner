using System;
using System.IO;
using Sharpliner.Model.Definition;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
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
                .WithTypeConverter(new VariableConverter())
                .Build();

            return serializer.Serialize(Pipeline);
        }
    }

    public abstract class ConditionedDefinitionConverter<T> : IYamlTypeConverter
    {
        public bool Accepts(Type type) => typeof(ConditionedDefinition<T>).IsAssignableFrom(type);

        public object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var condition = (ConditionedDefinition<T>)value!;

            if (!string.IsNullOrEmpty(condition.Condition))
            {
                emitter.Emit(new Scalar("${{ if " + condition.Condition + " }}"));
                emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));
            }

            var definitions = condition.Definitions.GetEnumerator();
            var conditions = condition.Conditions.GetEnumerator();

            foreach (var isCondition in condition.Order)
            {
                if (isCondition)
                {
                    conditions.MoveNext();
                    // var nestedCondition = conditions.Current;

                    // foreach (var nestedDefinition in collection)


                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "nested if"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "value"));
                    //WriteYaml(emitter, , Con)
                }
                else
                {
                    definitions.MoveNext();

                    // TODO: Move to EmitDefinition
                    if (definitions.Current is Variable variable)
                    {
                        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variable.Name));
                        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variable.Value.ToString()!));
                    }
                    else if (definitions.Current is VariableGroup group)
                    {
                        emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, group.Name));
                    }
                    //EmitDefinition(emitter, definitions.Current);
                }

            }

            if (condition.Condition != null)
            {
                emitter.Emit(new MappingEnd());
            }
        }
    }

    public class VariableConverter : ConditionedDefinitionConverter<VariableBase>
    {

    }
}
