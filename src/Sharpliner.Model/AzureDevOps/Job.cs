using System;
using System.Collections.Generic;
using Sharpliner.Model.ConditionedDefinitions;

namespace Sharpliner.Model.AzureDevOps
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#job
    /// </summary>
    public record Job
    {
        // TODO: missing properties Uses, Services

        public string Name { get; }

        public string DisplayName { get; }

        public List<string> DependsOn { get; init; } = new();

        public string? Condition { get; init; }

        public JobStrategy? Strategy { get; init; }

        public ConditionedDefinition<Pool>? Pool { get; init; }

        public ContainerReference? Container { get; init; }

        public TimeSpan? Timeout { get; init; }

        public TimeSpan? CancelTimeout { get; init; }

        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        public ConditionedDefinitionList<ConditionedDefinition<Step>> Steps { get; init; } = new();

        public JobWorkspace Workspace { get; init; } = JobWorkspace.Outputs;

        public bool ContinueOnError { get; init; } = false;

        public Job(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        public static implicit operator ConditionedDefinition<Job>(Job definition) => new(definition);
    }
}
