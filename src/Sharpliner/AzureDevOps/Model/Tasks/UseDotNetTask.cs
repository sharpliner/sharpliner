using System;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/tool/dotnet-core-tool-installer?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record UseDotNetTask : AzureDevOpsTask
    {
        public UseDotNetTask(DotNetPackageType packageType, string version) : base("UseDotNet@2")
        {
            var type = packageType switch
            {
                DotNetPackageType.Sdk => "sdk",
                DotNetPackageType.Runtime => "runtime",
                _ => throw new ArgumentOutOfRangeException(nameof(packageType)),
            };

            Inputs = new()
            {
                { "packageType", type },
                { "version", version },
            };
        }
    }

    public enum DotNetPackageType
    {
        Sdk,
        Runtime,
    }
}
