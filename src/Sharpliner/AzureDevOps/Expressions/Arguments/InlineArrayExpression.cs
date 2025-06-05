using OneOf;

namespace Sharpliner.AzureDevOps.Expressions.Arguments;

/// <summary>
/// Represents an array of values that can be used in an inline condition.
/// See methods in <see cref="AzureDevOpsDefinition"/> for usages.
/// </summary>
[GenerateOneOf]
public partial class InlineArrayExpression : OneOfBase<string[], object[], ParameterReference[], VariableReference[]>
{
}
