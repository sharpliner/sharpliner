using PublicApiGenerator;

namespace Sharpliner.Tests;

public class PublicApiChangeTest
{
    /// <summary>
    /// This test ensures that the public API of the Sharpliner.Core library hasn't changed.
    /// If the API has changed, the PublicApiExport.Sharpliner.Core.txt file should be updated.
    /// </summary>
    [Fact]
    public Task SharplinerCorePublicApisHaventChangedTest()
    {
        var publicApi = typeof(ISharplinerDefinition).Assembly.GeneratePublicApi();

        return Verify(publicApi)
            .UseFileName("PublicApiExport.Sharpliner.Core.txt")
            .UseDirectory(".");
    }

    /// <summary>
    /// This test ensures that the public API of the Sharpliner library (the MSBuild task)
    /// hasn't changed. If the API has changed, the PublicApiExport.Sharpliner.txt file should
    /// be updated.
    /// </summary>
    [Fact]
    public Task SharplinerPublicApisHaventChangedTest()
    {
        var publicApi = typeof(PublishDefinitions).Assembly.GeneratePublicApi();

        return Verify(publicApi)
            .UseFileName("PublicApiExport.Sharpliner.txt")
            .UseDirectory(".");
    }
}
