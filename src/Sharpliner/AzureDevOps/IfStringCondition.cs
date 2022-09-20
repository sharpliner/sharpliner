using Sharpliner.AzureDevOps.ConditionedExpressions;
using IfConditionOneOfArrayStringValue = Sharpliner.AzureDevOps.ConditionedExpressions.Arguments.IfConditionOneOfArrayStringValue;
using IfConditionOneOfStringValue = Sharpliner.AzureDevOps.ConditionedExpressions.Arguments.IfConditionOneOfStringValue;

namespace Sharpliner.AzureDevOps;

public abstract class IfStringCondition : IfCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, IfConditionOneOfArrayStringValue one, IfConditionOneOfStringValue two)
        : this(keyword, Serialize(one), two)
    {
    }

    protected IfStringCondition(string keyword, IfConditionOneOfStringValue one, IfConditionOneOfArrayStringValue two)
        : this(keyword, one, Serialize(two))
    {
    }

    protected IfStringCondition(string keyword, IfConditionOneOfStringValue one, IfConditionOneOfStringValue two)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
    }

    protected static string Serialize(IfConditionOneOfArrayStringValue arrayValue)
    {
        return IfStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(IfConditionOneOfStringValue value)
    {
        return IfStringConditionHelper.Serialize(value);
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}

public abstract class IfStringCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, IfConditionOneOfArrayStringValue one, IfConditionOneOfStringValue two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    protected IfStringCondition(string keyword, IfConditionOneOfStringValue one, IfConditionOneOfArrayStringValue two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    protected IfStringCondition(string keyword, IfConditionOneOfStringValue one, IfConditionOneOfStringValue two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    protected static string Serialize(IfConditionOneOfArrayStringValue arrayValue)
    {
        return IfStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(IfConditionOneOfStringValue value)
    {
        return IfStringConditionHelper.Serialize(value);
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}
