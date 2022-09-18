using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class InlineStringCondition : InlineCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected InlineStringCondition(string keyword, string[] one, string two)
    {
        _keyword = keyword;
        _one = Join(one);
        _two = two;
    }

    protected InlineStringCondition(string keyword, string one, string[] two)
    {
        _keyword = keyword;
        _one = one;
        _two = Join(two);
    }

    protected InlineStringCondition(string keyword, string one, string two)
    {
        _keyword = keyword;
        _one = one;
        _two = two;
    }

    protected static string Serialize(VariableReference staticVariableReference)
    {
        return staticVariableReference.RuntimeExpression;
    }

    protected static string Serialize(ParameterReference parameterReference)
    {
        return Condition.ExpressionStart + parameterReference.RuntimeExpression + Condition.ExpressionEnd;
    }

    protected static string Serialize(IEnumerable<object> array)
    {
        var convertedStringArray = array.Select(item =>
            {
                return item switch
                {
                    VariableReference staticVariableReference => Serialize(staticVariableReference),
                    ParameterReference parameterReference => Serialize(parameterReference),
                    _ => item.ToString()
                };
            })
            .Select(WrapQuotes);

        return string.Join(", ", convertedStringArray);
    }

    internal override string Serialize() => $"{_keyword}({_one}, {_two})";
}

public abstract class InlineStringCondition<T> : InlineStringCondition
{
    protected InlineStringCondition(string keyword, string one, string two, Conditioned<T>? parent = null)
        : base(keyword, one, two)
    {
        Parent = parent;
    }

    protected InlineStringCondition(string keyword, string[] one, string two, Conditioned<T>? parent = null)
    : this(keyword, Join(one), two, parent)
    {
        Parent = parent;
    }

    protected InlineStringCondition(string keyword, string one, string[] two, Conditioned<T>? parent = null)
        : this(keyword, one, Join(two), parent)
    {
        Parent = parent;
    }
}
