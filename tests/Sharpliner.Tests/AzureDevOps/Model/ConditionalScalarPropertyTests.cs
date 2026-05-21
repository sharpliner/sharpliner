using System;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class ConditionalScalarPropertyTests
{
    private class ConditionalTimeoutPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Deployment", "Deployment")
                {
                    Timeout = If.In("parameters.rolloutInfra", "'Test'", "'PPE'")
                        .Value(TimeSpan.FromMinutes(60)),
                    CancelTimeout = If.Equal("parameters.rolloutInfra", "'Prod'")
                        .Value(TimeSpan.FromMinutes(5)),
                }
            }
        };
    }

    [Fact]
    public Task ConditionalTimeout_Preserves_Condition_Test()
    {
        ConditionalTimeoutPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class UnconditionalTimeoutPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Deployment", "Deployment")
                {
                    Timeout = TimeSpan.FromMinutes(60),
                    CancelTimeout = TimeSpan.FromMinutes(5),
                }
            }
        };
    }

    [Fact]
    public Task UnconditionalTimeout_Test()
    {
        UnconditionalTimeoutPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalStepTimeoutPipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            new InlineBashTask("echo hi")
            {
                Timeout = If.In("parameters.rolloutInfra", "'Test'", "'PPE'")
                    .Value(TimeSpan.FromMinutes(60)),
            }
        ];
    }

    [Fact]
    public Task ConditionalStepTimeout_Preserves_Condition_Test()
    {
        ConditionalStepTimeoutPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalStepTimeout_IfElsePipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            new InlineBashTask("echo hi")
            {
                Timeout = If.In("parameters.rolloutInfra", "'Test'", "'PPE'")
                    .Value(TimeSpan.FromMinutes(60))
                    .Else
                    .Value(TimeSpan.FromMinutes(120)),
            }
        ];
    }

    [Fact]
    public Task ConditionalStepTimeout_IfElse_Test()
    {
        ConditionalStepTimeout_IfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalStepTimeout_IfElseIfElsePipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            new InlineBashTask("echo hi")
            {
                Timeout = If.Equal("parameters.rolloutInfra", "'Test'")
                    .Value(TimeSpan.FromMinutes(30))
                    .ElseIf.Equal("parameters.rolloutInfra", "'PPE'")
                    .Value(TimeSpan.FromMinutes(60))
                    .Else
                    .Value(TimeSpan.FromMinutes(120)),
            }
        ];
    }

    [Fact]
    public Task ConditionalStepTimeout_IfElseIfElse_Test()
    {
        ConditionalStepTimeout_IfElseIfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalContinueOnErrorPipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            new InlineBashTask("echo hi")
            {
                ContinueOnError = If.Equal("variables['Build.Reason']", "'PullRequest'").Value(true),
            }
        ];
    }

    [Fact]
    public Task ConditionalContinueOnError_Preserves_Condition_Test()
    {
        ConditionalContinueOnErrorPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalContinueOnError_IfElsePipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            new InlineBashTask("echo hi")
            {
                ContinueOnError = If.Equal("variables['Build.Reason']", "'PullRequest'")
                    .Value(true)
                    .Else
                    .Value(false),
            }
        ];
    }

    [Fact]
    public Task ConditionalContinueOnError_IfElse_Test()
    {
        ConditionalContinueOnError_IfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalContinueOnError_IfElseIfElsePipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps =>
        [
            new InlineBashTask("echo hi")
            {
                ContinueOnError = If.Equal("variables['Build.Reason']", "'PullRequest'")
                    .Value(true)
                    .ElseIf.Equal("variables['Build.Reason']", "'Manual'")
                    .Value(false)
                    .Else
                    .Value(true),
            }
        ];
    }

    [Fact]
    public Task ConditionalContinueOnError_IfElseIfElse_Test()
    {
        ConditionalContinueOnError_IfElseIfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalTimeout_IfElsePipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Deployment", "Deployment")
                {
                    Timeout = If.In("parameters.rolloutInfra", "'Test'", "'PPE'")
                        .Value(TimeSpan.FromMinutes(60))
                        .Else
                        .Value(TimeSpan.FromMinutes(120)),
                }
            }
        };
    }

    [Fact]
    public Task ConditionalTimeout_IfElse_Test()
    {
        ConditionalTimeout_IfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalTimeout_IfElseIfElsePipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Deployment", "Deployment")
                {
                    Timeout = If.Equal("parameters.rolloutInfra", "'Test'")
                        .Value(TimeSpan.FromMinutes(30))
                        .ElseIf.Equal("parameters.rolloutInfra", "'PPE'")
                        .Value(TimeSpan.FromMinutes(60))
                        .Else
                        .Value(TimeSpan.FromMinutes(120)),
                }
            }
        };
    }

    [Fact]
    public Task ConditionalTimeout_IfElseIfElse_Test()
    {
        ConditionalTimeout_IfElseIfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class ConditionalStageDisplayName_IfElsePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                new Stage("Example")
                {
                    DisplayName = If.IsBranch("main")
                        .Value("job_1")
                        .Else
                        .Value("job_2"),
                }
            }
        };
    }

    [Fact]
    public Task ConditionalStageDisplayName_IfElse_Test()
    {
        ConditionalStageDisplayName_IfElsePipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }
}
