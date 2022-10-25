using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public class StaticVariablesReference : VariablesReference
{
    public new VariableReference this[string variableName] => new(variableName);
}
