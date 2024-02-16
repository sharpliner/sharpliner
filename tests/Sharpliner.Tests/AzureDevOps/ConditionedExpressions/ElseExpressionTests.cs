using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps.ConditionedExpressions;

public class ElseExpressionTests
{
    private class Else_Expression_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                new Stage("foo")
                {
                    Jobs =
                    [
                        new Job("job1")
                        {
                            Steps =
                            [
                                DotNet.Pack("ProjectFile") with
                                {
                                    Inputs = new()
                                    {
                                        {
                                            If.Equal(parameters["IncludeSymbols"], "true"), new TaskInputs()
                                            {
                                                ["arguments"] = "--configuration $(BuildConfiguration) --no-restore --no-build -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg"
                                            }
                                        },
                                        {
                                            Else, new TaskInputs()
                                            {
                                                ["arguments"] = "--configuration $(BuildConfiguration) --no-restore --no-build"
                                            }
                                        }
                                    },
                                },
                            ]
                        }
                    ]
                }
            }
        };
    }

    [Fact]
    public void Else_Expression_Test()
    {
        var pipeline = new Else_Expression_Test_Pipeline();
        pipeline.Serialize().Trim().Should().Be(
            """
            stages:
            - stage: foo
              jobs:
              - job: job1
                steps:
                - task: DotNetCoreCLI@2
                  inputs:
                    command: pack
                    packagesToPack: ProjectFile
                    ${{ if eq(parameters.IncludeSymbols, true) }}:
                      arguments: --configuration $(BuildConfiguration) --no-restore --no-build -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
                    ${{ else }}:
                      arguments: --configuration $(BuildConfiguration) --no-restore --no-build
            """);
    }
}
