using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

public abstract class IfStringCondition : IfCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, IfStringOrVariableOrParameterArray one, IfStringOrVariableOrParameter two)
        : this(keyword, Serialize(one), two)
    {
    }

    protected IfStringCondition(string keyword, IfStringOrVariableOrParameter one, IfStringOrVariableOrParameterArray two)
        : this(keyword, one, Serialize(two))
    {
    }

    protected IfStringCondition(string keyword, IfStringOrVariableOrParameter one, IfStringOrVariableOrParameter two)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
    }

    protected static string Serialize(IfStringOrVariableOrParameterArray arrayValue)
    {
        return IfStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(IfStringOrVariableOrParameter stringOrVariableOrParameter)
    {
        return IfStringConditionHelper.Serialize(stringOrVariableOrParameter);
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}

public abstract class IfStringCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, IfStringOrVariableOrParameterArray one, IfStringOrVariableOrParameter two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    protected IfStringCondition(string keyword, IfStringOrVariableOrParameter one, IfStringOrVariableOrParameterArray two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    protected IfStringCondition(string keyword, IfStringOrVariableOrParameter one, IfStringOrVariableOrParameter two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    protected static string Serialize(IfStringOrVariableOrParameterArray arrayValue)
    {
        return IfStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(IfStringOrVariableOrParameter stringOrVariableOrParameter)
    {
        return IfStringConditionHelper.Serialize(stringOrVariableOrParameter);
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}
