using System.Collections.Generic;

namespace Sharpliner.AzureDevOps
{
    public record Pool(string? Name = null);

    public record HostedPool(string? Name = null, string? VmImage = null) : Pool(Name)
    {
        public List<string> Demands { get; init; } = new();
    }
}
