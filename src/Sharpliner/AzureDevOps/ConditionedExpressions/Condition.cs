using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// <para>
/// Represents an <c>${{ if ... }}</c> statement in the YAML.
/// </para>
/// <para>
/// When we build trees of definitions with conditions on them, we either start with a definition or a condition.
/// </para>
/// <para>
/// A condition then has to evolve into a conditioned definition (so that we have something inside the "if").
/// </para>
/// </summary>
public abstract class Condition : IYamlConvertible
{
    internal const string ExpressionStart = "${{ ";
    internal const string ExpressionEnd = " }}";

    internal const string IfTagStart = $"{ExpressionStart}if ";
    internal const string ElseTagStart = $"{ExpressionStart}else";

    private const string VariableIndexAccessStart = "variables[";
    private const string VariablePropertyAccessStart = "variables.";
    private const string ParametersIndexAccessStart = "parameters[";
    private const string ParametersPropertyAccessStart = "parameters.";

    internal virtual string TagStart => IfTagStart;
    internal virtual string TagEnd => ExpressionEnd;

    internal Conditioned? Parent { get; set; }

    internal EachExpression? EachExpression
    {
        get;
        set;
    }

    internal abstract string Serialize();

    /// <summary>
    /// Serializes the condition to a string.
    /// </summary>
    /// <param name="value">The condition.</param>
    public static implicit operator string(Condition value) => value.Serialize();

    /// <inheritdoc />
    public override string ToString() => this;

    internal static string Join(IEnumerable<string> expressions) => string.Join(", ", expressions);

    internal static string WrapQuotes(string value)
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

    internal static string Serialize(IfArrayExpression arrayValue)
    {
        return IfStringConditionHelper.Serialize(arrayValue);
    }

    internal static string Serialize(IfExpression stringOrVariableOrParameter)
    {
        return IfStringConditionHelper.Serialize(stringOrVariableOrParameter);
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) =>
        emitter.Emit(new Scalar(Serialize()));
}

internal class InlineCustomCondition(string condition) : InlineCondition
{
    internal override string Serialize() => condition;
}

internal class IfCustomCondition(string condition) : IfCondition
{
    internal override string Serialize() => WrapTag(condition);
}

internal class InlineNotCondition : InlineCondition
{
    private readonly string _condition;

    internal InlineNotCondition(InlineCondition condition)
        : this(condition.Serialize())
    {
    }

    internal InlineNotCondition(string condition)
    {
        _condition = NotConditionHelper.NegateCondition(condition);
    }

    internal override string Serialize() => _condition;
}

internal class IfNotCondition : IfCondition
{
    private readonly string _condition;

    internal IfNotCondition(IfCondition condition)
        : this(condition.WithoutTags())
    {
    }

    internal IfNotCondition(string condition)
    {
        _condition = NotConditionHelper.NegateCondition(WithoutTags(condition));
    }

    internal override string Serialize() => WrapTag(_condition);
}

internal class IfEqualityCondition : IfStringCondition
{
    internal IfEqualityCondition(bool equal, IfExpression expression1, IfExpression expression2)
        : base(equal ? "eq" : "ne", expression1, expression2)
    {
    }
}

internal class InlineEqualityCondition : InlineStringCondition
{
    internal InlineEqualityCondition(bool equal, InlineExpression expression1, InlineExpression expression2)
        : base(equal ? "eq" : "ne", expression1, expression2)
    {
    }
}

internal class InlineAndCondition : InlineConjunctionCondition
{
    internal InlineAndCondition(params InlineCondition[] expressions) : base("and", expressions)
    {
    }

    internal InlineAndCondition(params string[] expressions) : base("and", expressions)
    {
    }
}

internal class IfAndCondition : IfConjunctionCondition
{
    internal IfAndCondition(params IfCondition[] expressions) : base("and", expressions)
    {
    }

    internal IfAndCondition(params string[] expressions) : base("and", expressions)
    {
    }
}

internal class InlineOrCondition : InlineConjunctionCondition
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

internal class IfOrCondition : IfConjunctionCondition
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

internal class InlineConjunctionCondition : InlineCondition
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

internal class InlineConjunctionCondition<T> : InlineCondition<T>
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

internal class IfConjunctionCondition : IfCondition
{
    private readonly string _keyword;
    private readonly string[] _expressions;

