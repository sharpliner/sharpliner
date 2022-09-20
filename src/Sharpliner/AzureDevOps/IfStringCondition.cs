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


    protected IfStringCondition(string keyword, IEnumerable<object> one, string two)
        : this(keyword, Serialize(one), two)
    {
    }

    protected IfStringCondition(string keyword, string one, IEnumerable<object> two)
        : this(keyword, one, Serialize(two))
    {
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
        return IfStringConditionHelper.Serialize(staticVariableReference);
    }

    protected static string Serialize(ParameterReference parameterReference)
    {
        return IfStringConditionHelper.Serialize(parameterReference);
    }

    protected static string Serialize(IEnumerable<object> array)
    {
        return IfStringConditionHelper.Serialize(array);
    }

    protected static string Serialize(string value)
    {
        return value;
    }

    protected static string[] Serialize(string[] value)
    {
        return value;
    }

    internal override string Serialize() => WrapBraces($"{_keyword}({_one}, {_two})");
}

public abstract class IfStringCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, string one, string two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = one;
        _two = two;
        Parent = parent;
    }

    protected IfStringCondition(string keyword, IEnumerable<object> one, string two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
        Parent = parent;
    }

    protected IfStringCondition(string keyword, string one, IEnumerable<object> two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
        Parent = parent;
    }

    protected IfStringCondition(string keyword, string[] one, string two, Conditioned<T>? parent = null)
        : this(keyword, Join(one), two, parent)
    {
        _keyword = keyword;
        _one = Join(one);
        _two = two;
        Parent = parent;
    }

    protected IfStringCondition(string keyword, string one, string[] two, Conditioned<T>? parent = null)
        : this(keyword, one, Join(two), parent)
    {
        _keyword = keyword;
        _one = one;
        _two = Join(two);
        Parent = parent;
    }

    protected static string Serialize(StaticVariableReference staticVariableReference)
    {
        // If condition is compile time - So we can't use runtime variables
        return IfStringConditionHelper.Serialize(staticVariableReference);
    }

    protected static string Serialize(ParameterReference parameterReference)
    {
        return IfStringConditionHelper.Serialize(parameterReference);
    }

    protected static string Serialize(IEnumerable<object> array)
    {
        return IfStringConditionHelper.Serialize(array);
    }

    protected static string Serialize(string value)
    {
        return value;
    }

    protected static string[] Serialize(string[] value)
    {
        return value;
    }

    internal override string Serialize() => WrapBraces($"{_keyword}({_one}, {_two})");
}

internal class IfStringConditionHelper
{
    public static string Serialize(StaticVariableReference staticVariableReference)
    {
        // If condition is compile time - So we can't use runtime variables
        return staticVariableReference.RuntimeExpression;
    }

    public static string Serialize(ParameterReference parameterReference)
    {
        return Condition.WrapQuotes(parameterReference.RuntimeExpression);
    }

    public static string Serialize(IEnumerable<object> array)
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
            .Select(Condition.WrapQuotes);

        return string.Join(", ", convertedStringArray);
    }
}
