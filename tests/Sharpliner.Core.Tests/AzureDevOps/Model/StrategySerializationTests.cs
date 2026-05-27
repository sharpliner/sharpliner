using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class StrategySerializationTests
{
    [Fact]
    public Task Serialize_MatrixStrategy_Test()
    {
        var strategy = new MatrixStrategy
        {
            Matrix = new()
            {
                {
                    "Release",
                    new[]
                    {
                            ("_BuildConfig", "Release"),
                            ("_BuildConfig2", "Release2"),
                        }
                },
                { "Debug", new[] { ("_BuildConfig", "Debug") } },
            },
            MaxParallel = 2,
        };

        return Verify(SharplinerSerializer.Serialize(strategy));
    }
}
