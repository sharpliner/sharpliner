using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

/// <summary>
/// Represents a value that can be used in an inline expression.
/// </summary>
[GenerateOneOf]
public partial class InlineExpression : OneOfBase<string, ParameterReference, VariableReference>
{
}