    internal IfConjunctionCondition(string keyword, params IfCondition[] expressions)
        : this(keyword, expressions.Select(WithoutTags).ToArray())
    {
    }

    internal IfConjunctionCondition(string keyword, params string[] expressions)
    {
        _keyword = keyword;
        _expressions = expressions;
    }

    internal override string Serialize() => WrapTag($"{_keyword}({Join(_expressions)})");
}

internal class IfConjunctionCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string[] _expressions;

    internal IfConjunctionCondition(string keyword, IfCondition[] expressions, Conditioned<T>? parent = null)
        : this(keyword, expressions.Select(e => e.Serialize()).ToArray(), parent)
    {
    }

    internal IfConjunctionCondition(string keyword, string[] expressions, Conditioned<T>? parent = null) : base(parent)
    {
        _keyword = keyword;
        _expressions = expressions.Select(WithoutTags).ToArray();
    }

    internal override string Serialize() => WrapTag($"{_keyword}({Join(_expressions)})");
}

internal class InlineXorCondition : InlineConjunctionCondition
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

internal class IfXorCondition : IfConjunctionCondition
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

internal class InlineContainsCondition : InlineStringCondition
{
    internal InlineContainsCondition(InlineExpression needle, InlineExpression haystack)
        : base("contains", haystack, needle)
    {
    }
}

internal class IfContainsCondition : IfStringCondition
{
    internal IfContainsCondition(IfExpression needle, IfExpression haystack)
        : base("contains", haystack, needle)
    {
    }
}

internal class InlineStartsWithCondition : InlineStringCondition
{
    internal InlineStartsWithCondition(InlineExpression needle, InlineExpression haystack)
        : base("startsWith", haystack, needle)
    {
    }
}

internal class IfStartsWithCondition : IfStringCondition
{
    internal IfStartsWithCondition(IfExpression needle, IfExpression haystack)
        : base("startsWith", haystack, needle)
    {
    }
}

internal class InlineEndsWithCondition : InlineStringCondition
{
    internal InlineEndsWithCondition(InlineExpression needle, InlineExpression haystack)
        : base("endsWith", haystack, needle)
    {
    }
}

internal class IfEndsWithCondition : IfStringCondition
{
    internal IfEndsWithCondition(IfExpression needle, IfExpression haystack)
        : base("endsWith", haystack, needle)
    {
    }
}

internal class InlineInCondition : InlineStringCondition
{
    internal InlineInCondition(InlineExpression needle, params InlineExpression[] haystack)
        : base("in", needle, haystack)
    {
    }
}

internal class IfInCondition : IfStringCondition
{
    internal IfInCondition(IfExpression needle, params IfExpression[] haystack)
        : base("in", needle, haystack)
    {
    }
}

internal class InlineNotInCondition : InlineStringCondition
{
    internal InlineNotInCondition(InlineExpression needle, params InlineExpression[] haystack)
        : base("notIn", needle, haystack)
    {
    }
}

internal class IfNotInCondition : IfStringCondition
{
    internal IfNotInCondition(IfExpression needle, params IfExpression[] haystack)
        : base("notIn", needle, haystack)
    {
    }
}

internal class InlineContainsValueCondition : InlineStringCondition
{
    internal InlineContainsValueCondition(InlineExpression needle, params InlineExpression[] haystack)
        : base("containsValue", haystack, needle)
    {
    }
}

internal class IfContainsValueCondition : IfStringCondition
{
    internal IfContainsValueCondition(IfExpression needle, params IfExpression[] haystack)
        : base("containsValue", haystack, needle)
    {
    }
}

internal class InlineGreaterCondition : InlineStringCondition
{
    internal InlineGreaterCondition(InlineExpression first, InlineExpression second)
        : base("gt", first, second)
    {
    }
}

internal class IfGreaterCondition : IfStringCondition
{
    internal IfGreaterCondition(IfExpression first, IfExpression second)
        : base("gt", first, second)
    {
    }
}

internal class InlineLessCondition : InlineStringCondition
{
    internal InlineLessCondition(InlineExpression first, InlineExpression second)
        : base("lt", first, second)
    {
    }
}

internal class IfLessCondition : IfStringCondition
{
    internal IfLessCondition(IfExpression first, IfExpression second)
        : base("lt", first, second)
    {
    }
}

internal class InlineCustomCondition<T>(string condition) : InlineCondition<T>
{
    internal override string Serialize() => condition;
}

