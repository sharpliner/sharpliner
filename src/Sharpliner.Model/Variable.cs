namespace Sharpliner.Model
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#variables
    public abstract record VariableDefinition;

    public record VariableGroup(string Name) : VariableDefinition;

    public record Variable : VariableDefinition
    {
        public string Name { get; }

        public object Value { get; }

        public bool Readonly { get; }

        private Variable(string name, object value, bool isReadonly = true)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Value = value ?? throw new System.ArgumentNullException(nameof(value));
            Readonly = isReadonly;
        }

        public Variable(string name, string value, bool isReadonly = true)
            : this(name, (object)value, isReadonly)
        {
        }

        public Variable(string name, int value, bool isReadonly = true)
            : this(name, (object)value, isReadonly)
        {
        }

        public Variable(string name, bool value, bool isReadonly = true)
            : this(name, (object)value, isReadonly)
        {
        }
    }
}
