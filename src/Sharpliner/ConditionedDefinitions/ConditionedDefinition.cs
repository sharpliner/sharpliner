using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner
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

        /// <summary>
        /// This method is used for double-linking of the definition expression tree.
        /// </summary>
        /// <param name="condition">Parent condition</param>
        /// <param name="definition">Definition that was added below the condition</param>
        /// <returns>The conditioned definition coming out of the inputs</returns>
        internal static ConditionedDefinition<T> Link<T>(Condition condition, ConditionedDefinition<T> conditionedDefinition)
        {
            condition.Parent?.Definitions.Add(conditionedDefinition);
            conditionedDefinition.Parent = condition.Parent;
            return conditionedDefinition;
        }

        /// <summary>
        /// This method is used for double-linking of the definition expression tree.
        /// </summary>
        /// <param name="condition">Parent condition</param>
        /// <param name="definition">Definition that was added below the condition</param>
        /// <returns>The conditioned definition coming out of the inputs</returns>
        internal static ConditionedDefinition<T> Link<T>(Condition condition, Template<T> template)
        {
            condition.Parent?.Definitions.Add(template);
            template.Parent = condition.Parent;
            return template;
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

        public ConditionedDefinition(T definition) : base((string?)null)
        {
            Definition = definition;
        }

        protected ConditionedDefinition(string? condition) : base(condition)
        {
        }

        public ConditionBuilder<T> If => new(this);

        public ConditionedDefinition<T> EndIf => Parent as ConditionedDefinition<T>
            ?? throw new InvalidOperationException("You have called EndIf on a top level statement. EndIf should be only used to return from a nested definition.");

        public override void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            if (!string.IsNullOrEmpty(Condition))
            {
                emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, true, MappingStyle.Block));
                emitter.Emit(new Scalar("${{ if " + Condition + " }}"));
                emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Block));
            }

            // We are first serializing us. We can be
            //   - a condition-less definition (top level or leaf) => serialize value inside Definition
            //   - a template => serialize the special shape of template + parameters
            SerializeSelf(emitter, nestedObjectSerializer);

            // Otherwise, we expect a list of Definitions
            foreach (var childDefinition in Definitions)
            {
                nestedObjectSerializer(childDefinition);
            }

            if (!string.IsNullOrEmpty(Condition))
            {
                emitter.Emit(new SequenceEnd());
                emitter.Emit(new MappingEnd());
            }
        }

        protected virtual void SerializeSelf(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            if (Definition != null)
            {
                nestedObjectSerializer(Definition);
            }
        }

        internal IEnumerable<T> FlattenDefinitions()
        {
            var definitions = new List<T>();

            if (Definition is not null)
            {
                definitions.Add(Definition);
            }

            definitions.AddRange(
                Definitions
                    .SelectMany(s => (s as ConditionedDefinition<T>)?.FlattenDefinitions()!)
                    .Where(s => s is not null));

            return definitions;
        }
    }
}
