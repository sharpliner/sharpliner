using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class VSBuildTaskTests
{
    private readonly VSBuildTaskBuilder _builder = new();

    private class VSBuild_Pipeline(Step step) : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("build")
                {
                    Steps = { step }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new VSBuildTask("src/MyApp.sln")
        {
            VsVersion = VSBuildVisualStudioVersion.VisualStudio2022,
            Platform = "Any CPU",
            Configuration = "Release",
            Clean = true,
            MaximumCpuCount = true,
            MSBuildArchitecture = VSBuildArchitecture.X64,
            LogProjectEvents = true,
            CreateLogFile = true,
            LogFileVerbosity = VSBuildLogFileVerbosity.Detailed,
            EnableDefaultLogger = true,
            CustomVersion = "17.0"
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Serialize_Builder_WebPackage_Test()
    {
        var task = _builder
            .Solution("src/WebApp/WebApp.csproj")
            .PlatformAndConfiguration("Any CPU", "Release")
            .WebPackage(@"$(Build.ArtifactStagingDirectory)\WebApp.zip", packageAsSingleFile: true, skipInvalidConfigurations: true)
            .Build() with
        {
            VsVersion = VSBuildVisualStudioVersion.Latest,
            MSBuildArchitecture = VSBuildArchitecture.X64,
        };

        return Verify(GetYaml(task));
    }

    private static string GetYaml(Step step) => new VSBuild_Pipeline(step).Serialize();
}
