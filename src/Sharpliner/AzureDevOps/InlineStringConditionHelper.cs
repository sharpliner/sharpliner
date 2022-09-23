using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

internal static class InlineStringConditionHelper
{
    public static string Serialize(InlineStringOrVariableOrParameter stringOrVariableOrParameter)
    {
        return stringOrVariableOrParameter.Match(

            str => str,
            parameter => parameter.CompileTimeExpression,
            variable => variable.RuntimeExpression
        );
    }

    public static string Serialize(InlineStringOrVariableOrParameterArray arrayValue)
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
                    InlineStringOrVariableOrParameter oneOfStringValue => Serialize(oneOfStringValue),
                    InlineStringOrVariableOrParameterArray oneOfArrayStringValue => Serialize(oneOfArrayStringValue),
                    ParameterReference parameterReference => Serialize(parameterReference),
                    VariableReference variableReference => Serialize(variableReference),
                    _ => item.ToString()
                };
            })
            .Select(value => Condition.WrapQuotes(value ?? string.Empty));

        return string.Join(", ", convertedStringArray);
    }
}
