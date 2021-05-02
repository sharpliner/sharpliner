using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps.Converters
{
    internal class VariableGroupConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => typeof(VariableGroup) == type;

        public object? ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            var variableGroup = (VariableGroup)value!;

            emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "group"));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variableGroup.Name));
            emitter.Emit(new MappingEnd());
        }
    }
}
