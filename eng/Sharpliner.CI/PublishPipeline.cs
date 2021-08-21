using Sharpliner.AzureDevOps;

namespace Sharpliner.CI
{
    internal class PublishPipeline : SingleStageAzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "publish.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override SingleStageAzureDevOpsPipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Build", "Build and test")
                {
                    Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                    Steps =
                    {
                        Powershell.FromResourceFile("Get-Version.ps1").DisplayAs("Parse version"),

                        Task("UseDotNet@2", "Install .NET 6 preview 3") with
                        {
                            Inputs = new()
                            {
                                { "packageType", "sdk" },
                                { "version", "6.0.100-preview.3.21202.5" },
                            }
                        },

                        Task("NuGetCommand@2", "Pack Sharpliner.csproj") with
                        {
                            Inputs = new()
                            {
                                { "command", "pack" },
                                { "includeNuGetOrg", true },
                                { "packagesToPack", "src/Sharpliner/Sharpliner.csproj" },
                                { "versioningScheme", "byPrereleaseNumber" },
                                { "majorVersion", "$(majorVersion)" },
                                { "minorVersion", "$(minorVersion)" },
                                { "patchVersion", "$(patchVersion)" },
                            }
                        },

                        If.And(IsNotPullRequest, IsBranch("refs/heads/main"))
                            .Step(Task("NuGetAuthenticate@0", "Authenticate NuGet"))
                            .Step(Task("NuGetCommand@2", "Publish Sharpliner.csproj") with
                            {
                                Inputs = new()
                                {
                                    { "command", "push" },
                                    { "packagesToPush", "artifacts/packages/Sharpliner*.nupkg" },
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
