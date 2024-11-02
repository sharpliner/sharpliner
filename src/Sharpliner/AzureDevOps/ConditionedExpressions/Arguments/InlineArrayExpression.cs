using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an inline expression.
/// </summary>
[GenerateOneOf]
public partial class InlineArrayExpression : OneOfBase<string[], object[], ParameterReference[], VariableReference[]>
{
}
