namespace Sharpliner.AzureDevOps
{
    public record Pool(string? Name = null);

    public record HostedPool(string? Name = null, string? VmImage = null) : Pool(Name)
    {
        public ConditionedDefinitionList<string> Demands { get; init; } = new();
    }
}
