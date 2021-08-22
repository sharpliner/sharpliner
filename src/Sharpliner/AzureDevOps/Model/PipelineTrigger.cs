using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sharpliner.AzureDevOps
{
    public record PipelineTrigger
    {
        /// <summary>
        /// Batch changes if true; start a new build for every push if false (default)
        /// </summary>
        public bool Batch { get; init; } = false;

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

        public PipelineTrigger()
        {
        }

        public PipelineTrigger(params string[] branches)
        {
            Branches = new InclusionRule
            {
                Include = branches.ToList()
            };
        }
    }
}
