using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public abstract record Definition<T>
    {
        internal T Value { get; }

        protected Definition(T value)
        {
            Value = value;
        }
    }

    public record ConditionedDefinition<T> : Definition<T>
    {
        internal ConditionedDefinition<T>? Parent { get; set; }

        internal List<T> Definitions { get; }

        internal string? Condition { get; }

        internal ConditionedDefinition(string? condition, T definition) : base(definition)
        {
            Condition = condition;
            Definitions = new List<T>
            {
                definition
            };
        }

        public DefinitionConditionBuilder<T> If => new(this);

        public ConditionedDefinition<T> EndIf => Parent
            ?? throw new InvalidOperationException("You have called EndIf on a top level statement. EndIf should be only used to return from a nested definition.");
    }

    public abstract class DefinitionCondition
    {
        protected readonly string _condition;

        protected DefinitionCondition(string condition) => _condition = condition;

        public override string ToString() => _condition;
    }

    public abstract class DefinitionCondition<T> : DefinitionCondition
    {
        protected DefinitionCondition(string condition, ConditionedDefinition<T>? parent = null) : base(condition)
        {
            Parent = parent;
        }

        internal Definition<T>? Parent { get; set; }
    }

    public class EqualityDefinitionCondition : DefinitionCondition
    {
        internal EqualityDefinitionCondition(string expression1, string expression2, bool equal)
            : base((equal ? "eq" : "ne") + $"({expression1}, {expression2})")
        {
        }
    }

    public class AndDefinitionCondition : DefinitionCondition
    {
        internal AndDefinitionCondition(DefinitionCondition expression1, DefinitionCondition expression2)
            : base($"and({expression1}, {expression2})")
        {
        }
    }

    public class OrDefinitionCondition : DefinitionCondition
    {
        internal OrDefinitionCondition(DefinitionCondition expression1, DefinitionCondition expression2)
            : base($"or({expression1}, {expression2})")
        {
        }
    }

    public class EqualityDefinitionCondition<T> : DefinitionCondition<T>
    {
        internal EqualityDefinitionCondition(string expression1, string expression2, bool equal)
            : base((equal ? "eq" : "ne") + $"({expression1}, {expression2})")
        {
        }
    }

    public class AndDefinitionCondition<T> : DefinitionCondition<T>
    {
        internal AndDefinitionCondition(DefinitionCondition expression1, DefinitionCondition expression2)
            : base($"and({expression1}, {expression2})")
        {
        }
    }

    public class OrDefinitionCondition<T> : DefinitionCondition<T>
    {
        internal OrDefinitionCondition(DefinitionCondition expression1, DefinitionCondition expression2)
            : base($"or({expression1}, {expression2})")
        {
        }
    }

    public class DefinitionConditionBuilder<T>
    {
        internal Definition<T>? Parent { get; }

        internal DefinitionConditionBuilder(ConditionedDefinition<T>? parent = null)
        {
            Parent = parent;
        }

        public DefinitionCondition<T> Equal(string expression1, string expression2)
            => new EqualityDefinitionCondition<T>(expression1, expression2, true);

        public DefinitionCondition<T> NotEqual(string expression1, string expression2)
            => new EqualityDefinitionCondition<T>(expression1, expression2, false);

        public DefinitionCondition<T> Equal(DefinitionCondition<T> condition)
            => condition;

        public DefinitionCondition<T> NotEqual(DefinitionCondition<T> condition)
            => condition;

        public DefinitionCondition<T> And(DefinitionCondition<T> condition1, DefinitionCondition<T> condition2)
            => new AndDefinitionCondition<T>(condition1, condition2);

        public DefinitionCondition<T> Or(DefinitionCondition<T> condition1, DefinitionCondition<T> condition2)
            => new OrDefinitionCondition<T>(condition1, condition2);

        public DefinitionCondition And(DefinitionCondition condition1, DefinitionCondition condition2)
            => new AndDefinitionCondition(condition1, condition2);

        public DefinitionCondition Or(DefinitionCondition condition1, DefinitionCondition condition2)
            => new OrDefinitionCondition(condition1, condition2);
    }
}
