namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops
    /// </summary>
    public record DotNetCoreCliTask : AzureDevOpsTask
    {
        public DotNetCoreCliTask() : base("DotNetCoreCLI@2")
        {
        }
    }
}
