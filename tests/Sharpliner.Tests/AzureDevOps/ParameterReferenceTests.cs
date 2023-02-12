using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class ParameterReferenceTests
{
    private class ParameterReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        public override List<Parameter> Parameters => new()
        {
            StageListParameter("stages"),
            JobListParameter("jobs"),
            StepListParameter("steps"),
            ObjectParameter("variables"),
            ObjectParameter("pool", new()
            {
                { "vmImage", "windows-latest" }
            }),
        };

        public override ConditionedList<Stage> Definition => new()
        {
            new Stage("Stage_1")
            {
                Jobs =
                {
                    new Job("Job_1")
                    {
                        Pool = parameters["pool"],

                        Steps =
                        {
                            If.Equal(parameters["agentOs"], "Windows_NT")
                                .Step(parameters["steps"]),

                            parameters["steps"],

                            Bash.Inline("curl -o $(Agent.TempDirectory)/sharpliner.zip") with
                            {
                                ContinueOnError = parameters["continue"],
                            }
                        }
                    },

                    parameters["jobs"],
                }
            },

            parameters["stages"],
        };
    }

    // This tests that we can include parameters at any point of the pipeline
    [Fact]
    public void PipelineParameter_Serialization_Test()
    {
        var yaml = new ParameterReferenceTest_Template().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: stages
              type: stageList

            - name: jobs
              type: jobList

            - name: steps
              type: stepList

            - name: variables
              type: object

            - name: pool
              type: object
              default:
                vmImage: windows-latest

            stages:
            - stage: Stage_1
              jobs:
              - job: Job_1
                pool: ${{ parameters.pool }}
                steps:
                - ${{ parameters.steps }}
                - ${{ parameters.steps }}

                - bash: |-
                    curl -o $(Agent.TempDirectory)/sharpliner.zip
                  continueOnError: ${{ parameters.continue }}

              - ${{ parameters.jobs }}
            - ${{ parameters.stages }}
            """);
    }
}
