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

    internal abstract string Serialize();

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

    internal Conditioned? Parent { get; set; }

    public override string ToString() => ConditionString;

    public static implicit operator string(Condition value) => value.TagStart + value.ToString() + value.TagEnd;

    public static string Join(IEnumerable<string> expressions) => string.Join(", ", expressions);

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

public class InlineCustomCondition : InlineCondition
{
    private readonly string _condition;

    public InlineCustomCondition(string condition)
    {
        _condition = condition;
    }

    internal override string Serialize() => _condition;
}

public class IfCustomCondition : IfCondition
{
    private readonly string _condition;

    public IfCustomCondition(string condition)
    {
        _condition = condition;
    }

    internal override string Serialize() => _condition;
}

public class InlineNotCondition : InlineCondition
{
    private readonly string _condition;

    internal InlineNotCondition(InlineCondition condition)
        : this(condition.Serialize())
    {
    }

    internal InlineNotCondition(string condition)
    {
        _condition = NotConditionHelper.NegateCondition(RemoveTags(condition));
    }

    internal override string Serialize() => _condition;
}

public class IfNotCondition : IfCondition
{
    private readonly string _condition;

    internal IfNotCondition(IfCondition condition)
        : this(condition.RemoveBraces())
    {
    }

    internal IfNotCondition(string condition)
    {
        _condition = NotConditionHelper.NegateCondition(RemoveTags(condition));
    }

    internal override string Serialize() => _condition;
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

public class InlineAndCondition : InlineConjunctionCondition
{
    internal InlineAndCondition(params InlineCondition[] expressions) : base("and", expressions)
    {
    }

    internal InlineAndCondition(params string[] expressions) : base("and", expressions)
    {
    }
}

public class IfAndCondition : IfConjunctionCondition
{
    internal IfAndCondition(params IfCondition[] expressions) : base("and", expressions)
    {
    }

    internal IfAndCondition(params string[] expressions) : base("and", expressions)
    {
    }
}

public class InlineOrCondition : InlineConjunctionCondition
{
    internal InlineOrCondition(params InlineCondition[] expressions)
        : base("or", expressions)
    {
    }

    internal InlineOrCondition(params string[] expressions)
        : base("or", expressions)
    {
    }
}

public class IfOrCondition : IfConjunctionCondition
{
    internal IfOrCondition(params IfCondition[] expressions)
        : base("or", expressions)
    {
    }

    internal IfOrCondition(params string[] expressions)
        : base("or", expressions)
    {
    }
}

public class InlineConjunctionCondition : InlineCondition
{
    private readonly string _keyword;
    private readonly string[] _expressions;

    internal InlineConjunctionCondition(string keyword, params InlineCondition[] expressions)
        : this(keyword, expressions.Select(e => e.Serialize()).ToArray())
    {
    }

    internal InlineConjunctionCondition(string keyword, params string[] expressions)
    {
        _keyword = keyword;
        _expressions = expressions;
    }

    internal override string Serialize() => $"{_keyword}({Join(_expressions)})";
}

public class InlineConjunctionCondition<T> : InlineCondition<T>
{
    private readonly string _keyword;
    private readonly string[] _expressions;

    internal InlineConjunctionCondition(string keyword, InlineCondition[] expressions, Conditioned<T>? parent = null)
        : this(keyword, expressions.Select(e => e.Serialize()).ToArray(), parent)
    {
    }

    internal InlineConjunctionCondition(string keyword, string[] expressions, Conditioned<T>? parent = null) : base(parent)
    {
        _keyword = keyword;
        _expressions = expressions;
    }

    internal override string Serialize() => $"{_keyword}({Join(_expressions)})";
}

public class IfConjunctionCondition : IfCondition
{
    private readonly string _keyword;
    private readonly string[] _expressions;

    internal IfConjunctionCondition(string keyword, params IfCondition[] expressions)
        : this(keyword, expressions.Select(RemoveBraces).ToArray())
    {
    }

    internal IfConjunctionCondition(string keyword, params string[] expressions)
    {
        _keyword = keyword;
        _expressions = expressions;
    }

