using System.Collections.Generic;

namespace Sharpliner.Model
{
    public abstract record ConditionedDefinition
    {
        internal ConditionedDefinition? Parent { get; set; }

        /// <summary>
        /// Determines in which order elements from Definitions and Conditions are stored.
        /// False = Definition
        /// True = Condition
        /// </summary>
        internal List<bool> Order { get; } = new();

        internal List<Condition> Conditions { get; } = new();
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
        internal List<T> Definitions { get; }

        internal string? Condition { get; }

        internal ConditionedDefinition(T definition, string? condition)
        {
            Condition = condition;

            Definitions = new List<T>
            {
                definition
            };
        }

        public ConditionBuilder<T> If => new(this);
    }
}
