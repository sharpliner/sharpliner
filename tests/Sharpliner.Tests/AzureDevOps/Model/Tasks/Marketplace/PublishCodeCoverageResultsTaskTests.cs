using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PublishCodeCoverageResultsTaskTests
{
    [Fact]
    public void Serialize_Task_Test()
    {
        var task = new PublishCodeCoverageResultsTask("$(System.DefaultWorkingDirectory)/MyApp/**/site/cobertura/coverage.xml")
        {
            PathToSources = "$(System.DefaultWorkingDirectory)/MyApp",
            FailIfCoverageEmpty = true,
        };

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: PublishCodeCoverageResults@2

            inputs:
              summaryFileLocation: $(System.DefaultWorkingDirectory)/MyApp/**/site/cobertura/coverage.xml
              pathToSources: $(System.DefaultWorkingDirectory)/MyApp
              failIfCoverageEmpty: true
            """);
    }

    [Fact]
    public void Serialize_Task_With_Defaults_Test()
    {
        var task = new PublishCodeCoverageResultsTask("$(System.DefaultWorkingDirectory)/MyApp/**/site/cobertura/coverage.xml");

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: PublishCodeCoverageResults@2

            inputs:
              summaryFileLocation: $(System.DefaultWorkingDirectory)/MyApp/**/site/cobertura/coverage.xml
            """);
    }
}
