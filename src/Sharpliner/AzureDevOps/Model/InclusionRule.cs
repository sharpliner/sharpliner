﻿using System.Collections.Generic;

namespace Sharpliner.AzureDevOps
{
    public record InclusionRule
    {
        /// <summary>
        /// Branch names which will trigger a build
        /// </summary>
        public ConditionedDefinitionList<string> Include { get; init; } = new();

        public ConditionedDefinitionList<string> Exclude { get; init; } = new();
    }
}
