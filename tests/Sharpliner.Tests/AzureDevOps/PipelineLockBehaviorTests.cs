using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineLockBehaviorTests
{
    private class PipelineLockBehaviorTests_Pipeline : SimpleTestPipeline
    {
        public PipelineLockBehaviorTests_Pipeline(LockBehavior? lockBehavior)
        {
            Pipeline =  new()
            {
                Name = "LockBehaviorTest",
                LockBehavior = lockBehavior
            };
        }
        public override SingleStagePipeline Pipeline { get; }
    }

    [Theory]
    [InlineData(LockBehavior.Sequential, "sequential")]
    [InlineData(LockBehavior.RunLatest, "runLatest")]
    public Task PipelineLockBehavior_Serialization_TestValues(LockBehavior lockBehaviour, string expectedSerializedValue)
    {
        var pipeline = new PipelineLockBehaviorTests_Pipeline(lockBehaviour);

        return Verify(pipeline.Serialize())
            .UseParameters(lockBehaviour, expectedSerializedValue);
    }

    [Fact]
    public Task PipelineLockBehavior_Serialization_Test_Null()
    {
        var pipeline = new PipelineLockBehaviorTests_Pipeline(null);

        return Verify(pipeline.Serialize());
    }
}
