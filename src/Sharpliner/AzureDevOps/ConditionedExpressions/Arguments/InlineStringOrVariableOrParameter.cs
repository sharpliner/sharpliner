using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

[GenerateOneOf]
public partial class InlineStringOrVariableOrParameter : OneOfBase<string, ParameterReference, VariableReference>
{
}