internal class IfCustomCondition<T>(string condition) : IfCondition<T>
{
    internal override string Serialize() => WrapTag(condition);
}

internal class InlineNotCondition<T> : InlineCondition<T>
{
    private readonly string _condition;

    internal InlineNotCondition(Condition condition)
        : this(condition.Serialize())
    {
    }

    internal InlineNotCondition(string condition)
    {
        _condition = NotConditionHelper.NegateCondition(condition);
    }

    internal override string Serialize() => _condition;
}

internal class IfNotCondition<T> : IfCondition<T>
{
    private readonly string _condition;

    internal IfNotCondition(IfCondition condition)
        : this(condition.WithoutTags())
    {
    }

    internal IfNotCondition(string condition)
    {
        _condition = NotConditionHelper.NegateCondition(WithoutTags(condition));
    }

    internal override string Serialize() => _condition;
}

internal class ElseCondition<T> : IfCondition<T>
{
    internal override string Serialize() => ElseTagStart + ExpressionEnd;

    internal override string TagStart => ElseTagStart;

    internal ElseCondition()
    {
    }

    public static implicit operator string(ElseCondition<T> _) => ElseTagStart + ExpressionEnd;

    public override string ToString() => ElseTagStart + ExpressionEnd;
}

internal class InlineEqualityCondition<T> : InlineStringCondition<T>
{
    internal InlineEqualityCondition(bool equal, InlineExpression expression1, InlineExpression expression2)
        : base(equal ? "eq" : "ne", expression1, expression2)
    {
    }
}

internal class IfEqualityCondition<T> : IfStringCondition<T>
{
    internal IfEqualityCondition(bool equal, IfExpression expression1, IfExpression expression2)
        : base(equal ? "eq" : "ne", expression1, expression2)
    {
    }
}


internal class InlineAndCondition<T> : InlineConjunctionCondition<T>
{
    internal InlineAndCondition(params InlineCondition[] expressions)
        : base("and", expressions)
    {
    }

    internal InlineAndCondition(params string[] expressions)
        : base("and", expressions)
    {
    }
}

internal class IfAndCondition<T> : IfConjunctionCondition<T>
{
    internal IfAndCondition(params IfCondition[] expressions)
        : base("and", expressions)
    {
    }

    internal IfAndCondition(params string[] expressions)
        : base("and", expressions)
    {
    }
}

internal class InlineOrCondition<T> : InlineConjunctionCondition<T>
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

internal class IfOrCondition<T> : IfConjunctionCondition<T>
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

internal class InlineXorCondition<T> : InlineConjunctionCondition<T>
{
    internal InlineXorCondition(InlineCondition expression1, InlineCondition expression2)
        : base("xor", new[] { expression1, expression2 })
    {
    }

    internal InlineXorCondition(string expression1, string expression2)
        : base("xor", new[] { expression1, expression2 })
    {
    }
}

internal class IfXorCondition<T> : IfConjunctionCondition<T>
{
    internal IfXorCondition(IfCondition expression1, IfCondition expression2)
        : base("xor", new[] { expression1, expression2 })
    {
    }

    internal IfXorCondition(string expression1, string expression2)
        : base("xor", new[] { expression1, expression2 })
    {
    }
}

internal class InlineContainsCondition<T> : InlineStringCondition<T>
{
    internal InlineContainsCondition(InlineExpression haystack, InlineExpression needle)
        : base("contains", haystack, needle)
    {
    }
}

internal class IfContainsCondition<T> : IfStringCondition<T>
{
    internal IfContainsCondition(IfExpression haystack, IfExpression needle)
        : base("contains", haystack, needle)
    {
    }
}

internal class InlineStartsWithCondition<T> : InlineStringCondition<T>
{
    internal InlineStartsWithCondition(InlineExpression needle, InlineExpression haystack)
        : base("startsWith", haystack, needle)
    {
    }
}

internal class IfStartsWithCondition<T> : IfStringCondition<T>
{
    internal IfStartsWithCondition(IfExpression needle, IfExpression haystack)
        : base("startsWith", haystack, needle)
    {
    }
}

internal class InlineEndsWithCondition<T> : InlineStringCondition<T>
{
    internal InlineEndsWithCondition(InlineExpression needle, InlineExpression haystack)
        : base("endsWith", haystack, needle)
    {
    }
}

