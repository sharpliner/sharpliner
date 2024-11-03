using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
[GenerateOneOf]
public partial class IfArrayExpression : OneOfBase<string[], object[], ParameterReference[], VariableReference[]>
{
}
