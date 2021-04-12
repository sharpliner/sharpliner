namespace Sharpliner.Model
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#variables
    public abstract record VariableDefinition;

    public record VariableGroup(string Name) : VariableDefinition;

    public record Variable(
        string Name,
        string Value, // TODO: More overloads
        bool Readonly = true) : VariableDefinition;
}