internal class IfEndsWithCondition<T> : IfStringCondition<T>
{
    internal IfEndsWithCondition(IfExpression needle, IfExpression haystack)
        : base("endsWith", haystack, needle)
    {
    }
}

internal class InlineContainsValueCondition<T> : InlineStringCondition<T>
{
    internal InlineContainsValueCondition(InlineExpression needle, params InlineExpression[] haystack)
        : base("containsValue", haystack, needle)
    {
    }
}

internal class IfContainsValueCondition<T> : IfStringCondition<T>
{
    internal IfContainsValueCondition(IfExpression needle, params IfExpression[] haystack)
        : base("containsValue", haystack, needle)
    {
    }
}

internal class InlineInCondition<T> : InlineStringCondition<T>
{
    internal InlineInCondition(InlineExpression needle, params InlineExpression[] haystack)
        : base("in", needle, haystack)
    {
    }
}

internal class IfInCondition<T> : IfStringCondition<T>
{
    internal IfInCondition(IfExpression needle, params IfExpression[] haystack)
        : base("in", needle, haystack)
    {
    }
}

internal class InlineNotInCondition<T> : InlineStringCondition<T>
{
    internal InlineNotInCondition(InlineExpression needle, params InlineExpression[] haystack)
        : base("notin", needle, haystack)
    {
    }
}

internal class IfNotInCondition<T> : IfStringCondition<T>
{
    internal IfNotInCondition(IfExpression needle, params IfExpression[] haystack)
        : base("notin", needle, haystack)
    {
    }
}

internal class InlineGreaterCondition<T> : InlineStringCondition<T>
{
    internal InlineGreaterCondition(InlineExpression first, InlineExpression second)
        : base("gt", first, second)
    {
    }
}

internal class IfGreaterCondition<T> : IfStringCondition<T>
{
    internal IfGreaterCondition(IfExpression first, IfExpression second)
        : base("gt", first, second)
    {
    }
}

internal class InlineLessCondition<T> : InlineStringCondition<T>
{
    internal InlineLessCondition(InlineExpression first, InlineExpression second)
        : base("lt", first, second)
    {
    }
}

internal class IfLessCondition<T> : IfStringCondition<T>
{
    internal IfLessCondition(IfExpression first, IfExpression second)
        : base("lt", first, second)
    {
    }
}

internal class InlineBranchCondition : InlineEqualityCondition
{
    internal InlineBranchCondition(InlineExpression branchName, bool equal)
        : base(equal, BuildVariableReference.Instance.SourceBranch, BranchNameHelper.FormatBranchName(Serialize(branchName)))
    {
    }
}

internal class IfBranchCondition : IfEqualityCondition
{
    internal IfBranchCondition(IfExpression branchName, bool equal)
        : base(equal, BuildVariableReference.Instance.SourceBranch, BranchNameHelper.FormatBranchName(Serialize(branchName)))
    {
    }
}

internal class InlineBranchCondition<T> : InlineEqualityCondition<T>
{
    internal InlineBranchCondition(InlineExpression branchName, bool equal)
        : base(equal, BuildVariableReference.Instance.SourceBranch, BranchNameHelper.FormatBranchName(Serialize(branchName)))
    {
    }
}

internal class IfBranchCondition<T> : IfEqualityCondition<T>
{
    internal IfBranchCondition(IfExpression branchName, bool equal)
        : base(equal, BuildVariableReference.Instance.SourceBranch, BranchNameHelper.FormatBranchName(Serialize(branchName)))
    {
    }
}

internal class InlineBuildReasonCondition : InlineEqualityCondition
{
    internal InlineBuildReasonCondition(InlineExpression reason, bool equal)
        : base(equal, BuildVariableReference.Instance.Reason, reason)
    {
    }
}

internal class IfBuildReasonCondition : IfEqualityCondition
{
    internal IfBuildReasonCondition(IfExpression reason, bool equal)
        : base(equal, BuildVariableReference.Instance.Reason, reason)
    {
    }
}

internal class InlineBuildReasonCondition<T> : InlineEqualityCondition<T>
{
    internal InlineBuildReasonCondition(InlineExpression reason, bool equal)
        : base(equal, BuildVariableReference.Instance.Reason, reason)
    {
    }
}

internal class IfBuildReasonCondition<T> : IfEqualityCondition<T>
{
    internal IfBuildReasonCondition(IfExpression reason, bool equal)
        : base(equal, BuildVariableReference.Instance.Reason, reason)
    {
    }
}
