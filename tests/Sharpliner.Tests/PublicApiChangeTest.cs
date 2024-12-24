using PublicApiGenerator;

namespace Sharpliner.Tests;

public class PublicApiChangeTest
{
    /// <summary>
    /// This test ensures that the public API of the library hasn't changed.
    /// If the API has changed, the PublicApiExport.txt file should be updated.
    /// </summary>
    [Fact]
    public Task PublicApisHaventChangedTest()
    {
        var publicApi = typeof(ISharplinerDefinition).Assembly.GeneratePublicApi();

        return Verify(publicApi)
            .UseFileName("PublicApiExport.txt")
            .UseDirectory(".");
    }
}
