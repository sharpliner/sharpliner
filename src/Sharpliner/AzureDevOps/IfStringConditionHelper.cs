using System;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

internal static class IfStringConditionHelper
{
    public static string Serialize(IfExpression stringOrVariableOrParameter)
    {
        return stringOrVariableOrParameter.Match(

            str => str,
            parameter => parameter.RuntimeExpression,
            staticVariable => staticVariable.RuntimeExpression
        );
    }

    public static string Serialize(IfArrayExpression arrayValue)
    {
        return arrayValue.Match(
            strings => string.Join(", ", strings),
            Serialize,
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
                    IfExpression oneOfStringValue => Serialize(oneOfStringValue),
                    IfArrayExpression oneOfArrayStringValue => Serialize(oneOfArrayStringValue),
                    StaticVariableReference staticVariableReference => Serialize(staticVariableReference),
                    ParameterReference parameterReference => Serialize(parameterReference),
                    VariableReference => throw new ArgumentException("${{ if }} conditions are compile-time statements, therefore runtime variables cannot be evaluated. You can use variables or parameters instead"),
                    _ => item.ToString()
                };
            })
            .Select(value => Condition.WrapQuotes(value ?? string.Empty));

        return string.Join(", ", convertedStringArray);
    }
}
