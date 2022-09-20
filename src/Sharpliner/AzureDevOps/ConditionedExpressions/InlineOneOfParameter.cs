using System.Collections.Generic;
using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

[GenerateOneOf]
public partial class InlineConditionOneOfStringValue : OneOfBase<string, ParameterReference, VariableReference>
{
}

[GenerateOneOf]
public partial class InlineConditionOneOfArrayStringValue : OneOfBase<string[], object[], ParameterReference[], VariableReference[]>
{
}

[GenerateOneOf]
public partial class IfConditionOneOfStringValue : OneOfBase<string, ParameterReference, StaticVariableReference>
{
}

[GenerateOneOf]
public partial class IfConditionOneOfArrayStringValue : OneOfBase<string[], object[], ParameterReference[], StaticVariableReference[]>
{
}
