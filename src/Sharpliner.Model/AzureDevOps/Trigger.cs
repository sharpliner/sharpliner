namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#triggers
    public abstract record Trigger;

    public record NoTrigger : Trigger;

    public record BranchTrigger(params string[] BranchNames) : Trigger;

    public record DetailedTrigger : Trigger
    {
        /// <summary>
        /// Batch changes if true; start a new build for every push if false (default).
        /// </summary>
        public bool Batched { get; init; } = false;

        public InclusionRule? Branches { get; init; } = null;

        public InclusionRule? Tags { get; init; } = null;

        public InclusionRule? Paths { get; init; } = null;
    }
}
