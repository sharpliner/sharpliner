using System;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Expressions.Arguments;

namespace Sharpliner.AzureDevOps;

internal static class InlineStringConditionHelper
{
    public static string Serialize(InlineExpression inlineExpression)
    {
        return inlineExpression switch
        {

            string s => s,
            ParameterReference parameter => parameter.CompileTimeExpression,
            VariableReference variable => variable.RuntimeExpression,
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(InlineExpression)}")
        };
    }

    public static string Serialize(InlineArrayExpression inlineArray)
    {
        return inlineArray switch
        {
            string[] strings => string.Join(", ", strings),
            ParameterReference[] parameters => string.Join(", ", parameters.Select(p => Serialize(p))),
            VariableReference[] variables => string.Join(", ", variables.Select(v => Serialize(v))),
            object[] objects => Serialize(objects),
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(InlineArrayExpression)}")
        };
    }

    public static string Serialize(object[] array)
    {
        var convertedStringArray = array.Select(item =>
            {
                return item switch
                {
                    InlineExpression inlineExpression => Serialize(inlineExpression),
                    InlineArrayExpression inlineArrayExpression => Serialize(inlineArrayExpression),
                    ParameterReference parameterReference => Serialize(parameterReference),
                    VariableReference variableReference => Serialize(variableReference),
                    _ => item.ToString()
                };
            })
            .Select(value => Condition.WrapQuotes(value ?? string.Empty));

        return string.Join(", ", convertedStringArray);
    }
}
