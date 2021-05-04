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

        /// <summary>
        /// This method is used for double-linking of the definition expression tree.
        /// </summary>
        /// <param name="condition">Parent condition</param>
        /// <param name="definition">Definition that was added below the condition</param>
        /// <returns>The conditioned definition coming out of the inputs</returns>
        internal static ConditionedDefinition<T> Link<T>(Condition condition, T definition)
        {
            var conditionedDefinition = new ConditionedDefinition<T>(definition, condition.ToString());
            condition.Parent?.Definitions.Add(conditionedDefinition);
            conditionedDefinition.Parent = condition.Parent;
            return conditionedDefinition;
        }
    }

    /// <summary>
    /// Represents an item that might or might have a condition.
    /// Example of regular definition:
    ///     - task: publish
    /// Example of conditioned definition:
    ///     - ${{ if eq(variables._RunAsInternal, True) }}:
    ///       name: NetCoreInternal-Pool
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

        internal ConditionedDefinition(T definition, string condition) : base(condition)
        {
            Definition = definition;
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
            }

            // When we define an actual definition (not a nested if), we expect the Definition property to be set
            if (Definition != null)
            {
                nestedObjectSerializer(Definition, Definition.GetType());
            }

            // Otherwise, we expect a list of Definitions
            foreach (var childDefinition in Definitions)
            {
                nestedObjectSerializer(childDefinition, childDefinition.GetType());
            }

            if (!string.IsNullOrEmpty(Condition))
            {
                emitter.Emit(new SequenceEnd());
                emitter.Emit(new MappingEnd());
            }
        }
    }
}
