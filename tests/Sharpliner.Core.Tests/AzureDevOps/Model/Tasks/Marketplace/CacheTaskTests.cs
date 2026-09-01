using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class CacheTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Cache_Hit_Variable_Test()
    {
        var task = new CacheTask(
            CacheKey.FromSegments(
                CacheKey.Literal("nuget"),
                CacheKey.Literal("$(Agent.OS)"),
                CacheKey.File("**/packages.lock.json")),
            "$(Pipeline.Workspace)/.nuget/packages")
        {
            CacheHitVariable = "CACHE_RESTORED",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Restore_Keys_Test()
    {
        var task = new CacheTask(
            new CacheKeyBuilder()
                .Literal("npm")
                .Literal("$(Agent.OS)")
                .File("package-lock.json"),
            "$(Pipeline.Workspace)/.npm")
        {
            RestoreKeys =
            [
                new CacheKeyBuilder()
                    .Literal("npm")
                    .Literal("$(Agent.OS)"),
                new CacheKeyBuilder()
                    .Literal("npm"),
            ],
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Raw_Key_Test()
    {
        var task = new CacheTask(
            CacheKey.Raw("\"maven\" | \"$(Agent.OS)\" | **/pom.xml"),
            "$(Pipeline.Workspace)/.m2/repository");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a|b")]
    [InlineData("a\nb")]
    public void Cache_Key_Segment_Validates_Unsupported_Values(string value)
    {
        var action = () => CacheKey.Literal(value);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Cache_Path_Rejects_Wildcards()
    {
        var action = () => new CacheTask(CacheKey.Raw("\"npm\""), "$(Pipeline.Workspace)/**/*.npm");

        action.Should().Throw<ArgumentException>().WithMessage("*path*wildcards*");
    }
}
