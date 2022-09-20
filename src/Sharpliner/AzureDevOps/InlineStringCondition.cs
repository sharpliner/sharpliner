using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class InlineStringCondition : InlineCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected InlineStringCondition(string keyword, InlineConditionOneOfArrayStringValue one, InlineConditionOneOfStringValue two)
        : this(keyword, Serialize(one), two)
    {
    }

    protected InlineStringCondition(string keyword, InlineConditionOneOfStringValue one, InlineConditionOneOfArrayStringValue two)
        : this(keyword, one, Serialize(two))
    {
    }

    protected InlineStringCondition(string keyword, InlineConditionOneOfStringValue one, InlineConditionOneOfStringValue two)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
    }

    protected static string Serialize(InlineConditionOneOfArrayStringValue arrayValue)
    {
        return InlineStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(InlineConditionOneOfStringValue value)
    {
        return InlineStringConditionHelper.Serialize(value);
    }

    internal override string Serialize() => $"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})";
}

public abstract class InlineStringCondition<T> : InlineCondition<T>
{
    private readonly string _keyword;
    private string _one;
    private string _two;

    protected InlineStringCondition(string keyword, InlineConditionOneOfArrayStringValue one, InlineConditionOneOfStringValue two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    protected InlineStringCondition(string keyword, InlineConditionOneOfStringValue one, InlineConditionOneOfArrayStringValue two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    protected InlineStringCondition(string keyword, InlineConditionOneOfStringValue one, InlineConditionOneOfStringValue two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    protected static string Serialize(InlineConditionOneOfArrayStringValue arrayValue)
    {
        return InlineStringConditionHelper.Serialize(arrayValue);
    }

    protected static string Serialize(InlineConditionOneOfStringValue value)
    {
        return InlineStringConditionHelper.Serialize(value);
    }

    internal override string Serialize() => $"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})";
}

internal class InlineStringConditionHelper
{
    public static string Serialize(InlineConditionOneOfStringValue value)
    {
        return value.Match(

            str => str,
            parameter => parameter.CompileTimeExpression,
            variable => variable.RuntimeExpression
        );
    }

    public static string Serialize(InlineConditionOneOfArrayStringValue arrayValue)
    {
        return arrayValue.Match(
            strings => string.Join(", ", strings),
            objects => Serialize(objects),
            parameters => string.Join(", ", parameters.Select(p => Serialize(p))),
            variables => string.Join(", ", variables.Select(v => Serialize(v)))
        );
    }

    public static string Serialize(object[] array)
    {
        var convertedStringArray = array.Select(item =>
            {
                return item switch
                {
                    InlineConditionOneOfStringValue oneOfStringValue => Serialize(oneOfStringValue),
                    InlineConditionOneOfArrayStringValue oneOfArrayStringValue => Serialize(oneOfArrayStringValue),
                    ParameterReference parameterReference => Serialize(parameterReference),
                    VariableReference variableReference => Serialize(variableReference),
                    _ => item.ToString()
                };
            })
            .Select(Condition.WrapQuotes);

        return string.Join(", ", convertedStringArray);
    }
}
