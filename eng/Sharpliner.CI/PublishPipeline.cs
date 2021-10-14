using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.CI
{
    internal class PublishPipeline : SingleStagePipelineDefinition
    {
        private static readonly string _packageDestPath = $"artifacts/packages/Sharpliner.$(packageVersion).nupkg";

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

                        StepTemplate(InstallDotNetTemplate.Path, new()
                        {
                            { "version", "6.0.100-rc.2.21505.57" }
                        }),

                        Powershell
                            .Inline("New-Item -Path 'artifacts' -Name 'packages' -ItemType 'directory'")
                            .DisplayAs("Create artifacts/packages"),

                        DotNet
                            .Build("src/Sharpliner/Sharpliner.csproj", includeNuGetOrg: true)
                            .DisplayAs("Build"),

                        DotNet
                            .Command(DotNetCommand.Pack, "src/Sharpliner/Sharpliner.csproj -c Release --output artifacts/packages -p:PackageVersion=$(packageVersion)")
                            .DisplayAs("Pack the .nupkg"),

                        Publish("Sharpliner",
                            filePath: _packageDestPath,
                            displayName: "Publish build artifacts"),

                        /*If.And(IsNotPullRequest, IsBranch("main"))
                            .Step(Task("NuGetAuthenticate@0", "Authenticate NuGet"))
                            .Step(Task("NuGetCommand@2", "Publish to nuget.org") with
                            {
                                Inputs = new()
                                {
                                    { "command", "push" },
                                    { "packagesToPush", _packageDestPath },
                                    { "nuGetFeedType", "external" },
                                    { "publishFeedCredentials", "Sharpliner / nuget.org" },
                                }
                            })*/
                    }
                }
            },
        };
    }
}
