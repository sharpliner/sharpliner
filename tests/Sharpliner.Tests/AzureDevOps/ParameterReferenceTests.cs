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
        private static readonly Parameter p_pool = ObjectParameter("pool", new()
        {
            { "vmImage", "windows-latest" }
        });

        private static readonly Parameter p_stages = StageListParameter("stages");
        private static readonly Parameter p_jobs = JobListParameter("jobs");
        private static readonly Parameter p_steps = StepListParameter("steps");
        private static readonly Parameter p_variables = ObjectParameter("variables");

        public override string TargetFile => "stages.yml";

        public override List<Parameter> Parameters =>
        [
            p_stages,
            p_jobs,
            p_steps,
            p_variables,
            p_pool,
        ];

        public override ConditionedList<Stage> Definition =>
        [
            new Stage("Stage_1")
            {
                Jobs =
                {
                    new Job("Job_1")
                    {
                        Pool = parameters[p_pool],

                        Steps =
                        {
                            If.Equal(parameters["agentOs"], "Windows_NT")
                                .Step(parameters[p_steps]),

                            parameters[p_steps],

                            Bash.Inline($"curl -o {variables.Agent.TempDirectory}/sharpliner.zip") with
                            {
                                ContinueOnError = parameters["continue"],
                            },

                            Checkout.Repository(parameters["repository"]) with
                            {
                                Submodules = parameters["submodules"]
                            }
                        }
                    },

                    parameters[p_jobs],
                }
            },

            parameters[p_stages],
        ];
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

                - checkout: ${{ parameters.repository }}
                  submodules: ${{ parameters.submodules }}

              - ${{ parameters.jobs }}
            - ${{ parameters.stages }}
            """);
    }
}
