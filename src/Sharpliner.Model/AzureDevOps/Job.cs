using System;
using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{

    // TODO: missing properties Uses, Services
    public record Job
    {
        public string Name { get; }

        public string DisplayName { get; }

        public List<string> DependsOn { get; init; } = new();

        public string? Condition { get; init; }

        public JobStrategy? Strategy { get; init; }

        public Pool? Pool { get; init; }

        public ContainerReference? Container { get; init; }

        public TimeSpan? Timeout { get; init; }

        public TimeSpan? CancelTimeout { get; init; }

        public List<ConditionedDefinition<VariableBase>> Variables { get; init; } = new();

        public List<Step> Steps { get; init; } = new();

        public JobWorkspace Workspace { get; init; } = JobWorkspace.Outputs;

        public bool ContinueOnError { get; init; } = false;

        public Job(string name, string displayName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }
    }
}
