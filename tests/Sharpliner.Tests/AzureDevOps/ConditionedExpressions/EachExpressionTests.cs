using System.Linq;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps.ConditionedExpressions;

public class EachExpressionTests
{
    private class Each_Expression_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                Each("env", "parameters.environments")
                    .StageTemplate("../stages/provision.yml", new()
                    {
                        { "environment", "${{ env }}" },
                        {
                            If.Equal("env.deploymentEnvironmentName", "''"), new TemplateParameters()
                            {
                                { "deploymentEnvironment", parameters["applicationName"] + " - env.name"  }
                            }
                        },
                        {
                            "${{ else }}",
                            // TODO: Else,
                            new TemplateParameters()
                            {
                                { "deploymentEnvironment", "env.deploymentEnvironmentName" }
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

                        }),
            }
        };
    }

    [Fact]
    public void Each_Expression_Test()
    {
        var pipeline = new Each_Expression_Test_Pipeline();
        pipeline.Serialize().Trim().Should().Be(
            """
            stages:
            - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
              - ${{ each env in parameters.stages }}:
                - stage: stage-${{ env.name }}
            
                - stage: stage2-${{ env.name }}
            """);
    }
}
