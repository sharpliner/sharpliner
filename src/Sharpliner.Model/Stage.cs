using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record Stage
    {
        public string Name { get; }

        public string DisplayName { get; }

        public IEnumerable<string>? DependsOn { get; init; }

        public string? Condition { get; init; }

        public IEnumerable<Variable>? Variables { get; init; }

        public IEnumerable<Job>? Jobs { get; init; }

        public Stage(string name, string displayName)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            DisplayName = displayName ?? throw new System.ArgumentNullException(nameof(displayName));
        }
    }
}
