using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.Model
{
    public abstract record ConditionedDefinition : IYamlConvertible
    {
        /// <summary>
        /// Evaluated textual representation of the condition, e.g. "ne('foo', 'bar')".
        /// </summary>
        internal string? Condition { get; }

        /// <summary>
        /// Pointer in case of nested conditional blocks.
        /// </summary>
        internal ConditionedDefinition? Parent { get; set; }

        /// <summary>
        /// In case we define multiple items inside one ${{ if }}, they are stored here.
        /// </summary>
        internal List<ConditionedDefinition> Definitions { get; } = new();

        protected ConditionedDefinition(string? condition)
        {
            Condition = condition;
        }

        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
            => throw new NotImplementedException();

        public abstract void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer);
    }

    /// <summary>
    /// Represents an item that might or might have a condition.
    /// Example of regular definition:
    ///     - task: publish
    /// Example of conditioned definition:
    ///     - ${{ if eq(variables._RunAsInternal, True) }}:
    ///       name: NetCoreInternal-Pool
    ///
    /// When we define a condition, we expect a list of Definitions.
    /// When we define a condition-less definition (top-level), we only
    /// expect the Definition property to be set.
    /// </summary>
    public record ConditionedDefinition<T> : ConditionedDefinition
    {
        /// <summary>
        /// The actual definition (value).
        /// </summary>
        internal T? Definition { get; }

        internal ConditionedDefinition(ConditionedDefinition<T> definition, string condition) : base(condition)
        {
            Definitions.Add(definition);
        }

        internal ConditionedDefinition(T definition) : base((string?)null)
        {
            Definition = definition;
        }

        public ConditionBuilder<T> If => new(this);

        public override void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            if (!string.IsNullOrEmpty(Condition))
            {
                emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));
                emitter.Emit(new Scalar("${{ if " + Condition + " }}"));
                emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Block));

                foreach (var childDefinition in Definitions)
                {
                    nestedObjectSerializer(childDefinition, childDefinition.GetType());
                }

                emitter.Emit(new SequenceEnd());
                emitter.Emit(new MappingEnd());
            }
            else
            {
                nestedObjectSerializer(Definition, Definition?.GetType());
            }
        }
    }
}
