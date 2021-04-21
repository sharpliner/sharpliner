using System;

namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#variables
    public record Variable;

    public record VariableGroup(string Name) : Variable;

    public record SingleVariable : Variable
    {
        public string Name { get; }

        public object Value { get; }

        public bool Readonly { get; init; }

        private SingleVariable(string name, object value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public SingleVariable(string name, string value)
            : this(name, (object)value)
        {
        }

        public SingleVariable(string name, int value)
            : this(name, (object)value)
        {
        }

        public SingleVariable(string name, bool value)
            : this(name, (object)value)
        {
        }

        public SingleVariable ReadOnly() => this with
        {
            Readonly = true
        };
    }
}
