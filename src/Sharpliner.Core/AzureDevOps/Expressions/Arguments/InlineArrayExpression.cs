using System;
using System.Linq;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
public union InlineArrayExpression(InlineExpression[], ParameterReference[], VariableReference[], string[])
{
    internal string Serialize()
    {
        return this switch
        {
            string[] strings => string.Join(", ", strings),
            ParameterReference[] parameters => string.Join(", ", parameters.Select(p => InlineExpression.Serialize(p))),
            VariableReference[] variables => string.Join(", ", variables.Select(v => InlineExpression.Serialize(v))),
            InlineExpression[] inlineExpressions => string.Join(", ", inlineExpressions
                .Select(e => e.Serialize())
                .Select(value => Condition.WrapQuotes(value ?? string.Empty))),
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(InlineArrayExpression)}")
        };
    }
}
