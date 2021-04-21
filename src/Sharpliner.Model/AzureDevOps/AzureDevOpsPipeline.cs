using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    public record AzureDevOpsPipeline
    {
        public string? Name { get; init; }

        public Resources? Resources { get; init; }

        public List<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        public Trigger? Trigger { get; init; } = null;

        public PrTrigger? Pr { get; init; } = null;

        // TODO: Scheduled triggers

        public List<Stage> Stages { get; init; } = new();
    }
}
