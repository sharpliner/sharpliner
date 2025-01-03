using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.Tests.AzureDevOps;

public class ParameterReferenceTests
{
    private class ParameterReferenceTest_Template : StageTemplateDefinition
    {
        public override string TargetFile => "stages.yml";

        public override List<Parameter> Parameters =>
        [
            StageListParameter("stages"),
            JobListParameter("jobs"),
            StepListParameter("steps"),
            ObjectParameter("variables"),
            ObjectParameter("pool", defaultValue: new()
            {
                { "vmImage", "windows-latest" }
            }),
        ];

        public override ConditionedList<Stage> Definition =>
        [
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
                            },

                            Checkout.Repository(parameters["repository"]) with
                            {
                                Submodules = parameters["submodules"]
                            }
                        }
                    },

                    parameters["jobs"],
                }
            },

            parameters["stages"],
        ];
    }

    // This tests that we can include parameters at any point of the pipeline
    [Fact]
    public Task PipelineParameter_Serialization_Test()
    {
        var template = new ParameterReferenceTest_Template();

        return Verify(template.Serialize());
    }
}
