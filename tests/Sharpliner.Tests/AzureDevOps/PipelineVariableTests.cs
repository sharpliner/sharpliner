using Sharpliner.AzureDevOps;
using YamlDotNet.Serialization;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineVariableTests
{
    private enum Configuration
    {
        Debug,
        [YamlMember(Alias = "Release1")]
        Release
    }

    private class PipelineVariableTests_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Variables =
            {
                Variable("SomeString", "Some Value"),
                Variable("SomeInt", 32),
                Variable("SomeBool", true),
                Group("SomeGroup"),
                VariableTemplate("SomeTemplate"),
                Variable("SomeEnum1", Configuration.Release),
                If.IsBranch("main")
                    .Variable("SomeEnum2", Configuration.Debug)
            },
        };
    }

    [Fact]
    public Task PipelineVariable_Serialization_Test()
    {
        var pipeline = new PipelineVariableTests_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
