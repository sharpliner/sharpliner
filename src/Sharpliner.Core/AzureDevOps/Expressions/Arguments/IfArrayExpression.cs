namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
public union IfArrayExpression(string[], object[], ParameterReference[], VariableReference[])
{
}