using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.Model.AzureDevOps
{
    public record AzureDevOpsPipeline
    {
        private List<ConditionedDefinition<VariableBase>> _variables = new();

        public string? Name { get; init; }

        public Resources? Resources { get; init; }

        public List<ConditionedDefinition<VariableBase>> Variables
        {
            get => _variables;
            init => _variables = value
                .Select(v =>
                {
                    // Find the root definitions
                    // This is because when we define a tree of conditional definitions,
                    // the expression returns the leaf definition so we have to move back
                    // up to the top-level definitions.
                    while (v.Parent != null)
                    {
                        v = (ConditionedDefinition<VariableBase>)v.Parent;
                    }

                    return v;
                })
                .ToList();
        }

        public Trigger? Trigger { get; init; } = null;

        public PrTrigger? Pr { get; init; } = null;

        // TODO: Scheduled triggers

        public List<Stage> Stages { get; init; } = new();
    }
}
