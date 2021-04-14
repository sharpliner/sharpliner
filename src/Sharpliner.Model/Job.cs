using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record Job
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public IEnumerable<string> DependsOn { get; init; }
        public string Condition { get; init; }
        public JobStrategy Strategy { get; init; }
        public JobPool Pool { get; init; }
        public ContainerReference Container { get; init; }
        public TimeSpan Timeout { get; init; }
        public TimeSpan CancelTimeout { get; init; }
        public Variables Variables  { get; init; }
        public IEnumerable<Step> Steps { get; init; }
        public JobWorkspace Workspace { get; init; } = JobWorkspace.Outputs;
        public bool ContinueOnError { get; init; } = false;
    }

    // TODO: missing properties Uses, Services

    public class Jobs : List<Job> { }
}
