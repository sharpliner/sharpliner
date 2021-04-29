using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    public record Stage
    {
        public string Name { get; }

        public string DisplayName { get; }

        public List<string> DependsOn { get; init; } = new();

        public string? Condition { get; init; }

        public List<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        public List<Job> Jobs { get; init; } = new();

        public Stage(string name, string displayName)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new System.ArgumentNullException(nameof(displayName));
        }
    }
}
