using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#pr-trigger">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record PrTrigger
{
    /// <summary>
    /// Indicates whether additional pushes to a PR should cancel in-progress runs for the same PR
    /// Defaults to true
    /// </summary>
    [DefaultValue(true)]
    public bool AutoCancel { get; init; } = true;

    /// <summary>
    /// For GitHub only, whether to build draft PRs
    /// Defaults to true
    /// </summary>
    [DefaultValue(true)]
    public bool Drafts { get; init; } = true;

    [DisallowNull]
    public InclusionRule? Branches { get; init; }

    [DisallowNull]
    public InclusionRule? Paths { get; init; }

    public PrTrigger()
    {
    }

    public PrTrigger(params string[] branches)
    {
        Branches = new InclusionRule
        {
            Include = branches.ToList()
        };
    }
}
