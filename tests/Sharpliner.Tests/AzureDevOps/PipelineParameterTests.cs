using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineParameterTests
{
    private class PipelineParameterTests_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Parameters =
            {
                StringParameter("project", "AzureDevops project"),
                StringParameter("version", ".NET version", allowedValues: new[] { "5.0.100", "5.0.102" }),
                BooleanParameter("restore", "Restore NuGets", defaultValue: true),
                ObjectParameter<string>("list", "List input", new() { "Azure" , "DevOps" }),
                StepParameter("afterBuild", "After steps", Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)")),
            }
        };
    }

    [Fact]
    public void PipelineParameter_Serialization_Test()
    {
        var yaml = new PipelineParameterTests_Pipeline().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: project
              displayName: AzureDevops project
              type: string

            - name: version
              displayName: .NET version
              type: string
              values:
              - 5.0.100
              - 5.0.102

            - name: restore
              displayName: Restore NuGets
              type: boolean
              default: true

            - name: list
              displayName: List input
              type: object
              default:
              - Azure
              - DevOps

            - name: afterBuild
              displayName: After steps
              type: step
              default:
                bash: |-
                  cp -R logs $(Build.ArtifactStagingDirectory)
            """);
    }
}
