using Sharpliner.AzureDevOps;

namespace Sharpliner.CI;

class PullRequestPipeline : SingleStagePipelineDefinition
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
                Pool = new HostedPool("Azure Pipelines", "windows-2022"),
                Steps =
                {
                    StepLibrary(new ProjectBuildSteps("src/**/*.csproj")),

                    ValidateYamlsArePublished("eng/Sharpliner.CI/Sharpliner.CI.csproj"),

                    DotNet
                        .Test("tests/Sharpliner.Tests/Sharpliner.Tests.csproj")
                        .DisplayAs("Run unit tests"),

                    DotNet.Pack("tests/NuGet.Tests/NuGetWithBasePipeline/NuGetWithBasePipeline.csproj") with
                    {
                        DisplayName = "E2E tests - Pack NuGet.Tests library",
                        ConfigurationToPack = "Release",
                        OutputDir = "artifacts/packages",
                        WorkingDirectory = "tests/NuGet.Tests",
                    },

                    DotNet.Build("tests/NuGet.Tests/ProjectUsingTheNuGet/ProjectUsingTheNuGet.csproj") with
                    {
                        DisplayName = "E2E tests - Build NuGet.Tests project",
                        IncludeNuGetOrg = false,
                        WorkingDirectory = "tests/NuGet.Tests",
                    }
                }
            }
        },
    };
}
