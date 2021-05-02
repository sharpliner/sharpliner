using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps
{
    public class ConditionedDefinitionList<T> : List<T> where T : ConditionedDefinition
    {
        public new void Add(T item)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the top-level definition
            while (item.Parent != null)
            {
                item = (T)item.Parent;
            }

            base.Add(item);
        }
    }

    public record AzureDevOpsPipeline
    {
        private ConditionedDefinitionList<ConditionedDefinition<VariableBase>> _variables = new();

        [YamlMember(Order = 100)]
        public string? Name { get; init; }

        [YamlMember(Order = 200)]
        public Trigger? Trigger { get; init; } = null;

        [YamlMember(Order = 300)]
        public PrTrigger? Pr { get; init; } = null;

        [YamlMember(Order = 400)]
        public Resources? Resources { get; init; }

        [YamlMember(Order = 500)]
        public ConditionedDefinitionList<ConditionedDefinition<VariableBase>> Variables { get; } = new();

        // TODO: Scheduled triggers

        [YamlMember(Order = 600)]
        public List<Stage> Stages { get; init; } = new();
    }
}
