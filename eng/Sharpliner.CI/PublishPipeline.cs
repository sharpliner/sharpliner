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

                    Publish
                        .Pipeline("Sharpliner", $"{ProjectBuildSteps.PackagePath}/Sharpliner.{variables["packageVersion"]}.nupkg")
                        .DisplayAs("Publish build artifacts"),

                    If.And(IsNotPullRequest, IsBranch("main"))
                        .Step(NuGet.Authenticate() with { DisplayName = "Authenticate NuGet" })
                        .Step(NuGet.Push.ToExternalFeed("Sharpliner / nuget.org") with
                        {
                            DisplayName = "Publish to nuget.org",
                            PackagesToPush =
                            [
                                $"{ProjectBuildSteps.PackagePath}/Sharpliner.{variables["packageVersion"]}.nupkg"
                            ]
                        })
                }
            }
        },
    };
}
