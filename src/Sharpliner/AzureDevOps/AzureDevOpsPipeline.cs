using Sharpliner.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public abstract record AzureDevOpsPipelineBase
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
