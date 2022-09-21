using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class StrategySerializationTests
{
    [Fact]
    public void Serialize_MatrixStrategy_Test()
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
        string yaml = SharplinerSerializer.Serialize(strategy);
        yaml.Trim().Should().Be(
            """
            matrix:
              Release:
                _BuildConfig: Release
                _BuildConfig2: Release2
              Debug:
                _BuildConfig: Debug

            maxParallel: 2
            """);
    }
}
