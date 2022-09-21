using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

public abstract class InlineStringCondition : InlineCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected InlineStringCondition(string keyword, InlineStringOrVariableOrParameterArray one, InlineStringOrVariableOrParameter two)
        : this(keyword, Serialize(one), two)
    {
    }

    protected InlineStringCondition(string keyword, InlineStringOrVariableOrParameter one, InlineStringOrVariableOrParameterArray two)
        : this(keyword, one, Serialize(two))
    {
    }

    protected InlineStringCondition(string keyword, InlineStringOrVariableOrParameter one, InlineStringOrVariableOrParameter two)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
    }

    protected static string Serialize(InlineStringOrVariableOrParameterArray arrayValue)
    {
        return InlineStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(InlineStringOrVariableOrParameter stringOrVariableOrParameter)
    {
        return InlineStringConditionHelper.Serialize(stringOrVariableOrParameter);
    }

    internal override string Serialize() => $"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})";
}

public abstract class InlineStringCondition<T> : InlineCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected InlineStringCondition(string keyword, InlineStringOrVariableOrParameterArray one, InlineStringOrVariableOrParameter two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    protected InlineStringCondition(string keyword, InlineStringOrVariableOrParameter one, InlineStringOrVariableOrParameterArray two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    protected InlineStringCondition(string keyword, InlineStringOrVariableOrParameter one, InlineStringOrVariableOrParameter two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    protected static string Serialize(InlineStringOrVariableOrParameterArray arrayValue)
    {
        return InlineStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(InlineStringOrVariableOrParameter stringOrVariableOrParameter)
    {
        return InlineStringConditionHelper.Serialize(stringOrVariableOrParameter);
    }

    internal override string Serialize() => $"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})";
}
