using System;

namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#variables
    public abstract record VariableBase
    {
        public static implicit operator ConditionedDefinition<VariableBase>(VariableBase definition) => new(definition, null);
    }

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
}
