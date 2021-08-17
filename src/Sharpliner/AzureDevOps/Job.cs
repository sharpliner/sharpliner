using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#job
    /// </summary>
    public record Job : IDependsOn
    {
        // TODO: missing properties Uses, Services

        [YamlMember(Alias = "job", Order = 1, DefaultValuesHandling = DefaultValuesHandling.Preserve)]
        public string Name { get; }

        [YamlMember(Order = 100)]
        [DisallowNull]
        public string? DisplayName { get; init; }

        [YamlMember(Order = 200)]
        public List<string> DependsOn { get; init; } = new();

        [YamlMember(Order = 300)]
        public ConditionedDefinition<Pool>? Pool { get; init; }

        [YamlMember(Order = 400)]
        [DisallowNull]
        public Strategy? Strategy { get; init; }

        [YamlMember(Order = 500)]
        [DisallowNull]
        public ContainerReference? Container { get; init; }

        [YamlMember(Order = 600)]
        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        [YamlMember(Order = 700)]
        public ConditionedDefinitionList<ConditionedDefinition<Step>> Steps { get; init; } = new();

        [YamlMember(Order = 800)]
        [DisallowNull]
        public TimeSpan? Timeout { get; init; }

        [YamlMember(Order = 900)]
        [DisallowNull]
        public TimeSpan? CancelTimeout { get; init; }

        [YamlMember(Order = 1000)]
        [DefaultValue(JobWorkspace.Outputs)]
        public JobWorkspace Workspace { get; init; } = JobWorkspace.Outputs;

        [YamlMember(Order = 1100)]
        [DisallowNull]
        public string? Condition { get; init; }

        [YamlMember(Order = 1200)]
        public bool ContinueOnError { get; init; } = false;

        public Job(string name, string? displayName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (displayName != null)
            {
                DisplayName = displayName;
            }
        }

        public static implicit operator ConditionedDefinition<Job>(Job definition) => new(definition);
    }
}
