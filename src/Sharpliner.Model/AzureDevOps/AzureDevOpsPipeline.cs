using System.Collections.Generic;
using Sharpliner.Model.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps
{
    public record AzureDevOpsPipeline
    {
        [YamlMember(Order = 100)]
        public string? Name { get; init; }

        [YamlMember(Order = 200)]
        public Trigger? Trigger { get; init; } = null;

        [YamlMember(Order = 300)]
        public PrTrigger? Pr { get; init; } = null;

        [YamlMember(Order = 400)]
        public Resources? Resources { get; init; }

        [YamlMember(Order = 500)]
        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; } = new();

        [YamlMember(Order = 600)]
        public List<Stage> Stages { get; init; } = new();

        // TODO: Scheduled triggers
    }
}
