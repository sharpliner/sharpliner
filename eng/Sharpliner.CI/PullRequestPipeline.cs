using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

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

                    DotNet.Run with
                    {
                        DisplayName = "Validate generated docs",
                        Arguments = "dotnet eng/DocsGenerator/Program.cs FailIfChanged"
                    },

                    ValidateYamlsArePublished("eng/Sharpliner.CI/Sharpliner.CI.csproj"),

                    DotNet
                        .Test("tests/Sharpliner.Tests/Sharpliner.Tests.csproj", "/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura")
                        .DisplayAs("Run unit tests"),

                    new PublishCodeCoverageResultsTask("tests/Sharpliner.Tests/coverage.cobertura.xml")
                    {
                        DisplayName = "Publish code coverage",
                        PathToSources = variables.Build.SourcesDirectory
                    },

                    DotNet.Pack("tests/E2E.Tests/SharplinerLibrary/E2E.Tests.SharplinerLibrary.csproj") with
                    {
                        DisplayName = "E2E tests - Pack E2E.Tests library",
                        ConfigurationToPack = "release",
                        BuildProperties = "PackageVersion=43.43.43",
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
                    },

                    DotNet.Build("tests/E2E.Tests/ProjectUsingTheLibrarySkipPublish/E2E.Tests.ProjectUsingTheLibrarySkipPublish.csproj") with
                    {
                        DisplayName = "Build project reference test with skipped publishing",
                        IncludeNuGetOrg = false,
                        WorkingDirectory = "tests/E2E.Tests",
                    },
                }
            }
        },
    };
}
