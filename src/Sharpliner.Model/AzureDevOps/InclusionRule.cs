using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    public record InclusionRule
    {
        /// <summary>
        /// Branch names which will trigger a build
        /// </summary>
        public List<string> Include { get; init; } = new();

        public List<string> Exclude { get; init; } = new();
    }
}
