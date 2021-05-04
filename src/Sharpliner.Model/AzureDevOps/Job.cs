﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sharpliner.Model.ConditionedDefinitions;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#job
    /// </summary>
    public record Job
    {
        // TODO: missing properties Uses, Services

        [YamlMember(Alias = "job", Order = 1)]
        public string Name { get; }

        [YamlMember(Order = 100)]
        public string DisplayName { get; }

        [YamlMember(Order = 200)]
        public List<string> DependsOn { get; init; } = new();

        [YamlMember(Order = 300)]
        public ConditionedDefinition<Pool>? Pool { get; init; }

        [YamlMember(Order = 400)]
        public JobStrategy? Strategy { get; init; }

        [YamlMember(Order = 500)]
        public ContainerReference? Container { get; init; }

        [YamlMember(Order = 600)]
        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        [YamlMember(Order = 700)]
        public ConditionedDefinitionList<ConditionedDefinition<Step>> Steps { get; init; } = new();

        [YamlMember(Order = 800)]
        public TimeSpan? Timeout { get; init; }

        [YamlMember(Order = 900)]
        public TimeSpan? CancelTimeout { get; init; }

        [YamlMember(Order = 1000)]
        [DefaultValue(JobWorkspace.Outputs)]
        public JobWorkspace Workspace { get; init; } = JobWorkspace.Outputs;

        [YamlMember(Order = 1100)]
        public string? Condition { get; init; }

        [YamlMember(Order = 1200)]
        public bool ContinueOnError { get; init; } = false;

        public Job(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public static implicit operator ConditionedDefinition<Job>(Job definition) => new(definition);
    }
}
