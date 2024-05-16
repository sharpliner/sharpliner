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

                    DotNet.Pack("tests/E2E.Tests/SharplinerLibrary/E2E.Tests.SharplinerLibrary.csproj") with
                    {
                        DisplayName = "E2E tests - Pack E2E.Tests library",
                        ConfigurationToPack = "Release",
                        OutputDir = "artifacts/packages",
                        WorkingDirectory = "tests/E2E.Tests",
                    },

                    DotNet.Build("tests/E2E.Tests/ProjectUsingTheLibraryNuGet/E2E.Tests.ProjectUsingTheLibraryNuGet.csproj") with
                    {
                        DisplayName = "Build NuGet reference test",
                        IncludeNuGetOrg = false,
                        WorkingDirectory = "tests/E2E.Tests",
                    },

                    DotNet.Build("tests/E2E.Tests/ProjectUsingTheLibrary/E2E.Tests.ProjectUsingTheLibrary.csproj") with
                    {
                        DisplayName = "Build project reference test",
                        IncludeNuGetOrg = false,
                        WorkingDirectory = "tests/E2E.Tests",
                    }
                }
            }
        },
    };
}
