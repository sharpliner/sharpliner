using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

[GenerateOneOf]
public partial class IfStringOrVariableOrParameterArray : OneOfBase<string[], object[], ParameterReference[], StaticVariableReference[]>
{
}
