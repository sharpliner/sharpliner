using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineParameterTests
{
    private class PipelineParameterTests_Pipeline : SimpleTestPipeline
    {
        private static readonly Parameter p_project = StringParameter("project", "AzureDevops project");
        private static readonly Parameter p_version = StringParameter("version", ".NET version", allowedValues: [ "5.0.100", "5.0.102" ]);
        private static readonly Parameter p_restore = BooleanParameter("restore", "Restore NuGets", defaultValue: true);
        private static readonly Parameter p_list = ObjectParameter<string>("list", "List input", ["Azure" , "DevOps"]);
        private static readonly Parameter p_afterBuild = StepParameter("afterBuild", "After steps", Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)"));

        public override SingleStagePipeline Pipeline => new()
        {
            Parameters = 
            { 
              p_project, 
              p_version, 
              p_restore, 
              p_list, 
              p_afterBuild
            },
            Jobs =
            {
                new Job("Job1")
                {
                    Steps =
                    {
                        StepTemplate("beforeBuild", new()
                        {
                          ["project"] = p_project,
                          ["version"] = p_version,
                          ["restore"] = p_restore,
                          ["list"] = p_list,
                        }),
                        p_afterBuild,
                    },
                }
            },
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
