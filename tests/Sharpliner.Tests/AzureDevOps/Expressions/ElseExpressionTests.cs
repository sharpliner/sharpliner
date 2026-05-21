using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps.Expressions;

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
    public Task Else_Expression_Test()
    {
        var pipeline = new Else_Expression_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Multiple_IfElse_Groups_Pipeline : TestPipeline
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
                                new AzureDevOpsTask("SomeTask@1")
                                {
                                    Inputs =
                                    {
                                        [If.Equal(parameters["useEndpoint"], "true")] = new TaskInputs
                                        {
                                            ["EndpointProviderType"] = "Ev2Endpoint",
                                            ["ConnectedServiceName"] = "${{ parameters.serviceConnection }}",
                                        },
                                        [Else] = new TaskInputs
                                        {
                                            ["EndpointProviderType"] = "ApprovalService",
                                        },
                                        [If.Equal(parameters["rings"], "")] = new TaskInputs
                                        {
                                            ["StageMapName"] = "Microsoft.Azure.SDP.Standard",
                                            ["Select"] = "regions(${{ parameters.regions }})",
                                        },
                                        [Else] = new TaskInputs
                                        {
                                            ["StageMapName"] = "${{ parameters.environment }}.Rings",
                                            ["Select"] = "rings(${{ parameters.rings }})",
                                        },
                                    },
                                },
                            ]
                        }
                    ]
                }
            }
        };
    }

    /// <summary>
    /// Regression test for https://github.com/sharpliner/sharpliner/issues/537:
    /// Two independent if/else groups inside the same task inputs block must both appear
    /// in the generated YAML — the second ${{ else }} must not overwrite the first one.
    /// </summary>
    [Fact]
    public Task Multiple_IfElse_Groups_Test()
    {
        var pipeline = new Multiple_IfElse_Groups_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
