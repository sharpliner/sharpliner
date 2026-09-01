using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AdvancedSecurityCodeqlAnalyzeTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new AdvancedSecurityCodeqlAnalyzeTask();

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Wait_For_Processing_Test()
    {
        var task = new AdvancedSecurityCodeqlAnalyzeTask
        {
            WaitForProcessing = true,
            WaitForProcessingInterval = "10",
            WaitForProcessingTimeout = "300",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_Builder_Test()
    {
        var pipeline = new AdvancedSecurityCodeqlAnalyzeTaskPipeline();

        return Verify(pipeline.Serialize());
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(10, 0)]
    [InlineData(10, -1)]
    public void Analyze_And_Wait_Throws_For_Non_Positive_Values(int? waitForProcessingIntervalSeconds, int? waitForProcessingTimeoutSeconds)
    {
        var pipeline = new AdvancedSecurityCodeqlAnalyzeInvalidBuilderPipeline(waitForProcessingIntervalSeconds, waitForProcessingTimeoutSeconds);

        Assert.Throws<ArgumentOutOfRangeException>(() => pipeline.Serialize());
    }

    private abstract class TestPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
    }

    private class AdvancedSecurityCodeqlAnalyzeTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        AdvancedSecurityCodeql.Analyze,
                        AdvancedSecurityCodeql.AnalyzeAndWait(10, 300),
                    }
                }
            }
        };
    }

    private class AdvancedSecurityCodeqlAnalyzeInvalidBuilderPipeline(int? waitForProcessingIntervalSeconds, int? waitForProcessingTimeoutSeconds) : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        AdvancedSecurityCodeql.AnalyzeAndWait(waitForProcessingIntervalSeconds, waitForProcessingTimeoutSeconds),
                    }
                }
            }
        };
    }
}
