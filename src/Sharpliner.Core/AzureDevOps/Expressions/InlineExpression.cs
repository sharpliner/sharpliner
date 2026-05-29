using System;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// Represents a value that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
public union InlineExpression(string, ParameterReference, VariableReference)
{
    internal string Serialize() => Serialize(this);

    internal static string Serialize(InlineExpression inlineExpression)
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
