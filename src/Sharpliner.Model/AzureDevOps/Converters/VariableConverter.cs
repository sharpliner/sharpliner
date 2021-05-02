using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps.Converters
{
    internal class VariableConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => typeof(Variable) == type;

        public object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var variable = (Variable)value!;

            emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "name"));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variable.Name));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "value"));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variable.Value.ToString()!));
            emitter.Emit(new MappingEnd());
        }
    }
}