    internal override string Serialize() => $"{ExpressionStart} if {_keyword}({Join(_expressions)}) {ExpressionEnd}";
}

public class IfConjunctionCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string[] _expressions;

    internal IfConjunctionCondition(string keyword, IfCondition[] expressions, Conditioned<T>? parent = null)
        : this(keyword, expressions.Select(RemoveBraces).ToArray(), parent)
    {
    }

    internal IfConjunctionCondition(string keyword, string[] expressions, Conditioned<T>? parent = null) : base(parent)
    {
        _keyword = keyword;
        _expressions = expressions;
    }

    internal override string Serialize() => $"{ExpressionStart} if {_keyword}({Join(_expressions)}) {ExpressionEnd}";
}

public class InlineXorCondition : InlineConjunctionCondition
{
    internal InlineXorCondition(InlineCondition expression1, InlineCondition expression2)
        : base("xor", expression1, expression2)
    {
    }

    internal InlineXorCondition(string expression1, string expression2)
        : base("xor", expression1, expression2)
    {
    }
}

public class IfXorCondition : IfConjunctionCondition
{
    internal IfXorCondition(IfCondition expression1, IfCondition expression2)
        : base("xor", expression1, expression2)
    {
    }

    internal IfXorCondition(string expression1, string expression2)
        : base("xor", expression1, expression2)
    {
    }
}

public class InlineContainsCondition : InlineStringCondition
{
    internal InlineContainsCondition(string needle, string haystack)
        : base("contains", haystack, needle)
    {
    }
}

public class IfContainsCondition : IfStringCondition
{
    internal IfContainsCondition(string needle, string haystack)
        : base("contains", haystack, needle)
    {
    }
}

public class InlineStartsWithCondition : InlineStringCondition
{
    internal InlineStartsWithCondition(string needle, string haystack)
        : base("startsWith", haystack, needle)
    {
    }
}

public class IfStartsWithCondition : IfStringCondition
{
    internal IfStartsWithCondition(string needle, string haystack)
        : base("startsWith", haystack, needle)
    {
    }
}

public class InlineEndsWithCondition : InlineStringCondition
{
    internal InlineEndsWithCondition(string needle, string haystack)
        : base("endsWith", haystack, needle)
    {
    }
}

public class IfEndsWithCondition : IfStringCondition
{
    internal IfEndsWithCondition(string needle, string haystack)
        : base("endsWith", haystack, needle)
    {
    }
}

public class InlineInCondition : InlineStringCondition
{
    internal InlineInCondition(string needle, params string[] haystack)
        : base("in", haystack.Prepend(needle))
    {
    }
}

public class IfInCondition : IfStringCondition
{
    internal IfInCondition(string needle, params string[] haystack)
        : base("in", haystack.Prepend(needle))
    {
    }
}

public class InlineNotInCondition : InlineStringCondition
{
    internal InlineNotInCondition(string needle, params string[] haystack)
        : base("notIn", haystack.Prepend(needle))
    {
    }
}

public class IfNotInCondition : IfStringCondition
{
    internal IfNotInCondition(string needle, params string[] haystack)
        : base("notIn", haystack.Prepend(needle))
    {
    }
}

public class InlineContainsValueCondition : InlineStringCondition
{
    internal InlineContainsValueCondition(string needle, params string[] haystack)
        : base("containsValue", haystack.Append(needle))
    {
    }
}

public class IfContainsValueCondition : IfStringCondition
{
    internal IfContainsValueCondition(string needle, params string[] haystack)
        : base("containsValue", haystack.Append(needle))
    {
    }
}

public class InlineGreaterCondition : InlineStringCondition
{
    internal InlineGreaterCondition(string first, string second)
        : base("gt", first, second)
    {
    }
}

public class IfGreaterCondition : IfStringCondition
{
    internal IfGreaterCondition(string first, string second)
        : base("gt", first, second)
    {
    }
}

public class InlineLessCondition : InlineStringCondition
{
    internal InlineLessCondition(string first, string second)
        : base("lt", first, second)
    {
    }
}

public class IfLessCondition : IfStringCondition
{
    internal IfLessCondition(string first, string second)
        : base("lt", first, second)
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
