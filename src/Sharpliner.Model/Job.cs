using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record Job(
        string Name,
        string DisplayName,
        IEnumerable<string> DependsOn,
        string Condition,
        JobStrategy Strategy,
        JobPool Pool,
        ContainerReference Container,
        TimeSpan Timeout,
        TimeSpan CancelTimeout,
        IEnumerable<Variable> Variables,
        IEnumerable<Step> Steps,
        JobWorkspace Workspace = JobWorkspace.Outputs,
        bool ContinueOnError = false);

    // TODO: missing properties Uses, Services
}
