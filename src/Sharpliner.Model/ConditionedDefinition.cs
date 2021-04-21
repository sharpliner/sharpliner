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

        public ConditionBuilder<T> If => new(this);

        public ConditionedDefinition<T> EndIf => Parent
            ?? throw new InvalidOperationException("You have called EndIf on a top level statement. EndIf should be only used to return from a nested definition.");
    }

    public abstract class Condition
    {
        protected readonly string _condition;

        protected Condition(string condition) => _condition = condition;

        public override string ToString() => _condition;
    }

    public abstract class Condition<T> : Condition
    {
        protected Condition(string condition, ConditionedDefinition<T>? parent = null) : base(condition)
        {
            Parent = parent;
        }

        internal Definition<T>? Parent { get; set; }
    }

    public class EqualityDefinitionCondition : Condition
    {
        internal EqualityDefinitionCondition(string expression1, string expression2, bool equal)
            : base((equal ? "eq" : "ne") + $"({expression1}, {expression2})")
        {
        }
    }

    public class AndCondition : Condition
    {
        internal AndCondition(Condition expression1, Condition expression2)
            : base($"and({expression1}, {expression2})")
        {
        }
    }

    public class OrCondition : Condition
    {
        internal OrCondition(Condition expression1, Condition expression2)
            : base($"or({expression1}, {expression2})")
        {
        }
    }

    public class EqualityCondition<T> : Condition<T>
    {
        internal EqualityCondition(string expression1, string expression2, bool equal)
            : base((equal ? "eq" : "ne") + $"({expression1}, {expression2})")
        {
        }
    }

    public class AndCondition<T> : Condition<T>
    {
        internal AndCondition(Condition expression1, Condition expression2)
            : base($"and({expression1}, {expression2})")
        {
        }
    }

    public class OrCondition<T> : Condition<T>
    {
        internal OrCondition(Condition expression1, Condition expression2)
            : base($"or({expression1}, {expression2})")
        {
        }
    }

    public class ConditionBuilder<T>
    {
        internal Definition<T>? Parent { get; }

        internal ConditionBuilder(ConditionedDefinition<T>? parent = null)
        {
            Parent = parent;
        }

        public Condition<T> Equal(string expression1, string expression2)
            => new EqualityCondition<T>(expression1, expression2, true);

        public Condition<T> NotEqual(string expression1, string expression2)
            => new EqualityCondition<T>(expression1, expression2, false);

        public Condition<T> Equal(Condition<T> condition)
            => condition;

        public Condition<T> NotEqual(Condition<T> condition)
            => condition;

        public Condition<T> And(Condition<T> condition1, Condition<T> condition2)
            => new AndCondition<T>(condition1, condition2);

        public Condition<T> Or(Condition<T> condition1, Condition<T> condition2)
            => new OrCondition<T>(condition1, condition2);

        public Condition And(Condition condition1, Condition condition2)
            => new AndCondition(condition1, condition2);

        public Condition Or(Condition condition1, Condition condition2)
            => new OrCondition(condition1, condition2);
    }
}
