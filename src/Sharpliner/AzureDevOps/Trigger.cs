using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#triggers
    /// </summary>
    public record Trigger
    {
        /// <summary>
        /// Batch changes if true; start a new build for every push if false (default).
        /// </summary>
        public bool Batch { get; init; } = false;

        [DisallowNull]
        public InclusionRule? Branches { get; init; }

        [DisallowNull]
        public InclusionRule? Tags { get; init; }

        [DisallowNull]
        public InclusionRule? Paths { get; init; }

        public Trigger()
        {
        }

        public Trigger(params string[] branches)
        {
            Branches = new InclusionRule
            {
                Include = branches.ToList()
            };
        }
    }
}
