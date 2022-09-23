using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

[GenerateOneOf]
public partial class IfStringOrVariableOrParameter : OneOfBase<string, ParameterReference, StaticVariableReference>
{
}
