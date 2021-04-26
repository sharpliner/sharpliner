namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#pr-trigger
    public abstract record PrTrigger;

    public record NoPrTrigger : PrTrigger;

    public record BranchPrTrigger(params string[] Branches) : PrTrigger;

    public record DetailedPrTrigger : PrTrigger
    {
        /// <summary>
        /// Indicates whether additional pushes to a PR should cancel in-progress runs for the same PR.
        /// Defaults to true.
        /// </summary>
        public bool AutoCancel { get; init; } = true;

        /// <summary>
        /// For GitHub only, whether to build draft PRs.
        /// Defaults to true.
        /// </summary>
        public bool Drafts { get; init; } = true;

        public InclusionRule? Branches { get; init; } = null;

        public InclusionRule? Paths { get; init; } = null;
    }
}
