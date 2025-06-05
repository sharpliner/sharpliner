using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.Tests.AzureDevOps;

public class VariableReferenceTests
{
    private class VariableReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        public override AdoExpressionList<Stage> Definition =>
        [
            new Stage("Stage_1")
            {
                Jobs =
                {
                    new Job("Job_1")
                    {
                        Pool = variables["pool"],

                        Steps =
                        {
                            If.Equal(variables["agentOs"], "Windows_NT")
                                .Step(variables["steps"]),

                            variables["steps"],

                            Bash.Inline("curl -o $(Agent.TempDirectory)/sharpliner.zip") with
                            {
                                ContinueOnError = variables["continue"],
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
    public Task PipelineVariable_Serialization_Test()
    {
        var pipeline = new VariableReferenceTest_Template();

        return Verify(pipeline.Serialize());
    }
}
