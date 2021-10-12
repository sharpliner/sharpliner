using System;

namespace Sharpliner.AzureDevOps
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

        internal Conditioned? Parent { get; set; }

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
        protected Condition(string condition, Conditioned<T>? parent = null) : base(condition)
        {
            Parent = parent;
        }
    }

    public class NotCondition : Condition
    {
        internal NotCondition(Condition condition)
            : base(NotConditionHelper.NegateCondition(condition.ToString()))
        {
        }

        internal NotCondition(string condition)
            : base(NotConditionHelper.NegateCondition(condition))
        {
        }
    }

    public class EqualityCondition : Condition
    {
        internal EqualityCondition(string expression1, string expression2, bool equal)
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

    public class NotCondition<T> : Condition<T>
    {
        internal NotCondition(Condition condition)
            : base(NotConditionHelper.NegateCondition(condition.ToString()))
        {
        }

        internal NotCondition(string condition)
            : base(NotConditionHelper.NegateCondition(condition))
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
    public class BranchCondition : EqualityCondition
    {
        internal BranchCondition(string branchName, bool equal)
            : base("variables['Build.SourceBranch']", '\'' + (branchName.StartsWith("refs/heads/") ? branchName : "refs/heads/" + branchName) + '\'', equal)
        {
        }
    }

    public class BranchCondition<T> : EqualityCondition<T>
    {
        internal BranchCondition(string branchName, bool equal)
            : base("variables['Build.SourceBranch']", '\'' + (branchName.StartsWith("refs/heads/") ? branchName : "refs/heads/" + branchName) + '\'', equal)
        {
        }
    }

    public class BuildReasonCondition : EqualityCondition
    {
        internal BuildReasonCondition(string reason, bool equal)
            : base("variables['Build.Reason']", reason, equal)
        {
        }
    }

    public class BuildReasonCondition<T> : EqualityCondition<T>
    {
        internal BuildReasonCondition(string reason, bool equal)
            : base("variables['Build.Reason']", reason, equal)
        {
        }
    }

    internal static class NotConditionHelper
    {
        public static string NegateCondition(string condition)
        {
            if (condition.StartsWith("eq("))
            {
                return string.Concat("ne", condition.AsSpan(2));
            }

            if (condition.StartsWith("ne("))
            {
                return string.Concat("eq", condition.AsSpan(2));
            }

            return $"not({condition})";
        }
    }
}
