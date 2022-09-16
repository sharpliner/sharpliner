using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

[GenerateOneOf]
public partial class StringOrVariableOrParameter : OneOfBase<string, VariableReference, ParameterReference>
{
}
