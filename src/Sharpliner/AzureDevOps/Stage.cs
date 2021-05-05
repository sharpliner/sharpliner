using System.Collections.Generic;
using Sharpliner.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public record Stage
    {
        [YamlMember(Alias = "stage", Order = 1)]
        public string Name { get; }

        [YamlMember(Order = 2)]
        public string DisplayName { get; }

        [YamlMember(Order = 100)]
        public List<string> DependsOn { get; init; } = new();

        [YamlMember(Order = 200)]
        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        [YamlMember(Order = 300)]
        public ConditionedDefinitionList<ConditionedDefinition<Job>> Jobs { get; init; } = new();

        [YamlMember(Order = 400)]
        public string? Condition { get; init; }

        public Stage(string name, string displayName)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new System.ArgumentNullException(nameof(displayName));
        }

        public static implicit operator ConditionedDefinition<Stage>(Stage stage) => new(stage);
    }
}
