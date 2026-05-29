using System;
using System.Linq;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// Represents an array of values that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
public union IfArrayExpression(IfExpression[], ParameterReference[], VariableReference[], string[])
{
    internal string Serialize()
    {
        return this switch
        {
            ParameterReference[] parameters => string.Join(", ", parameters.Select(p => IfExpression.Serialize(p))),
            VariableReference[] variables => string.Join(", ", variables.Select(v => IfExpression.Serialize(v))),
            IfExpression[] ifExpressions => string.Join(", ", ifExpressions
                .Select(e => e.Serialize())
                .Select(value => Condition.WrapQuotes(value ?? string.Empty))),
            string[] strings => string.Join(", ", strings),
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(IfArrayExpression)}")
        };
    }
}
