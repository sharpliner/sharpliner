namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
public union InlineArrayExpression(InlineExpression[], ParameterReference[], VariableReference[], string[], object[]);
