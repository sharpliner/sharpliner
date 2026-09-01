using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class MSBuildTaskTests
{
    [Fact]
    public Task Serialize_MSBuild_Task_With_Defaults_Test()
    {
        var task = new MSBuildTask("**/*.sln");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_MSBuild_Task_Test()
    {
        var task = new MSBuildTask("MySolution.sln")
        {
            DisplayName = "Build solution",
            Platform = "x64",
            Configuration = "Release",
            MSBuildArguments = "/t:Restore;Build",
            Clean = true,
            MaximumCpuCount = true,
            LogProjectEvents = true,
            CreateLogFile = true,
            LogFileVerbosity = MSBuildLogFileVerbosity.Diagnostic,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_MSBuild_Task_With_Explicit_Location_Test()
    {
        var task = new MSBuildTask("MySolution.sln")
        {
            MSBuildLocationMethod = MSBuildLocationMethod.Location,
            MSBuildLocationPath = "C:\\MSBuild\\MSBuild.exe",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_MSBuild_Task_With_Version_And_Architecture_Test()
    {
        var task = new MSBuildTask("MySolution.sln")
        {
            MSBuildLocationMethod = MSBuildLocationMethod.Version,
            MSBuildVersion = "17.0",
            MSBuildArchitecture = MSBuildArchitecture.X64,
            RestoreNugetPackages = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
