using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.SourceGenerator;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Represents an ${{ if ... }} statement in the YAML.
/// When we build trees of definitions with conditions on them, we either start with a definition or a condition.
/// A condition then has to evolve into a conditioned definition (so that we have something inside the "if").
/// </summary>
public abstract class Condition
{
    protected Condition()
    {
    }

    protected abstract string Serialize();

    internal const string ExpressionStart = "${{ ";
    private const string IfTagStart = $"{ExpressionStart}if ";
    protected const string ElseTagStart = $"{ExpressionStart}else";
    internal const string ExpressionEnd = " }}";

    private const string VariableIndexAccessStart = "variables[";
    private const string VariablePropertyAccessStart = "variables.";
    private const string ParametersIndexAccessStart = "parameters[";
    private const string ParametersPropertyAccessStart = "parameters.";

    private static readonly (string Start, string End)[] s_tagsToRemove = new[]
    {
        (IfTagStart, ExpressionEnd),
        (ElseTagStart, ExpressionEnd),
        ('\'' + IfTagStart, ExpressionEnd + '\''),
        ('\'' + ElseTagStart, ExpressionEnd + '\''),
        (ExpressionStart, ExpressionEnd),
        ('\'' + ExpressionStart, ExpressionEnd + '\''),
    };

    internal virtual string TagStart => IfTagStart;
    internal virtual string TagEnd => ExpressionEnd;

    internal readonly string ConditionString;
    internal readonly string? Keyword;
    internal readonly IEnumerable<string>? Expressions;

    protected Condition(string condition)
    {
        ConditionString = condition;
    }

    protected Condition(string keyword, bool requireTwoPlus, params Condition[] expressions)
        : this(keyword, requireTwoPlus, expressions.Select(e => new string(e.ToString())))
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

        Keyword = keyword;
        Expressions = expressions;

