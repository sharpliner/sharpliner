using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

/// <summary>
/// Represents a value that can be used in an if expression.
/// </summary>
[GenerateOneOf]
public partial class IfExpression : OneOfBase<string, ParameterReference, VariableReference>
{
}
