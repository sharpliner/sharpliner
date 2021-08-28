using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.CI
{
    internal class PullRequestPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => Pipelines.Location + "pr.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override SingleStagePipeline Pipeline => new()
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
                        Template<Step>(InstallDotNetTemplate.Path),
                                
                        // Validate we published the YAML
                        new SharplinerValidateTask("eng/Sharpliner.CI/Sharpliner.CI.csproj", false),

                        DotNet
                            .Build("Sharpliner.sln", includeNuGetOrg: true)
                            .DisplayAs("Build"),
                                
                        // dotnet test needs .NET 5
                        DotNet
                            .Install.Sdk("5.0.202")
                            .DisplayAs("Install .NET 5"),

                        DotNet
                            .Command(DotNetCommand.Test, projects: "Sharpliner.sln")
                            .DisplayAs("Build"),
                    }
                }
            },
        };
    }
}
