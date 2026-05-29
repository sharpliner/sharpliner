using System;
using System.Linq;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
public union InlineArrayExpression(InlineExpression[], ParameterReference[], VariableReference[], string[], object[])
{
    public string Serialize()
    {
        return this switch
        {
            string[] strings => string.Join(", ", strings),
            ParameterReference[] parameters => string.Join(", ", parameters.Select(p => InlineExpression.Serialize(p))),
            VariableReference[] variables => string.Join(", ", variables.Select(v => InlineExpression.Serialize(v))),
            InlineExpression[] inlineExpressions => Serialize([.. inlineExpressions.Cast<object>()]),
            object[] objects => Serialize(objects),
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(InlineArrayExpression)}")
        };
    }

    private static string Serialize(object[] array)
    {
        var convertedStringArray = array.Select(item =>
        {
            return item switch
            {
                InlineExpression inlineExpression => inlineExpression.Serialize(),
                InlineArrayExpression inlineArrayExpression => inlineArrayExpression.Serialize(),
                ParameterReference parameterReference => InlineExpression.Serialize(parameterReference),
                VariableReference variableReference => InlineExpression.Serialize(variableReference),
                _ => item.ToString()
            };
        })
            .Select(value => Condition.WrapQuotes(value ?? string.Empty));

        return string.Join(", ", convertedStringArray);
    }
}
