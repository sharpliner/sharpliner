using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class PublishCodeCoverageResultsTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new PublishCodeCoverageResultsTask("$(System.DefaultWorkingDirectory)/MyApp/**/site/cobertura/coverage.xml")
        {
            PathToSources = "$(System.DefaultWorkingDirectory)/MyApp",
            FailIfCoverageEmpty = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new PublishCodeCoverageResultsTask("$(System.DefaultWorkingDirectory)/MyApp/**/site/cobertura/coverage.xml");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
