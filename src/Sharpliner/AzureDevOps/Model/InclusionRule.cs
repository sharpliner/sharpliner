using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Lists of items to include or exclude for trigger events. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/include-exclude-filters">includeExcludeFilters</see>.
/// </summary>
public record InclusionRule
{
    /// <summary>
    /// List of items to include.
    /// </summary>
    public AdoExpressionList<string> Include { get; init; } = [];

    /// <summary>
    /// List of items to exclude.
    /// </summary>
    public AdoExpressionList<string> Exclude { get; init; } = [];
}
