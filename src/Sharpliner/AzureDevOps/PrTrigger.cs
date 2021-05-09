using System.ComponentModel;
using System.Linq;

namespace Sharpliner.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#pr-trigger
    public record PrTrigger
    {
        /// <summary>
        /// Indicates whether additional pushes to a PR should cancel in-progress runs for the same PR.
        /// Defaults to true.
        /// </summary>
        [DefaultValue(true)]
        public bool AutoCancel { get; init; } = true;

        /// <summary>
        /// For GitHub only, whether to build draft PRs.
        /// Defaults to true.
        /// </summary>
        [DefaultValue(true)]
        public bool Drafts { get; init; } = true;

        public InclusionRule? Branches { get; init; } = null;

        public InclusionRule? Paths { get; init; } = null;

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
}
