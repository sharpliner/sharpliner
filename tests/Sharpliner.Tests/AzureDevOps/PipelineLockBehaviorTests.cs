using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineLockBehaviorTests
{
    private class PipelineLockBehaviorTests_Pipeline : SimpleTestPipeline
    {
        public PipelineLockBehaviorTests_Pipeline(LockBehaviour? lockBehavior)
        {
            Pipeline =  new()
            {
                Name = "LockBehaviorTest",
                LockBehavior = lockBehavior
            };
        }
        public override SingleStagePipeline Pipeline { get; }
    }

    [Fact]
    public void PipelineLockBehavior_Serialization_Test_Sequential()
    {
        var yaml = new PipelineLockBehaviorTests_Pipeline(LockBehaviour.sequential).Serialize();

        yaml.Trim().Should().Be(
            """
            name: LockBehaviorTest

            lockBehavior: sequential
            """);
    }

    [Fact]
    public void PipelineLockBehavior_Serialization_Test_RunLatest()
    {
        var yaml = new PipelineLockBehaviorTests_Pipeline(LockBehaviour.runLatest).Serialize();

        yaml.Trim().Should().Be(
            """
            name: LockBehaviorTest

            lockBehavior: runLatest
            """);
    }

    [Fact]
    public void PipelineLockBehavior_Serialization_Test_Null()
    {
        var yaml = new PipelineLockBehaviorTests_Pipeline(null).Serialize();

        yaml.Trim().Should().Be(
            """
            name: LockBehaviorTest
            """);
    }

    [Fact]
    public void PipelineLockBehavior_Serialization_Test_Default()
    {
        var yaml = new PipelineLockBehaviorTests_Pipeline(default).Serialize();

        yaml.Trim().Should().Be(
            """
            name: LockBehaviorTest
            """);
    }
}
