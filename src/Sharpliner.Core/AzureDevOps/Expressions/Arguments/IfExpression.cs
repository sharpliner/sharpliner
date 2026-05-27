using OneOf;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents a value that can be used in an if condition.
/// See <see cref="IfConditionBuilder"/> for usages.
/// </summary>
[GenerateOneOf]
public partial class IfExpression : OneOfBase<string, ParameterReference, VariableReference>
{
}
