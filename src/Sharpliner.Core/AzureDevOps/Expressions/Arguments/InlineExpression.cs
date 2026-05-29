using System;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents a value that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
public union InlineExpression(string, ParameterReference, VariableReference)
{
}
