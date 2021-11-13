using Sharpliner.AzureDevOps;

namespace Sharpliner.CI
{
    internal class PublishPipeline : SingleStagePipelineDefinition
    {
        private const string DestPath = "artifacts/packages";

        public override string TargetFile => Pipelines.Location + "publish.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Publish", "Publish to nuget.org")
                {
                    Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                    Steps =
                    {
                        Powershell
                            .FromResourceFile("Get-Version.ps1")
                            .DisplayAs("Detect package version"),

                        StepTemplate(Pipelines.TemplateLocation + "install-dotnet-sdk.yml", new()
                        {
                            { "version", "6.0.100" }
                        }),

                        Powershell
                            .Inline("New-Item -Path 'artifacts' -Name 'packages' -ItemType 'directory'")
                            .DisplayAs($"Create {DestPath}"),

                        DotNet
                            .Build("src/Sharpliner/Sharpliner.csproj", includeNuGetOrg: true)
                            .DisplayAs("Build"),

                        DotNet
                            .Pack("src/Sharpliner/Sharpliner.csproj", $"-p:PackageVersion=$(packageVersion)") with
                            {
                                DisplayName = "Pack the .nupkg",
                                OutputDir = DestPath,
                                ConfigurationToPack = "Release",
                            },

                        Publish("Sharpliner",
                            filePath: DestPath + "/Sharpliner.$(packageVersion).nupkg",
                            displayName: "Publish build artifacts"),

                        If.And(IsNotPullRequest, IsBranch("main"))
                            .Step(Task("NuGetAuthenticate@0", "Authenticate NuGet"))
                            .Step(Task("NuGetCommand@2", "Publish to nuget.org") with
                            {
                                Inputs = new()
                                {
                                    { "command", "push" },
                                    { "packagesToPush", DestPath + "/Sharpliner.$(packageVersion).nupkg" },
                                    { "nuGetFeedType", "external" },
                                    { "publishFeedCredentials", "Sharpliner / nuget.org" },
                                }
                            })
                    }
                }
            },
        };
    }
}
