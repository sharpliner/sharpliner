using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.CI
{
    internal class PullRequestPipeline : SingleStageAzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override SingleStageAzureDevOpsPipeline Pipeline => new()
        {
            Trigger = new Trigger("main")
            {
                Batch = true,
            },

            Pr = new PrTrigger("main"),

            Jobs =
            {
                new Job("Build", "Build and test")
                {
                    Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                    Steps =
                    {
                        // dotnet build fails with .NET 5 SDK and the new() statements
                        DotNet.Install(DotNetPackageType.Sdk, "6.0.100-preview.3.21202.5").DisplayAs("Install .NET 6 preview 3"),
                                
                        // Validate we published the YAML
                        new SharplinerValidateTask("eng/Sharpliner.CI/Sharpliner.CI.csproj", false),

                        DotNet.Build("Sharpliner.sln", includeNuGetOrg: true).DisplayAs("Build"),
                                
                        // dotnet test somehow doesn't work with .NET 6 SDK
                        DotNet.Install(DotNetPackageType.Sdk, "5.0.202").DisplayAs("Install .NET 5"),

                        DotNet.Command(DotNetCommand.Test, projects: "Sharpliner.sln").DisplayAs("Build"),
                    }
                }
            },
        };
    }
}
