using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record Pipeline
    {
        public string? Name { get; init; }

        public Resources? Resources { get; init; }

        public IEnumerable<Variable>? Variables { get; init; }

        public IEnumerable<Trigger>? Triggers { get; init; }

        public IEnumerable<Stage>? Stages { get; init; }
    }
}
