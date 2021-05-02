using System.Collections.Generic;

namespace Sharpliner.Model
{
    public abstract record ConditionedDefinition
    {
        internal string? Condition { get; }

        internal ConditionedDefinition? Parent { get; set; }

        internal List<ConditionedDefinition> Definitions { get; } = new();

        protected ConditionedDefinition(string? condition)
        {
            Condition = condition;
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
    }
}
