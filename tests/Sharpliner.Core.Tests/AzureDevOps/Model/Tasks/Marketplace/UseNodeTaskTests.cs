using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;
using System.Linq;

namespace Sharpliner.Tests.AzureDevOps;

public class UseNodeTaskTests
{
    private readonly NodeTaskBuilder _builder = new();

    private class Node_Pipeline(Step step) : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job")
                {
                    Steps = { step }
                }
            }
        };
    }

    [Fact]
    public Task Install_By_Version_Spec_Test()
    {
        var task = _builder.Install.Version(
            "20.x",
            checkLatest: true,
            architecture: NodeArchitecture.X86,
            nodejsMirror: "https://npmmirror.com/mirrors/node",
            retryCountOnDownloadFails: 7,
            delayBetweenRetries: 2000);

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Install_By_Version_File_Test()
    {
        var task = _builder.Install.FromFile(
            ".nvmrc",
            checkLatest: false,
            architecture: NodeArchitecture.X64,
            nodejsMirror: "https://nodejs.org/dist",
            retryCountOnDownloadFails: 5,
            delayBetweenRetries: 1000);

        return Verify(GetYaml(task));
    }

    [Fact]
    public void Defaults_Are_Aligned_With_UseNodeV1_Task_Specification()
    {
        var task = new UseNodeTask();

        Assert.Equal(UseNodeVersionSource.Spec, task.VersionSource);
        Assert.Equal("10.x", task.Version?.FlattenDefinitions().Single());
        Assert.Equal(NodeArchitecture.X64, task.Architecture);
        Assert.Equal("https://nodejs.org/dist", task.NodejsMirror?.FlattenDefinitions().Single());
        Assert.Equal(5, task.RetryCountOnDownloadFails?.FlattenDefinitions().Single());
        Assert.Equal(1000, task.DelayBetweenRetries?.FlattenDefinitions().Single());
    }

    private static string GetYaml(Step task) => new Node_Pipeline(task).Serialize();
}
