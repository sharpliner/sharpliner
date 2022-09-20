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

internal class IfStringConditionHelper
{
    public static string Serialize(IfConditionOneOfStringValue value)
    {
        return value.Match(

            str => str,
            parameter => parameter.RuntimeExpression,
            staticVariable => staticVariable.RuntimeExpression
        );
    }

    public static string Serialize(IfConditionOneOfArrayStringValue arrayValue)
    {
        return arrayValue.Match(
            strings => string.Join(", ", strings),
            objects => Serialize(objects),
            parameters => string.Join(", ", parameters.Select(p => Serialize(p))),
            staticVariables => string.Join(", ", staticVariables.Select(v => Serialize(v)))
        );
    }

    public static string Serialize(object[] array)
    {
        var convertedStringArray = array.Select(item =>
            {
                return item switch
                {
                    IfConditionOneOfStringValue oneOfStringValue => Serialize(oneOfStringValue),
                    IfConditionOneOfArrayStringValue oneOfArrayStringValue => Serialize(oneOfArrayStringValue),
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
