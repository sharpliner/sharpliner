using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps.ConditionedExpressions;

public class ConditionalsTests
{
    private class And_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.And(
                    NotIn("bar", "foo", "xyz", "'foo'"),
                    NotEqual(variables["Configuration"], "'Debug'"),
                    ContainsValue("10", variables.System.JobId)
                )
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)"),
            }
        };
    }

    [Fact]
    public Task And_Condition_Test()
    {
        var pipeline = new And_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Or_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.Or(
                    And(
                        Less("5", "3"),
                        Equal(variables.Build.SourceBranch, "'refs/heads/production'"),
                        IsBranch("release")),
                    NotEqual(variables["Configuration"], "'Debug'"),
                    IsPullRequest)
                    .Variable("TargetBranch", variables.System.PullRequest.SourceBranch),
            }
        };
    }

    [Fact]
    public Task Or_Condition_Test()
    {
        var pipeline = new Or_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Branch_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("feature", "on"),
                If.IsBranch("refs/heads/master")
                    .Variable("legacy", true)
                .EndIf
                .If.IsBranch("patch-1")
                    .VariableTemplate("patch-template.yml", new()
                    {
                        ["target"] = "patch-branch"
                    })
                    .Variable("fast", true),
                If.And(IsPullRequest, IsNotBranch("main"))
                    .Group("pr-group")
                    .VariableTemplate("pr-template.yml", new()
                    {
                        ["target"] = "pr-branch"
                    }),
            },
        };
    }

    [Fact]
    public Task Branch_Condition_Test()
    {
        var pipeline = new Branch_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Else_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.Equal("a", "b")
                    .Variable("feature", "on")
                    .Variable("feature2", "on")
                .Else
                    .Variable("feature", "off")
                    .Variable("feature2", "off"),

                If.ContainsValue("'foo'", "'bar'", "'foo'", "'xyz'")
                    .Variable("feature", "on")
                    .Variable("feature2", "on")
                    .If.And(Equal("e", "f"), NotEqual("g", "h"))
                        .Variable("feature", "on")
                        .Variable("feature2", "on")
                    .Else
                        .Variable("feature", "off")
                        .Variable("feature2", "off"),
            }
        };
    }

    [Fact]
    public Task Else_Test()
    {
        var pipeline = new Else_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class ConditionedValueWithElse_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Job")
                {
                    Pool = If.Equal("A", "B")
                                .Pool(new HostedPool("pool-A")
                                {
                                    Demands = { "SomeProperty -equals SomeValue" }
                                })
                            .Else
                                .Pool(new HostedPool("pool-B")),
                }
            }
        };
    }

    [Fact]
    public Task ConditionedValueWithElse_Test()
    {
        var pipeline = new ConditionedValueWithElse_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class ConditionedValueWithElseIf_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Job")
                {
                    Pool = If.Equal("A", "B")
                                .Pool(new HostedPool("pool-A")
                                {
                                    Demands = { "SomeProperty -equals SomeValue" }
                                })
                            .EndIf
                            .If.Equal("C", "D")
                                .Pool(new HostedPool("pool-B")),
                }
            }
        };
    }

    [Fact]
    public Task ConditionedValueWithElseIf_Test()
    {
        var pipeline = new ConditionedValueWithElseIf_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Custom_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.Condition("containsValue($(System.User), 'azdobot')")
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)"),

                If.Condition(In("'foo'", "'bar'"))
                    .Variable("TargetBranch", "production"),

                If.Condition(Xor("True", "$(Variable)"))
                    .Variable("TargetBranch", "main"),
            }
        };
    }

    [Fact]
    public Task Custom_Condition_Test()
    {
        var pipeline = new Custom_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class Parameter_Condition_Test_Pipeline : TestPipeline
    {
        static Parameter<bool?> param2 = new BooleanParameter("param2");
        public override Pipeline Pipeline => new()
        {
            Parameters = { BooleanParameter("param1"), param2 },
            Variables =
            {
                If.Condition(parameters["param1"])
                    .Variable("feature1", "on"),
                If.Condition(param2)
                    .Variable("feature2", "on")
            }
        };
    }

    [Fact]
    public void Parameter_Condition_Test()
    {
        var pipeline = new Parameter_Condition_Test_Pipeline();

        var yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
            """
            parameters:
            - name: param1
              type: boolean

            - name: param2
              type: boolean

            variables:
            - ${{ if parameters.param1 }}:
              - name: feature1
                value: on

            - ${{ if parameters.param2 }}:
              - name: feature2
                value: on
            """
        );
    }

    private class Contains_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.Contains("refs/heads/feature/", variables.Build.SourceBranch)
                    .Variable("feature", "on"),

                If.Contains("refs/heads/feature/", variables["Build.SourceBranch"])
                    .Variable("feature", "on")
            }
        };
    }

    [Fact]
    public Task Contains_Condition_Test()
    {
        var pipeline = new Contains_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class StartsWith_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.StartsWith("refs/heads/feature/", variables.Build.SourceBranch)
                    .Variable("feature", "on"),

                If.StartsWith("refs/heads/feature/", variables["Build.SourceBranch"])
                    .Variable("feature", "on")
            }
        };
    }

    [Fact]
    public Task StartsWith_Condition_Test()
    {
        var pipeline = new StartsWith_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class EndsWith_Condition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.EndsWith("refs/heads/feature/", variables.Build.SourceBranch)
                    .Variable("feature", "on"),

                If.EndsWith("refs/heads/feature/", variables["Build.SourceBranch"])
                    .Variable("feature", "on")
            }
        };
    }

    [Fact]
    public Task EndsWith_Condition_Test()
    {
        var pipeline = new EndsWith_Condition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class IfCondition_And_InlineCondition_Test_Pipeline : TestPipeline
    {
        protected Parameter Param1 = StringParameter("Param1", defaultValue: "ParamValue1");

        public override Pipeline Pipeline => new()
        {
            Parameters = { Param1 },
            Variables =
            {
                If.StartsWith("Param", Param1)
                    .Variable("feature", "on"),
            },
            Stages =
            {
                new Stage("Stage1")
                {
                    Jobs =
                    {
                        new Job("Job1")
                        {
                            Steps =
                            {
                                Script.Inline("echo Does this condition work?") with
                                {
                                    Condition = StartsWith("Param", Param1)
                                }
                            },
                            Condition = StartsWith("Param", Param1)
                        }
                    },
                    Condition = StartsWith("Param", Param1)
                }
            }
        };
    }

    [Fact]
    public Task IfCondition_And_InlineCondition_Test()
    {
        var pipeline = new IfCondition_And_InlineCondition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
