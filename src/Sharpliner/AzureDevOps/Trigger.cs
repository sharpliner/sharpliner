using System.Linq;

namespace Sharpliner.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#triggers
    public record Trigger
    {
        /// <summary>
        /// Batch changes if true; start a new build for every push if false (default).
        /// </summary>
        public bool Batch { get; init; } = false;

        public InclusionRule? Branches { get; init; } = null;

        public InclusionRule? Tags { get; init; } = null;

        public InclusionRule? Paths { get; init; } = null;

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
