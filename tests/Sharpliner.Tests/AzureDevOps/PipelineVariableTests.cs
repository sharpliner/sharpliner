using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineVariableTests
{
    private enum Configuration
    {
        Debug,
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
    public void PipelineVariable_Serialization_Test()
    {
        var yaml = new PipelineVariableTests_Pipeline().Serialize();

        yaml.Trim().Should().Be(
            """
            variables:
            - name: SomeString
              value: Some Value

            - name: SomeInt
              value: 32

            - name: SomeBool
              value: true

            - group: SomeGroup

            - template: SomeTemplate

            - name: SomeEnum1
              value: Release

            - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
              - name: SomeEnum2
                value: Debug
            """);
    }
}
