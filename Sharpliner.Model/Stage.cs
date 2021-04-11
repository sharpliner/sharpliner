using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record Stage(
        string Name,
        string DisplayName,
        IEnumerable<string> DependsOn,
        string Condition,
        IEnumerable<Variable> Variables,
        IEnumerable<Job> Jobs);
}
