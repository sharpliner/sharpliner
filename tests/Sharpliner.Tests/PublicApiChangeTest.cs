using PublicApiGenerator;

namespace Sharpliner.Tests;

public class PublicApiChangeTest
{
    /// <summary>
    /// This test ensures that the public API of the library hasn't changed.
    /// If the API has changed, the PublicApiExport.txt file should be updated.
    /// You can do so by running the PublicApiExporter project.
    /// </summary>
    [Fact]
    public Task VerifyPublicApi()
    {
        var publicApi = typeof(ISharplinerDefinition).Assembly.GeneratePublicApi();
        
        return Verify(publicApi)
            .UseFileName("PublicApiExport.txt")
            .UseDirectory(".");
    }
}
