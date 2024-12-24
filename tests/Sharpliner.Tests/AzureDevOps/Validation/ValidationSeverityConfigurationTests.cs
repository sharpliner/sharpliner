using Sharpliner.AzureDevOps;
using Sharpliner.Common;

namespace Sharpliner.Tests.AzureDevOps.Validation;

[CollectionDefinition(nameof(ValidationSeverityConfigurationTests), DisableParallelization = true)]
public class ValidationSeverityConfigurationTests
{
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
