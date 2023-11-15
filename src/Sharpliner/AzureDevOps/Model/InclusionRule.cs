using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public record InclusionRule
{
    /// <summary>
    /// Branch names which will trigger a build
    /// </summary>
    public ConditionedList<string> Include { get; init; } = [];

    public ConditionedList<string> Exclude { get; init; } = [];
}
