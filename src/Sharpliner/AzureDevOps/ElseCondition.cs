using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Represents an <c>${{ else }}</c> expression in the pipeline.
/// </summary>
public class ElseCondition : Condition
{
    internal override string Serialize() => ElseTagStart + TagEnd;
}
