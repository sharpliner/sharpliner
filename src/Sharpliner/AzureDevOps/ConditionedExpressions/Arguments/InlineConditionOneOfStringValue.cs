﻿using OneOf;

namespace Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

[GenerateOneOf]
public partial class InlineConditionOneOfStringValue : OneOfBase<string, ParameterReference, VariableReference>
{
}