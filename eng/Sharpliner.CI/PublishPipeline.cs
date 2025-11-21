using Sharpliner.AzureDevOps;

namespace Sharpliner.CI;

class PublishPipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => Pipelines.Location + "publish.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Trigger = new Trigger
        {
            Tags = new InclusionRule
            {
                Include = ["*"]
            }
        },
        
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

                    Powershell
                        .Inline(
                            $"(Get-Content templates/Sharpliner.Templates/sharpliner-pipeline/SharplinerPipelineProject.csproj) " +
                            $"-replace '__SHARPLINER_PACKAGE_VERSION__', '{variables["packageVersion"]}' " +
                            $"| Set-Content templates/Sharpliner.Templates/sharpliner-pipeline/SharplinerPipelineProject.csproj")
                        .DisplayAs("Update template project version"),

                    StepLibrary(new ProjectBuildSteps("src/Sharpliner/Sharpliner.csproj")),

                    DotNet
                        .Pack("src/Sharpliner/Sharpliner.csproj", $"-p:PackageVersion={variables["packageVersion"]}") with
                        {
                            DisplayName = "Pack Sharpliner .nupkg",
                            OutputDir = ProjectBuildSteps.PackagePath,
                            ConfigurationToPack = "Release",
                        },

                    DotNet
                        .Pack("templates/Sharpliner.Templates/Sharpliner.Templates.csproj", $"-p:PackageVersion={variables["packageVersion"]}") with
                        {
                            DisplayName = "Pack Sharpliner.Templates .nupkg",
                            OutputDir = ProjectBuildSteps.PackagePath,
                            ConfigurationToPack = "Release",
                        },

                    Publish
                        .Pipeline("Sharpliner", $"{ProjectBuildSteps.PackagePath}/Sharpliner.{variables["packageVersion"]}.nupkg")
                        .DisplayAs("Publish Sharpliner build artifacts"),

                    Publish
                        .Pipeline("Sharpliner.Templates", $"{ProjectBuildSteps.PackagePath}/Sharpliner.Templates.{variables["packageVersion"]}.nupkg")
                        .DisplayAs("Publish Sharpliner.Templates build artifacts"),

                    If.And(IsNotPullRequest, StartsWith("refs/tags/", variables.Build.SourceBranch))
                        .Step(NuGet.Authenticate() with { DisplayName = "Authenticate NuGet" })
                        .Step(NuGet.Push.ToExternalFeed("Sharpliner / nuget.org") with
                        {
                            DisplayName = "Publish to nuget.org",
                            PackagesToPush =
                            [
                                $"{ProjectBuildSteps.PackagePath}/Sharpliner.{variables["packageVersion"]}.nupkg",
                                $"{ProjectBuildSteps.PackagePath}/Sharpliner.Templates.{variables["packageVersion"]}.nupkg"
                            ]
                        })
                }
            }
        },
    };
}
