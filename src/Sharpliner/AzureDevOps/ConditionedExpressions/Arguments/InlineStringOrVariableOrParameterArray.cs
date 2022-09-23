﻿using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

[GenerateOneOf]
public partial class InlineStringOrVariableOrParameterArray : OneOfBase<string[], object[], ParameterReference[], VariableReference[]>
{
}
