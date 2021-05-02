using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.ConditionedDefinitions
{
    public class ConditionedDefinitionConverter<T> : IYamlTypeConverter
    {
        public IValueSerializer? ValueSerializer { get; set; }

        public bool Accepts(Type type) => typeof(ConditionedDefinition<T>).IsAssignableFrom(type);

        public object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var definition = (ConditionedDefinition<T>)value!;

            if (!string.IsNullOrEmpty(definition.Condition))
            {
                emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));
                emitter.Emit(new Scalar("${{ if " + definition.Condition + " }}"));
                emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Block));

                foreach (var childDefinition in definition.Definitions)
                {
                    WriteYaml(emitter, childDefinition, type);
                }

                emitter.Emit(new SequenceEnd());
                emitter.Emit(new MappingEnd());
            }
            else
            {
                ValueSerializer?.SerializeValue(emitter, definition.Definition, definition.Definition?.GetType());
            }
        }
    }
}
