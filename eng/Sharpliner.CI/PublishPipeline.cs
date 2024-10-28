using Sharpliner.AzureDevOps;

namespace Sharpliner.CI;

class PublishPipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => Pipelines.Location + "publish.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Jobs =
        {
            new Job("Publish", "Publish to nuget.org")
            {
                Pool = new HostedPool("Azure Pipelines", "windows-2022"),
                Steps =
                {
                    Powershell
                        .FromResourceFile("Get-Version.ps1")
                        .DisplayAs("Detect package version"),

                    StepLibrary(new ProjectBuildSteps("src/Sharpliner/Sharpliner.csproj")),

                    DotNet
                        .Pack("src/Sharpliner/Sharpliner.csproj", $"-p:PackageVersion={variables["packageVersion"]}") with
                        {
                            DisplayName = "Pack the .nupkg",
                            OutputDir = ProjectBuildSteps.PackagePath,
                            ConfigurationToPack = "Release",
                        },

                    Publish("Sharpliner",
                        filePath:  $"{ProjectBuildSteps.PackagePath}/Sharpliner.{variables["packageVersion"]}.nupkg",
                        displayName: "Publish build artifacts"),

                    If.And(IsNotPullRequest, IsBranch("main"))
                        .Step(Task("NuGetAuthenticate@1", "Authenticate NuGet"))
                        .Step(Task("NuGetCommand@2", "Publish to nuget.org") with
                        {
                            Inputs = new()
                            {
                                { "command", "push" },
                                { "packagesToPush", $"{ProjectBuildSteps.PackagePath}/Sharpliner.{variables["packageVersion"]}.nupkg" },
                                { "nuGetFeedType", "external" },
                                { "publishFeedCredentials", "Sharpliner / nuget.org" },
                            }
                        })
                }
            }
        },
    };
}
