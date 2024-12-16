using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class VariableReferenceTests
{
    private class VariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        private static readonly Variable v_agentOs = new("agentOs", "Windows_NT");
        private static readonly Variable v_continue = new("continue", "true");

        public override ConditionedList<Stage> Definition =>
        [
            new Stage("Stage_1")
            {
                Jobs =
                {
                    new Job("Job_1")
                    {
                        Pool = variables["pool"],

                        Variables = [v_agentOs, v_continue],

                        Steps =
                        {
                            If.Equal(variables[v_agentOs], "Windows_NT")
                                .Step(variables["steps"]),

                            variables["steps"],

                            Bash.Inline($"curl -o {variables.Agent.TempDirectory}/sharpliner.zip") with
                            {
                                ContinueOnError = variables[v_continue],
                            }
                        }
                    },

                    variables["jobs"],
                }
            },

            variables["stages"],
        ];
    }

    // This tests that we can include variables at any point of the pipeline
    [Fact]
    public void PipelineVariable_Serialization_Test()
    {
        var yaml = new VariableReferenceTest_Template().Serialize();

        yaml.Trim().Should().Be(
            """
            stages:
            - stage: Stage_1
              jobs:
              - job: Job_1
                pool: ${{ variables['pool'] }}
                variables:
                - name: agentOs
                  value: Windows_NT

                - name: continue
                  value: true
                steps:
                - ${{ variables['steps'] }}
                - ${{ variables['steps'] }}

                - bash: |-
                    curl -o $(Agent.TempDirectory)/sharpliner.zip
                  continueOnError: ${{ variables['continue'] }}

              - ${{ variables['jobs'] }}
            - ${{ variables['stages'] }}
            """);
    }
}
