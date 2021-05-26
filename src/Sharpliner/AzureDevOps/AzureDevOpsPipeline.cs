using System.Diagnostics.CodeAnalysis;
using Sharpliner.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public abstract record AzureDevOpsPipelineBase
    {
        [YamlMember(Order = 100)]
        [DisallowNull]
        public string? Name { get; init; }

        [YamlMember(Order = 200)]
        [DisallowNull]
        public Trigger? Trigger { get; init; }

        [YamlMember(Order = 300)]
        [DisallowNull]
        public PrTrigger? Pr { get; init; }

        [YamlMember(Order = 400)]
        [DisallowNull]
        public Resources? Resources { get; init; }

        [YamlMember(Order = 500)]
        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; } = new();

        // TODO: Scheduled triggers
    }

    public record AzureDevOpsPipeline : AzureDevOpsPipelineBase
    {
        [YamlMember(Order = 600)]
        public ConditionedDefinitionList<ConditionedDefinition<Stage>> Stages { get; init; } = new();
    }

    public record SingleStageAzureDevOpsPipeline : AzureDevOpsPipelineBase
    {
        [YamlMember(Order = 600)]
        public ConditionedDefinitionList<ConditionedDefinition<Job>> Jobs { get; init; } = new();
    }
}
