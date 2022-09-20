using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public class StaticVariablesReference : VariablesReference
{
    public new StaticVariableReference this[string variableName] => new(variableName);
}
