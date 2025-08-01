﻿using Sharpliner.AzureDevOps;
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
}
