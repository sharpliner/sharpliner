using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    /// <summary>
    /// Represents an item that might or might have a condition.
    /// Example of regular definition:
    ///     - task: publish
    /// Example of conditioned definition:
    ///     - ${{ if eq(variables._RunAsInternal, True) }}:
    ///       name: NetCoreInternal-Pool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record ConditionedDefinition<T>
    {
        internal ConditionedDefinition<T>? Parent { get; set; }

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

        public ConditionedDefinition<T> EndIf => Parent
            ?? throw new InvalidOperationException("You have called EndIf on a top level statement. EndIf should be only used to return from a nested definition.");
    }
}
