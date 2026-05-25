using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DefaultDisplayNameTests
{
    [Fact]
    public void Candidate_Tasks_Should_Have_Default_DisplayName()
    {
        new UseDotNetTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Install .NET SDK");
        new NuGetAuthenticateTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Authenticate to NuGet feeds");
        new PublishCodeCoverageResultsTask("coverage.xml").DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Publish code coverage results");
        new PublishTestResultsTask(TestResultsFormat.JUnit, "**/TEST-*.xml").DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Publish test results");
        new DotNetBuildCoreCliTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("dotnet build");
        new DotNetRestoreCoreCliTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("dotnet restore");
        new DotNetPackCoreCliTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("dotnet pack");
        new DotNetPublishCoreCliTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("dotnet publish");
        new DotNetTestCoreCliTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("dotnet test");
        new DotNetPushCoreCliTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("dotnet nuget push");
        new NuGetPackCommandTaskOff().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("NuGet pack");
        new NuGetPushInternalCommandTask("feed").DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("NuGet push");
        new NuGetRestoreFeedCommandTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("NuGet restore");
        new UniversalPackagesDownloadTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Download universal package");
        new UniversalPackagesPublishTask().DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Publish universal package");
    }

    [Fact]
    public void DisplayName_Should_Be_Overridable()
    {
        var task = new NuGetAuthenticateTask
        {
            DisplayName = "Auth"
        };

        task.DisplayName!.FlattenDefinitions().Should().ContainSingle().Which.Should().Be("Auth");
    }
}
