using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineValidationTests
{
    private class ConditionedDependsOnPipeline : TestPipeline
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
                    DependsOn =
                    {
                        If.IsBranch("main")
                            .Value("stage_1")
                        .Else
                            .Value("stage_2")
                    }
                },
            }
        };
    }

    [Fact]
    public void ConditionedDependsOn_Validation_Test()
    {
        var pipeline = new ConditionedDependsOnPipeline();
        var yaml = pipeline.Serialize();
        yaml.Should().Be(@"stages:
- stage: stage_1

- stage: stage_2
  dependsOn:
  - stage_1

- stage: stage_3
  dependsOn:
  - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    - stage_1

  - ${{ else }}:
    - stage_2
");
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
                    DependsOn = { "stage_1" }
                },
                new Stage("stage_3")
                {
                    DependsOn = { "foo" }
                },
            }
        };
    }

    [Fact]
    public void MissingStageDependsOn_Validation_Test()
    {
        var pipeline = new MissingStageDependsOnErrorPipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate());
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
        var errors = pipeline.Validations.SelectMany(v => v.Validate());
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
        var errors = pipeline.Validations.SelectMany(v => v.Validate());
        Assert.Single(errors);
        Assert.Equal("Found duplicate stage name `stage_1`", errors.Single().Message);
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
                        Job("job_1"),
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
        var errors = pipeline.Validations.SelectMany(v => v.Validate());
        Assert.Single(errors);
        Assert.Equal("Found duplicate job name `job_4`", errors.Single().Message);
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
        var errors = pipeline.Validations.SelectMany(v => v.Validate());
        Assert.Single(errors);
        Assert.Contains("stage_2", errors.Single().Message);
    }
}
