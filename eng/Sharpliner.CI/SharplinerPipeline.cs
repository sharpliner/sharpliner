﻿using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.CI
{
    internal class SharplinerPipeline : SingleStageAzureDevOpsPipelineDefinition
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
                        Task("UseDotNet@2", "Install .NET 6 preview 3") with
                        {
                            Inputs = new()
                            {
                                { "packageType", "sdk" },
                                { "version", "6.0.100-preview.3.21202.5" },
                            }
                        },
                                
                        // Validate we published the YAML
                        new SharplinerValidateTask("eng/Sharpliner.CI/Sharpliner.CI.csproj", false),

                        Task("DotNetCoreCLI@2", "dotnet build") with
                        {
                            Inputs = new()
                            {
                                { "command", "build" },
                                { "includeNuGetOrg", true },
                                { "projects", "Sharpliner.sln" },
                            }
                        },
                                
                        // dotnet test somehow doesn't work with .NET 6 SDK
                        Task("UseDotNet@2", "Install .NET 5") with
                        {
                            Inputs = new()
                            {
                                { "packageType", "sdk" },
                                { "version", "5.0.202" },
                            }
                        },

                        Task("DotNetCoreCLI@2", "dotnet test") with
                        {
                            Inputs = new()
                            {
                                { "command", "test" },
                                { "includeNuGetOrg", true },
                                { "projects", "Sharpliner.sln" },
                            }
                        },
                    }
                }
            },
        };
    }
}
