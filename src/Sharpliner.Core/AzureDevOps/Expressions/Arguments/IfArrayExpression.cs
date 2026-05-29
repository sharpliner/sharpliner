using System;
using System.Linq;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
public union IfArrayExpression(IfExpression[], ParameterReference[], VariableReference[], string[], object[])
{
    public string Serialize()
    {
        return this switch
        {
            ParameterReference[] parameters => string.Join(", ", parameters.Select(p => IfExpression.Serialize(p))),
            VariableReference[] variables => string.Join(", ", variables.Select(v => IfExpression.Serialize(v))),
            IfExpression[] ifExpressions => string.Join(", ", Serialize([.. ifExpressions.Cast<object>()])),
            string[] strings => string.Join(", ", strings),
            object[] objects => Serialize(objects),
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(IfArrayExpression)}")
        };
    }

    private static string Serialize(object[] array)
    {
        var convertedStringArray = array
            .Select(item => item switch
            {
                IfExpression ifExpression => ifExpression.Serialize(),
                IfArrayExpression ifArrayExpression => ifArrayExpression.Serialize(),
                VariableReference variableReference => IfExpression.Serialize(variableReference),
                ParameterReference parameterReference => IfExpression.Serialize(parameterReference),
                _ => item.ToString()
            })
            .Select(value => Condition.WrapQuotes(value ?? string.Empty));

        return string.Join(", ", convertedStringArray);
    }
}
