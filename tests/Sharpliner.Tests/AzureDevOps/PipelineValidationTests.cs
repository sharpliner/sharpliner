using System;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineValidationTests
{
    private class StageDependsOnErrorPipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
                {
                    new Stage("stage_1"),
                    new Stage("stage_2")
                    {
                        DependsOn = { "stage_1" }
                    },
                    new Stage("stage_3")
                    {
                        DependsOn = { "foo" }
                    },
                }
        };
    }

    [Fact(Skip = "This check was disabled")]
    public void StageDependsOn_Validation_Test()
    {
        var pipeline = new StageDependsOnErrorPipeline();
        Action action = () => pipeline.Validate();
        var e = action.Should().Throw<Exception>();
        e.WithMessage("*stage_3*");
        e.WithMessage("*foo*");
    }

    private class JobDependsOnErrorPipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
                {
                    new Stage("stage_1")
                    {
                        Jobs =
                        {
                            new Job("job_1"),
                            new Job("job_2"),
                            new Job("job_3"),
                        }
                    },

                    new Stage("stage_2")
                    {
                        Jobs =
                        {
                            new Job("job_1"),
                            new DeploymentJob("job_2"),
                            new Job("job_4")
                            {
                                DependsOn = { "job_1", "job_2" }
                            },

                            If.IsBranch("main").Job(
                                new Job("job_5")
                                {
                                    DependsOn = { "job_2", "job_3" }
                                }),
                        }
                    },
                }
        };
    }

    [Fact(Skip = "This check was disabled")]
    public void JobDependsOn_Validation_Test()
    {
        var pipeline = new JobDependsOnErrorPipeline();
        Action action = () => pipeline.Validate();
        var e = action.Should().Throw<Exception>();
        e.WithMessage("*job_5*");
        e.WithMessage("*job_3*");
    }

    private class DuplicateNamePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
                {
                    new Stage("stage_1"),
                    new Stage("stage_1"),
                    new Stage("stage_3"),
                }
        };
    }

    [Fact]
    public void DuplicateName_Validation_Test()
    {
        var pipeline = new DuplicateNamePipeline();
        Action action = () => pipeline.Validate();
        var e = action.Should().Throw<Exception>();
        e.WithMessage("*stage_1*");
    }

    private class InvalidNamePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
                {
                    new Stage("stage_1"),
                    new Stage("stage_2_*&^"),
                }
        };
    }

    [Fact]
    public void InvalidName_Validation_Test()
    {
        var pipeline = new InvalidNamePipeline();
        Action action = () => pipeline.Validate();
        var e = action.Should().Throw<Exception>();
        e.WithMessage("*stage_2*");
    }
}
