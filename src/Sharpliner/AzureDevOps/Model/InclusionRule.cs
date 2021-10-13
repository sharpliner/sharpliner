namespace Sharpliner.AzureDevOps
{
    public record InclusionRule
    {
        /// <summary>
        /// Branch names which will trigger a build
        /// </summary>
        public ConditionedList<string> Include { get; init; } = new();

        public ConditionedList<string> Exclude { get; init; } = new();
    }
}
