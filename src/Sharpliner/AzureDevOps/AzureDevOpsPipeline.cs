using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Sharpliner.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public abstract record AzureDevOpsPipelineBase
    {
        private static Regex s_nameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

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
            var allDefs = definitions.SelectMany(s => s.FlattenDefinitions());

            var duplicate = allDefs.FirstOrDefault(d => allDefs.Count(o => o.Name == d.Name) > 1);
            if (duplicate is not null)
            {
                throw new Exception($"Found duplicate {typeof(T).Name.ToLower()} name '{duplicate.Name}'");
            }

            var invalidName = allDefs.FirstOrDefault(d => !s_nameRegex.IsMatch(d.Name));
            if (invalidName is not null)
            {
                throw new Exception($"Invalid character found in {typeof(T).Name.ToLower()} name '{invalidName.Name}', only A-Z, a-z, 0-9, and underscore are allowed");
            }

            foreach (var definition in allDefs)
            {
                foreach (var dependsOn in definition.DependsOn)
                {
                    if (dependsOn == definition.Name)
                    {
                        throw new Exception($"{typeof(T).Name} `{definition.Name}` depends on itself");
                    }

                    if (!allDefs.Any(d => d.Name == dependsOn))
                    {
                        throw new Exception($"{typeof(T).Name} `{definition.Name}` depends on stage `{dependsOn}` which was not found");
                    }
                }
            }

            // TODO: Validate circular dependencies?
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
