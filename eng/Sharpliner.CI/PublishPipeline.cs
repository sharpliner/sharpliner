using Sharpliner.AzureDevOps;

namespace Sharpliner.CI
{
    internal class PublishPipeline : SingleStagePipelineDefinition
    {
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

                        Template<Step>(InstallDotNetTemplate.Path),

                        Powershell
                            .Inline("New-Item -Path 'artifacts' -Name 'packages' -ItemType 'directory'")
                            .DisplayAs("Create artifacts/packages"),

                        DotNet
                            .Build("src/Sharpliner/Sharpliner.csproj", arguments: "-c Release", includeNuGetOrg: true)
                            .DisplayAs("Build"),

                        DotNet
                            .Custom("pack", arguments:
                                "src/Sharpliner/Sharpliner.csproj " +
                                "-c Release --output artifacts/packages " +
                                "-p:PackageVersion=$(majorVersion).$(minorVersion).$(patchVersion)")
                            .DisplayAs("Pack the .nupkg"),

                        Publish("Sharpliner",
                            filePath: "artifacts/packages/Sharpliner.$(majorVersion).$(minorVersion).$(patchVersion).nupkg",
                            displayName: "Publish build artifacts"),

                        If.And(IsNotPullRequest, IsBranch("refs/heads/main"))
                            .Step(Task("NuGetAuthenticate@0", "Authenticate NuGet"))
                            .Step(Task("NuGetCommand@2", "Publish to nuget.org") with
                            {
                                Inputs = new()
                                {
                                    { "command", "push" },
                                    { "packagesToPush", "artifacts/packages/Sharpliner.$(majorVersion).$(minorVersion).$(patchVersion).nupkg" },
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
