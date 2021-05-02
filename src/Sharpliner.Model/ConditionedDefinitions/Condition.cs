namespace Sharpliner.Model
{
    /// <summary>
    /// Represents an ${{ if ... }} statement in the YAML.
    /// When we build trees of definitions with conditions on them, we either start with a definition or a condition.
    /// A condition then has to evolve into a conditioned definition (so that we have something inside the "if").
    /// </summary>
    public abstract class Condition
    {
        protected readonly string _condition;

        protected Condition(string condition) => _condition = condition;

        internal ConditionedDefinition? Parent { get; set; }

        public override string ToString() => _condition;
    }

    /// <summary>
    /// This generic version exists so that we can enforce the type of items that are bound by the condition.
    /// The condition eventually translates into a string but the added value is the Parent pointer.
    /// That way, when we define the whole tree of conditions, the expression ends with the leaf node.
    /// The Parent pointer allows us to traverse up to the top-level condition.
    /// </summary>
    public abstract class Condition<T> : Condition
    {
        // internal new ConditionedDefinition<T>? Parent { get; set; }

        protected Condition(string condition, ConditionedDefinition<T>? parent = null) : base(condition)
        {
            Parent = parent;
        }
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
}
