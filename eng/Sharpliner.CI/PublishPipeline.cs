using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.CI
{
    internal class PublishPipeline : SingleStageAzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "publish.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override SingleStageAzureDevOpsPipeline Pipeline => new()
        {

            Pr = new PrTrigger("main"),

            Variables =
            {
                If.IsBranch("net-6.0")
                    .Variable("DotnetVersion", "6.0.100")
                    .Group("net6-kv")
                .Else
                    .Variable("DotnetVersion", "5.0.202"),
            },

                Jobs =
            {
                new Job("Build", "Build and test")
                {
                    Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                    Steps =
                    {
                        If.IsPullRequest
                            .Step(Powershell.Inline("Write-Host 'Hello-World'").DisplayAs("Hello world")),

                        DotNet.Install(DotNetPackageType.Sdk, "$(DotnetVersion)").DisplayAs("Install .NET SDK"),

                        DotNet.Build("src/MyProject.sln", includeNuGetOrg: true).DisplayAs("Build"),

                        DotNet.Command(DotNetCommand.Test, projects: "src/MyProject.sln").DisplayAs("Test"),
                    }
                }
            },
        };
    }
}
