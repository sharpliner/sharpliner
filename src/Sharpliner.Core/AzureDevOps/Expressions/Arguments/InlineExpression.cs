using System;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents a value that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
public union InlineExpression(string, ParameterReference, VariableReference)
{
    public string Serialize() => Serialize(this);

    public static string Serialize(InlineExpression inlineExpression)
    {
        return inlineExpression switch
        {
            ParameterReference parameter => parameter.CompileTimeExpression,
            VariableReference variable => variable.RuntimeExpression,
            string s => s,
            _ => throw new InvalidOperationException($"Unsupported type in {nameof(InlineExpression)}")
        };
    }
}