        ConditionString = $"{keyword}({string.Join(", ", expressions.Select(SerializeExpression).Select(WrapQuotes).Select(RemoveTags))})";
    }

    private string SerializeExpression(string expression)
    {
        return expression.Match(
            str => str,
            variable => variable.RuntimeExpression,
            parameter => parameter.RuntimeExpression
        );
    }

    internal Conditioned? Parent { get; set; }

    public override string ToString() => ConditionString;

    public static implicit operator string(Condition value) => value.TagStart + value.ToString() + value.TagEnd;

    public static string RemoveTags(Condition condition) => RemoveTags(condition.ToString());

    public static string RemoveTags(string condition)
    {
        foreach (var (start, end) in s_tagsToRemove)
        {
            if (condition.StartsWith(start) && condition.EndsWith(end))
            {
                condition = condition.Substring(start.Length, condition.Length - start.Length - end.Length);
                break;
            }
        }

        return condition;
    }

    public static string WrapQuotes(string value)
    {
        if (value.StartsWith('\'')
            || bool.TryParse(value, out _)
            || long.TryParse(value, out _)
            || double.TryParse(value, out _)
            || value.StartsWith(VariableIndexAccessStart)
            || value.StartsWith(VariablePropertyAccessStart)
            || value.StartsWith(ParametersIndexAccessStart)
            || value.StartsWith(ParametersPropertyAccessStart))
        {
            return value;
        }

        return $"'{value}'";
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

        return $"{keyword}({string.Join(", ", expressions.Select(RemoveTags))})";
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
    protected Condition(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }

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

public class CustomCondition : Condition
{
    public CustomCondition(string condition) : base(RemoveTags(condition))
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
        : base(NotConditionHelper.NegateCondition(RemoveTags(condition)))
    {
    }
}
public abstract class StringCondition : Condition
{
    protected StringCondition(string keyword, bool requireTwoPlus, params string[] expressions)
        : this(keyword, requireTwoPlus, expressions as IEnumerable<string>)
    {
    }

    protected StringCondition(string keyword, bool requireTwoPlus, IEnumerable<string> expressions)
        : base(keyword, requireTwoPlus, expressions)
    {
    }
}

public abstract class StringCondition<T> : Condition<T>
{
    protected StringCondition(string keyword, bool requireTwoPlus, params string[] expressions)
        : this(keyword, requireTwoPlus, expressions as IEnumerable<string>)
    {
    }

    protected StringCondition(string keyword, bool requireTwoPlus, IEnumerable<string> expressions)
        : base(keyword, requireTwoPlus, expressions)
    {
    }
}

[StringCondition]
public class IfEqualityCondition : IfStringCondition
{
    internal IfEqualityCondition(bool equal, string expression1, string expression2)
        : base(equal ? "eq" : "ne", expression1, expression2)
    {
    }
}

[StringCondition]
public class InlineEqualityCondition : InlineStringCondition
{
    internal InlineEqualityCondition(bool equal, string expression1, string expression2)
        : base(equal ? "eq" : "ne", expression1, expression2)
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
        : base("and", true, expressions.Select(e => new string(e)))
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
        : base("or", true, expressions.Select(e => new string(e)))
    {
    }
}

public class XorCondition : Condition
{
    internal XorCondition(Condition expression1, Condition expression2)
        : base("xor", true, expression1, expression2)
    {
    }

    internal XorCondition(string expression1, string expression2)
        : base("xor", true, expression1, expression2)
    {
    }
}

public class ContainsCondition : StringCondition
{
    internal ContainsCondition(string needle, string haystack)
        : base("contains", false, haystack, needle)
    {
    }
}

public class StartsWithCondition : StringCondition
{
    internal StartsWithCondition(string needle, string haystack)
        : base("startsWith", false, haystack, needle)
    {
    }
}

public class EndsWithCondition : StringCondition
{
    internal EndsWithCondition(string needle, string haystack)
        : base("endsWith", false, haystack, needle)
    {
    }
}

public class InCondition : StringCondition
{
    internal InCondition(string needle, params string[] haystack)
        : base("in", true, haystack.Prepend(needle))
    {
    }
}

public class NotInCondition : StringCondition
{
    internal NotInCondition(string needle, params string[] haystack)
        : base("notIn", true, haystack.Prepend(needle))
    {
    }
}

public class ContainsValueCondition : StringCondition
{
    internal ContainsValueCondition(string needle, params string[] haystack)
        : base("containsValue", true, haystack.Append(needle))
    {
    }
}

public class GreaterCondition : StringCondition
{
    internal GreaterCondition(string first, string second)
        : base("gt", true, first, second)
    {
    }
}

public class LessCondition : StringCondition
{
    internal LessCondition(string first, string second)
        : base("lt", true, first, second)
    {
    }
}

public class CustomCondition<T> : Condition<T>
{
    public CustomCondition(string condition) : base(condition)
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

public class ElseCondition<T> : Condition<T>
{
    internal override string TagStart => ElseTagStart;

    internal ElseCondition()
        : base("else")
    {
    }

    public static implicit operator string(ElseCondition<T> value) => ElseTagStart + ExpressionEnd;

    public override string ToString() => ElseTagStart + ExpressionEnd;
}

public class EqualityCondition<T> : StringCondition<T>
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
        : base("and", true, expressions.Select(e => new string(e)))
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
        : base("or", true, expressions.Select(e => new string(e)))
    {
    }
}

public class XorCondition<T> : Condition<T>
{
    internal XorCondition(Condition expression1, Condition expression2)
        : base("xor", true, expression1, expression2)
    {
    }

    internal XorCondition(string expression1, string expression2)
        : base("xor", true, expression1, expression2)
    {
    }
}

public class ContainsCondition<T> : StringCondition<T>
{
    internal ContainsCondition(string haystack, string needle)
        : base("contains", false, haystack, needle)
    {
    }
}

public class StartsWithCondition<T> : StringCondition<T>
{
    internal StartsWithCondition(string needle, string haystack)
        : base("startsWith", false, haystack, needle)
    {
    }
}

public class EndsWithCondition<T> : StringCondition<T>
{
    internal EndsWithCondition(string needle, string haystack)
        : base("endsWith", false, haystack, needle)
    {
    }
}

public class ContainsValueCondition<T> : StringCondition<T>
{
    internal ContainsValueCondition(string needle, params string[] haystack)
        : base("containsValue", true, haystack.Append(needle))
    {
    }
}

public class InCondition<T> : StringCondition<T>
{
    internal InCondition(string needle, params string[] haystack)
        : base("in", true, haystack.Prepend(needle))
    {
    }
}

public class NotInCondition<T> : StringCondition<T>
{
    internal NotInCondition(string needle, params string[] haystack)
        : base("notin", true, haystack.Prepend(needle))
    {
    }
}

public class GreaterCondition<T> : StringCondition<T>
{
    internal GreaterCondition(string first, string second)
        : base("gt", true, first, second)
    {
    }
}

public class LessCondition<T> : StringCondition<T>
{
    internal LessCondition(string first, string second)
        : base("lt", true, first, second)
    {
    }
}


public class BranchCondition : EqualityCondition
{
    internal BranchCondition(string branchName, bool equal)
        : base(equal, new VariableReference("Build.SourceBranch"), branchName)
    {
    }
}

public class BranchCondition<T> : EqualityCondition<T>
{
    internal BranchCondition(string branchName, bool equal)
        : base(equal, new VariableReference("Build.SourceBranch"), branchName)
    {
    }
}

public class BuildReasonCondition : EqualityCondition
{
    internal BuildReasonCondition(string reason, bool equal)
        : base(equal, new VariableReference("Build.Reason"), reason)
    {
    }
}

public class BuildReasonCondition<T> : EqualityCondition<T>
{
    internal BuildReasonCondition(string reason, bool equal)
        : base(equal, new VariableReference("Build.Reason"), reason)
    {
    }
}
