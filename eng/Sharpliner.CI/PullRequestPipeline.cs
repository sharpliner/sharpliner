﻿using Sharpliner.AzureDevOps;
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
                        StepTemplate(InstallDotNetTemplate.Path, new()
                        {
                            { "version", "6.0.100-rc.2.21505.57" }
                        }),

                        ValidateYamlsArePublished("eng/Sharpliner.CI/Sharpliner.CI.csproj", false),

                        DotNet
                            .Build("Sharpliner.sln", includeNuGetOrg: true)
                            .DisplayAs("Build"),
                                
                        // dotnet test needs .NET 5
                        StepTemplate(InstallDotNetTemplate.Path, new()
                        {
                            { "version", "5.0.402" }
                        }),

                        DotNet
                            .Command(DotNetCommand.Test, projects: "Sharpliner.sln")
                            .DisplayAs("Test"),
                    }
                }
            },
        };
    }
}
