using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps
{
    public record AzureDevOpsPipeline
    {
        private List<ConditionedDefinition<VariableBase>> _variables = new();

        [YamlMember(Order = 100)]
        public string? Name { get; init; }

        [YamlMember(Order = 200)]
        public Trigger? Trigger { get; init; } = null;

        [YamlMember(Order = 300)]
        public PrTrigger? Pr { get; init; } = null;

        [YamlMember(Order = 400)]
        public Resources? Resources { get; init; }

        [YamlMember(Order = 500)]
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

        // TODO: Scheduled triggers

        [YamlMember(Order = 600)]
        public List<Stage> Stages { get; init; } = new();
    }
}
