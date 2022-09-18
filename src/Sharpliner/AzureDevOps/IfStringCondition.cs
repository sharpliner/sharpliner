using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class IfStringCondition : IfCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, string[] one, string two)
    {
        _keyword = keyword;
        _one = Join(one);
        _two = two;
    }

    protected IfStringCondition(string keyword, string one, string[] two)
    {
        _keyword = keyword;
        _one = one;
        _two = Join(two);
    }

    protected IfStringCondition(string keyword, string one, string two)
    {
        _keyword = keyword;
        _one = one;
        _two = two;
    }

    protected static string Serialize(StaticVariableReference staticVariableReference)
    {
        // If condition is compile time - So we can't use runtime variables
        return staticVariableReference.RuntimeExpression;
    }

    protected static string Serialize(ParameterReference parameterReference)
    {
        return WrapQuotes(parameterReference.RuntimeExpression);
    }

    protected static string Serialize(IEnumerable<object> array)
    {
        var convertedStringArray = array.Select(item =>
            {
                return item switch
                {
                    StaticVariableReference staticVariableReference => Serialize(staticVariableReference),
                    ParameterReference parameterReference => Serialize(parameterReference),
                    VariableReference => throw new ArgumentException("If Conditions are compile-time statements, therefore runtime variables cannot be evaluated. You can use static variables or parameters instead."),
                    _ => item.ToString()
                };
            })
            .Select(WrapQuotes);

        return string.Join(", ", convertedStringArray);
    }

    internal override string Serialize() => WrapBraces($"{_keyword}({_one}, {_two})");
}

public abstract class IfStringCondition<T> : IfStringCondition
{
    protected IfStringCondition(string keyword, string one, string two, Conditioned<T>? parent = null)
        : base(keyword, one, two)
    {
        Parent = parent;
    }

    protected IfStringCondition(string keyword, string[] one, string two, Conditioned<T>? parent = null)
        : this(keyword, Join(one), two, parent)
    {
        Parent = parent;
    }

    protected IfStringCondition(string keyword, string one, string[] two, Conditioned<T>? parent = null)
        : this(keyword, one, Join(two), parent)
    {
        Parent = parent;
    }
}
