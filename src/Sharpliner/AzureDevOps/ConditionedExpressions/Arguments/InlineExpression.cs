using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

/// <summary>
/// Represents a value that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
[GenerateOneOf]
public partial class InlineExpression : OneOfBase<string, ParameterReference, VariableReference>
{
}
