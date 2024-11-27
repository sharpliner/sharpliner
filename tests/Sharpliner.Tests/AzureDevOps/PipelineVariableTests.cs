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
                Variable("SomeVariable", "Some Value"),
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
            - name: SomeVariable
              value: Some Value

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
