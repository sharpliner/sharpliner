using System;
using Sharpliner.Model.ConditionedDefinitions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Sharpliner.Model.AzureDevOps.Converters
{
    internal class VariableConverter : ConditionedDefinitionConverter<VariableBase>
    {
        protected override void EmitValue(IEmitter emitter, VariableBase definition)
        {
            emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));

            switch (definition)
            {
                case Variable variable:
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "name"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variable.Name));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "value"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, variable.Value.ToString()!));
                    break;

                case VariableGroup group:
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, "group"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, group.Name));
                    break;

                default:
                    throw new InvalidOperationException("Unknown VariableBase type");
            }

            emitter.Emit(new MappingEnd());
        }
    }
}
