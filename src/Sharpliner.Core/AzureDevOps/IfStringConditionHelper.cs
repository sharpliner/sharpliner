using System;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Expressions.Arguments;

namespace Sharpliner.AzureDevOps;

internal static class IfStringConditionHelper
{
    public static string Serialize(IfExpression ifExpression)
    {
        return ifExpression switch
        {
            string s => s,
            ParameterReference parameter => parameter.RuntimeExpression,
            VariableReference variable => variable.RuntimeExpression,
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(IfExpression)}")
        };
    }

    public static string Serialize(IfArrayExpression arrayValue)
    {
        return arrayValue switch
        {
            ParameterReference[] parameters => string.Join(", ", parameters.Select(p => Serialize(p))),
            VariableReference[] variables => string.Join(", ", variables.Select(v => Serialize(v))),
            IfExpression[] ifExpressions => string.Join(", ", Serialize([..ifExpressions.Cast<object>()])),
            string[] strings => string.Join(", ", strings),
            object[] objects => Serialize(objects),
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(IfArrayExpression)}")
        };
    }

    public static string Serialize(object[] array)
    {
        var convertedStringArray = array
            .Select(item => item switch
            {
                IfExpression ifExpression => Serialize(ifExpression),
                IfArrayExpression ifArrayExpression => Serialize(ifArrayExpression),
                VariableReference variableReference => Serialize(variableReference),
                ParameterReference parameterReference => Serialize(parameterReference),
                _ => item.ToString()
            })
            .Select(value => Condition.WrapQuotes(value ?? string.Empty));

        return string.Join(", ", convertedStringArray);
    }
}
