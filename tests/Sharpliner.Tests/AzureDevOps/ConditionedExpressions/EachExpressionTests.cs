using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps.ConditionedExpressions;

public class EachExpressionTests
{
    private class Each_Expression_Test_Pipeline : TestPipeline
    {
        StringParameter Environment = new("environment", "Environment", "Dev", ["Dev", "Int", "Prod"]);

        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                Each("env", Environment)
                    .StageTemplate("../stages/provision.yml", new()
                    {
                        { "environment", "${{ env }}" },
                        {
                            If.Equal("env.deploymentEnvironmentName", "''"), new TemplateParameters()
                            {
                                { "deploymentEnvironment", parameters["applicationName"] + "-${{ env.name }}"  }
                            }
                        },
                        {
                            "${{ else }}",
                            new TemplateParameters()
                            {
                                { "deploymentEnvironment", "${{ env.deploymentEnvironmentName }}" }
                            }
                        },
                        { "regions", parameters["regions"] },
                    }),

                If.IsBranch("main")
                    .Each("env", "parameters.stages")
                        .Stage(new Stage("stage-${{ env.name }}")
                        {

                        })
                        .Stage(new Stage("stage2-${{ env.name }}")
                        {
                            Jobs =
                            {
                                Each("foo", "bar")
                                    .Job(new Job("job-${{ foo }}"))
                                .EndEach
                                .If.Equal("foo", "bar")
                                    .Job(new Job("job2-${{ foo }}"))
                            }
                        }),
            }
        };
    }

    [Fact]
    public Task Each_Expression_Test()
    {
        var pipeline = new Each_Expression_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
