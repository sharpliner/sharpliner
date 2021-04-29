namespace Sharpliner.Model.Definition
{
    public sealed class PipelineVariable
    {
        public string this[string variableName] => $"variables['{variableName}']";
    }
}
