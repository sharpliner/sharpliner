using System.Linq;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps.Validation;

public class NameValidationTest
{
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
}
