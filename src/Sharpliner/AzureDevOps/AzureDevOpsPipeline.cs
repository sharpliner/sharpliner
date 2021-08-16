using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        public abstract void Validate();

        // TODO: Scheduled triggers

        protected static void ValidateDependsOn<T>(ConditionedDefinitionList<ConditionedDefinition<T>> definitions) where T : IDependsOn
        {
            // TODO: Validate circular dependencies?
            var allDefs = definitions.SelectMany(s => s.FlattenDefinitions());

            foreach (var definition in allDefs)
            {
                foreach (var dependsOn in definition.DependsOn)
                {
                    if (!allDefs.Any(s => s.Name == dependsOn))
                    {
                        throw new Exception($"Stage `{definition.Name}` depends on stage `{dependsOn}` which was not found");
                    }
                }
            }
        }
    }

    public record AzureDevOpsPipeline : AzureDevOpsPipelineBase
    {
        [YamlMember(Order = 600)]
        public ConditionedDefinitionList<ConditionedDefinition<Stage>> Stages { get; init; } = new();

        public override void Validate()
        {
            ValidateDependsOn(Stages);

            foreach (var stage in Stages.SelectMany(s => s.FlattenDefinitions()))
            {
                ValidateDependsOn(stage.Jobs);
            }
        }
    }

    public record SingleStageAzureDevOpsPipeline : AzureDevOpsPipelineBase
    {
        [YamlMember(Order = 600)]
        public ConditionedDefinitionList<ConditionedDefinition<Job>> Jobs { get; init; } = new();

        public override void Validate()
        {
            ValidateDependsOn(Jobs);
        }
    }
}
