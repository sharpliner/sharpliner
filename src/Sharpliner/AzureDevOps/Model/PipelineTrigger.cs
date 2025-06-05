using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Configure pipeline resource triggers using the full syntax.
/// </summary>
public record PipelineTrigger
{
    /// <summary>
    /// Batch changes if true; start a new build for every push if false (default)
    /// </summary>
    public AdoExpression<bool>? Batch { get; init; }

    /// <summary>
    /// Branch conditions to filter the events
    /// Optional, defaults to all branches
    /// </summary>
    [DisallowNull]
    public InclusionRule? Branches { get; init; }

    /// <summary>
    /// List of tags to evaluate for trigger event
    /// Optional, 2020.1 and greater
    /// </summary>
    [DisallowNull]
    public InclusionRule? Tags { get; init; }

    /// <summary>
    /// List of stages to evaluate for trigger event
    /// Optional, 2020.1 and greater
    /// </summary>
    [DisallowNull]
    public InclusionRule? Stages { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineTrigger"/> class.
    /// </summary>
    public PipelineTrigger()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineTrigger"/> class with the specified branches.
    /// </summary>
    /// <param name="branches">The branches to trigger the pipeline on.</param>
    public PipelineTrigger(params string[] branches)
    {
        Branches = new InclusionRule
        {
            Include = branches.ToList()
        };
    }
}
