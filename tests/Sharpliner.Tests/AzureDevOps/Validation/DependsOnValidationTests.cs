using Sharpliner.AzureDevOps;
using Sharpliner.Common;

namespace Sharpliner.Tests.AzureDevOps.Validation;

public class DependsOnValidationTests
{
    private class ConditionedDependsOnPipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Parameters = 
            {
                StringParameter("stageDependsOn", defaultValue: string.Empty) ,
                StringParameter("jobDependsOn", defaultValue: string.Empty)
            },
            Stages =
            {
                new Stage("stage_1"),
                new Stage("stage_2")
                {
                    DependsOn = "stage_1"
                },
                new Stage("stage_3")
                {
                    DependsOn =
                    {
                        If.IsBranch("main")
                            .Value("stage_1")
                        .Else
                            .Value("stage_2")
                    }
                },
                new Stage("stage_4")
                {
                    DependsOn = parameters["stageDependsOn"],
                    Jobs = 
                    {
                        new Job("job_1"),
                        new Job("job_2")
                        {
                            DependsOn = "job_1"
                        },
                        new Job("job_3")
                        {
                            DependsOn = 
                            {
                                If.IsBranch("main")
                                    .Value("job_1")
                                .Else
                                    .Value("job_2")
                            }
                        },
                        new Job("job_4")
                        {
                            DependsOn = parameters["jobDependsOn"]
                        }
                    }
                },
            },
        };
    }

    [Fact]
    public Task ConditionedDependsOn_Validation_Test()
    {
        var pipeline = new ConditionedDependsOnPipeline();

        return Verify(pipeline.Serialize());
    }

    private class MissingStageDependsOnErrorPipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                new Stage("stage_1"),
                new Stage("stage_2")
                {
                    DependsOn = "stage_1"
                },
                new Stage("stage_3")
                {
                    DependsOn = "foo"
                },
            }
        };
    }

    [Fact]
    public void MissingStageDependsOn_Validation_Test()
    {
        var pipeline = new MissingStageDependsOnErrorPipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Equal("Stage `stage_3` depends on stage `foo` which was not found", errors.Single().Message);
    }

    private class MissingJobDependsOnErrorPipeline : TestPipeline
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

    [Fact]
    public void MissingJobDependsOn_Validation_Test()
    {
        var pipeline = new MissingJobDependsOnErrorPipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Equal("Job `job_5` depends on job `job_3` which was not found", errors.Single().Message);
    }

    private class DuplicateStageNamePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                new Stage("stage_1")
                {
                    Jobs =
                    {
                        Job("job_1")
                    }
                },
                new Stage("stage_1"),
                new Stage("stage_3")
                {
                    Jobs =
                    {
                        Job("job_1")
                    }
                },
            }
        };
    }

    [Fact]
    public void DuplicateStageName_Validation_Test()
    {
        var pipeline = new DuplicateStageNamePipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Equal("Found duplicate name `stage_1`", errors.Single().Message);
    }

    private class DuplicateJobNamePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                new Stage("stage_1")
                {
                    Jobs =
                    {
                        Job("job_1"),
                        Job("job_2"),
                        Job("job_3"),
                    }
                },
                new Stage("stage_2"),
                new Stage("stage_3")
                {
                    Jobs =
                    {
                        Job("stage_3"), // this should not conflict with the stage
                        Job("job_4"),
                        Job("job_4"),
                    }
                },
            }
        };
    }

    [Fact]
    public void DuplicateJobName_Validation_Test()
    {
        var pipeline = new DuplicateJobNamePipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Equal("Found duplicate name `job_4`", errors.Single().Message);
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
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Contains("stage_2", errors.Single().Message);
    }

    private class DuplicateJobNameSimplePipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                Job("job_1"),
                Job("job_4"),
                Job("job_4"),
            }
        };
    }

    [Fact]
    public void DuplicateJobNameSingleStage_Validation_Test()
    {
        var pipeline = new DuplicateJobNameSimplePipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Equal("Found duplicate name `job_4`", errors.Single().Message);
    }

    private class SelfDependencyPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job_1"),
                new Job("job_2")
                {
                    DependsOn = { "job_1" }
                },
                new Job("job_3")
                {
                    DependsOn = { "job_2", "job_3" }
                },
            }
        };
    }

    [Fact]
    public void SelfDependency_Validation_Test()
    {
        var pipeline = new SelfDependencyPipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        Assert.Single(errors);
        Assert.Equal("Job `job_3` depends on itself", errors.Single().Message);
    }

    [Fact]
    public void ValidationLevelMatchesConfiguration_Validation_Test()
    {
        var pipeline = new SelfDependencyPipeline();
        SharplinerConfiguration.Current.Validations.DependsOnFields = ValidationSeverity.Warning;
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        SharplinerConfiguration.Current.Validations.DependsOnFields = ValidationSeverity.Error;
        Assert.Single(errors);
        Assert.Equal(ValidationSeverity.Warning, errors.Single().Severity);
    }

    [Fact]
    public void ValidationIsTurnedOff_Validation_Test()
    {
        var pipeline = new SelfDependencyPipeline();
        SharplinerConfiguration.Current.Validations.DependsOnFields = ValidationSeverity.Off;
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        SharplinerConfiguration.Current.Validations.DependsOnFields = ValidationSeverity.Error;
        Assert.Empty(errors);
    }
}
