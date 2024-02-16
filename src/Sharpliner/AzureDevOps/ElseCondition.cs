using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public class ElseCondition : Condition
{
    internal override string Serialize() => ElseTagStart + TagEnd;
}
