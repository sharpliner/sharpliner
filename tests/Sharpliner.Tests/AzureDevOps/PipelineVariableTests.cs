using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineVariableTests
{
    private class PipelineVariableTests_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Variables =
            {
                Variable("SomeVariable", "Some Value"),
                Group("SomeGroup"),
                Template("SomeTemplate")
            }
        };
    }

    [Fact]
    public void PipelineVariable_Serialization_Test()
    {
        var yaml = new PipelineVariableTests_Pipeline().Serialize();

        yaml.Trim().Should().Be(
            """
            variables:
            - name: SomeVariable
              value: Some Value

            - group: SomeGroup

            - template: SomeTemplate
            """);
    }
}
