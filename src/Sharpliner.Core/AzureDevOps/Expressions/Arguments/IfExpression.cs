using System;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents a value that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
public union IfExpression(string, ParameterReference, VariableReference)
{
    public string Serialize() => Serialize(this);

    public static string Serialize(IfExpression expression)
    {
        return expression switch
        {
            string s => s,
            ParameterReference parameter => parameter.CompileTimeExpression,
            VariableReference variable => variable.RuntimeExpression,
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(IfExpression)}")
        };
    }
}
