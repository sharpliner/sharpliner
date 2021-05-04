using System.Collections.Generic;
using Sharpliner.Model.ConditionedDefinitions;

namespace Sharpliner.Model.AzureDevOps
{
    public record Stage
    {
        public string Name { get; }

        public string DisplayName { get; }

        public List<string> DependsOn { get; init; } = new();

        public string? Condition { get; init; }

        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        public ConditionedDefinitionList<ConditionedDefinition<Job>> Jobs { get; init; } = new();

        public Stage(string name, string displayName)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new System.ArgumentNullException(nameof(displayName));
        }

        public static implicit operator ConditionedDefinition<Stage>(Stage stage) => new(stage);
    }
}
