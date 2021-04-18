using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    public record Pipeline
    {
        public string? Name { get; init; }

        public Resources? Resources { get; init; }

        public List<VariableDefinition> Variables { get; init; } = new();

        public List<Trigger> Triggers { get; init; } = new();

        public List<Stage> Stages { get; init; } = new();
    }
}
