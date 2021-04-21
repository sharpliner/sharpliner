using System;

namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#variables
    public record VariableBase;

    public record VariableGroup(string Name) : VariableBase;

    public record Variable : VariableBase
    {
        public string Name { get; }

        public object Value { get; }

        public bool Readonly { get; init; }

        private Variable(string name, object value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Variable(string name, string value)
            : this(name, (object)value)
        {
        }

        public Variable(string name, int value)
            : this(name, (object)value)
        {
        }

        public Variable(string name, bool value)
            : this(name, (object)value)
        {
        }

        public Variable ReadOnly() => this with
        {
            Readonly = true
        };
    }

    public static class VariableConditionDefinitions
    {
        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, string value)
            => new(condition.ToString(), new Variable(name, value));

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, bool value)
            => new(condition.ToString(), new Variable(name, value));

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, int value)
            => new(condition.ToString(), new Variable(name, value));

        public static ConditionedDefinition<VariableBase> Group(this Condition condition, string name)
            => new(condition.ToString(), new VariableGroup(name));

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> condition,
            string name,
            string value)
        {
            condition.Definitions.Add(new Variable(name, value));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> condition,
            string name,
            bool value)
        {
            condition.Definitions.Add(new Variable(name, value));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> condition,
            string name,
            int value)
        {
            condition.Definitions.Add(new Variable(name, value));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> Group(
            this ConditionedDefinition<VariableBase> condition,
            string name)
        {
            condition.Definitions.Add(new VariableGroup(name));
            return condition;
        }
    }
}
