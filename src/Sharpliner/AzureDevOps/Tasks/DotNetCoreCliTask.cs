namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record DotNetCoreCliTask : AzureDevOpsTask
    {
        public DotNetCoreCliTask() : base("DotNetCoreCLI@2")
        {
        }
    }
}
