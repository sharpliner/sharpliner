using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Represents an ${{ if ... }} statement in the YAML.
/// When we build trees of definitions with conditions on them, we either start with a definition or a condition.
/// A condition then has to evolve into a conditioned definition (so that we have something inside the "if").
/// </summary>
public abstract class Condition
{
    internal const string IfTagStart = "${{ if ";
    internal const string IfTagEnd = " }}";

    protected readonly string _condition;

    protected Condition(string condition)
    {
        _condition = condition;
    }
    protected Condition(string keyword, bool requireTwoPlus, params Condition[] expressions)
        : this(keyword, requireTwoPlus, expressions.Select(e => e.ToString()))
    {}

    protected Condition(string keyword, bool requireTwoPlus, params string[] expressions)
        : this(keyword, requireTwoPlus, expressions as IEnumerable<string>)
    {}

    protected Condition(string keyword, bool requireTwoPlus, IEnumerable<string> expressions)
    {
        if (requireTwoPlus && expressions.Count() < 2)
        {
            throw new ArgumentException($"You need to provide at least 2 values for the {keyword}() operator");
        }

        _condition = $"{keyword}({string.Join(", ", expressions.Select(RemoveIf))})";
    }

    internal Conditioned? Parent { get; set; }

    public override string ToString() => _condition;

    public static implicit operator string(Condition value) => IfTagStart + value.ToString() + IfTagEnd;

    protected static string RemoveIf(Condition condition) => RemoveIf(condition.ToString());

    protected static string RemoveIf(string condition)
    {
        if (condition.StartsWith(IfTagStart) && condition.EndsWith(IfTagEnd))
        {
            return condition.Substring(IfTagStart.Length, condition.Length - IfTagStart.Length - IfTagEnd.Length);
        }

        return condition;
    }

    protected static string BuildExpression(string keyword, params Condition[] expressions)
        => BuildExpression(keyword, expressions.Select(e => e.ToString()));

    protected static string BuildExpression(string keyword, params string[] expressions)
        => BuildExpression(keyword, expressions as IEnumerable<string>);

    protected static string BuildExpression(string keyword, IEnumerable<string> expressions)
    {
        if (expressions.Count() < 2)
        {
            throw new ArgumentException($"You need to provide at least 2 values for the {keyword}() operator");
        }

        return $"{keyword}({string.Join(", ", expressions.Select(RemoveIf))})";
    }
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

    protected Condition(string keyword, bool requireTwoPlus, params Condition[] expressions)
        : base(keyword, requireTwoPlus, expressions)
    {
    }

    protected Condition(string keyword, bool requireTwoPlus, params string[] expressions)
        : base(keyword, requireTwoPlus, expressions)
    {
    }

    protected Condition(string keyword, bool requireTwoPlus, IEnumerable<string> expressions)
        : base(keyword, requireTwoPlus, expressions)
    {
    }
}

public class NotCondition : Condition
{
    internal NotCondition(Condition condition)
        : this(condition.ToString())
    {
    }

    internal NotCondition(string condition)
        : base(NotConditionHelper.NegateCondition(RemoveIf(condition)))
    {
    }
}

public class EqualityCondition : Condition
{
    internal EqualityCondition(bool equal, string expression1, string expression2)
        : base(equal ? "eq" : "ne", true, expression1, expression2)
    {
    }
}

public class AndCondition : Condition
{
    internal AndCondition(params Condition[] expressions)
        : base("and", true, expressions)
    {
    }

    internal AndCondition(params string[] expressions)
        : base("and", true, expressions)
    {
    }
}

public class OrCondition : Condition
{
    internal OrCondition(params Condition[] expressions)
        : base("or", true, expressions)
    {
    }

    internal OrCondition(params string[] expressions)
        : base("or", true, expressions)
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
    internal EqualityCondition(bool equal, string expression1, string expression2)
        : base(equal ? "eq" : "ne", true, expression1, expression2)
    {
    }
}

public class AndCondition<T> : Condition<T>
{
    internal AndCondition(params Condition[] expressions)
        : base("and", true, expressions)
    {
    }

    internal AndCondition(params string[] expressions)
        : base("and", true, expressions)
    {
    }
}

public class OrCondition<T> : Condition<T>
{
    internal OrCondition(params Condition[] expressions)
        : base("or", true, expressions)
    {
    }

    internal OrCondition(params string[] expressions)
        : base("or", true, expressions)
    {
    }
}
public class BranchCondition : EqualityCondition
{
    internal BranchCondition(string branchName, bool equal)
        : base(equal, "variables['Build.SourceBranch']", '\'' + (branchName.StartsWith("refs/heads/") ? branchName : "refs/heads/" + branchName) + '\'')
    {
    }
}

public class BranchCondition<T> : EqualityCondition<T>
{
    internal BranchCondition(string branchName, bool equal)
        : base(equal, "variables['Build.SourceBranch']", '\'' + (branchName.StartsWith("refs/heads/") ? branchName : "refs/heads/" + branchName) + '\'')
    {
    }
}

public class BuildReasonCondition : EqualityCondition
{
    internal BuildReasonCondition(string reason, bool equal)
        : base(equal, "variables['Build.Reason']", reason)
    {
    }
}

public class BuildReasonCondition<T> : EqualityCondition<T>
{
    internal BuildReasonCondition(string reason, bool equal)
        : base(equal, "variables['Build.Reason']", reason)
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
