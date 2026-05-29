using System;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// Represents a value that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
public union IfExpression(string, ParameterReference, VariableReference)
{
    internal string Serialize() => Serialize(this);

    internal static string Serialize(IfExpression expression)
    {
        return expression switch
        {
            string s => s,
            ParameterReference parameter => parameter.RuntimeExpression,
            VariableReference variable => variable.RuntimeExpression,
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(IfExpression)}")
        };
    }
}
