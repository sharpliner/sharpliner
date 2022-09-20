using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

[GenerateOneOf]
public partial class IfConditionOneOfStringValue : OneOfBase<string, ParameterReference, StaticVariableReference>
{
}
