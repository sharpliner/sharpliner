using System;

namespace Sharpliner.Model
{
    public record Stage
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public string[] DependsOn { get; init; } = Array.Empty<string>();
        public string Condition { get; init; }
        public Variables Variables { get; init; }
        public Jobs Jobs { get; init; }
    }
}
