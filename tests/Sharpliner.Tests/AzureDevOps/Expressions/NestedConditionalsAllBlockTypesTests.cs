using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps.Expressions;

/// <summary>
/// Tests for nested conditional expressions working across all block types (Stages, Jobs, Steps, etc.)
/// This validates that the fix in AdoExpression.Link works universally, not just for Variables.
/// </summary>
public class NestedConditionalsAllBlockTypesTests
{
    /// <summary>
    /// Test nested If().If() pattern on Stages
    /// </summary>
    private class NestedIfStages_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                If.IsBranch("main")
                    .If.Equal("env", "prod")
                        .Stage(new Stage("deploy-prod")
                        {
                            DisplayName = "Deploy to Production"
                        }),
            }
        };
    }

    [Fact]
    public Task NestedIfStages_ShouldCreateNestedStructure()
    {
        var pipeline = new NestedIfStages_Pipeline();
        
        // This should generate nested conditionals:
        // stages:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - ${{ if eq('env', 'prod') }}:
        //     - stage: deploy-prod
        //       displayName: Deploy to Production
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test nested If().If() pattern on Jobs
    /// </summary>
    private class NestedIfJobs_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                If.IsBranch("main")
                    .If.Equal("deploy", "true")
                        .Job(new Job("deploy-job")
                        {
                            DisplayName = "Deploy Job",
                            Pool = new HostedPool("ubuntu-latest")
                        }),
            }
        };
    }

    [Fact]
    public Task NestedIfJobs_ShouldCreateNestedStructure()
    {
        var pipeline = new NestedIfJobs_Pipeline();
        
        // This should generate nested conditionals in jobs section
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test nested If().If() pattern on Steps
    /// </summary>
    private class NestedIfSteps_Pipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps => new()
        {
            If.IsBranch("main")
                .If.Equal("runTests", "true")
                    .Step(new InlineBashTask("echo 'Running tests'")
                    {
                        DisplayName = "Run Tests"
                    }),
        };
    }

    [Fact]
    public Task NestedIfSteps_ShouldCreateNestedStructure()
    {
        var pipeline = new NestedIfSteps_Pipeline();
        
        // This should generate nested conditionals in steps section
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test nested If().If() with EndIf on Stages
    /// </summary>
    private class NestedIfStagesWithEndIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                If.IsBranch("main")
                    .Stage(new Stage("build-main")
                    {
                        DisplayName = "Build Main"
                    })
                    .If.Equal("env", "prod")
                        .Stage(new Stage("deploy-prod")
                        {
                            DisplayName = "Deploy Prod"
                        })
                    .EndIf
                    .Stage(new Stage("finalize")
                    {
                        DisplayName = "Finalize"
                    }),
            }
        };
    }

    [Fact]
    public Task NestedIfStagesWithEndIf_ShouldCreateNestedStructure()
    {
        var pipeline = new NestedIfStagesWithEndIf_Pipeline();
        
        // This should generate:
        // stages:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - stage: build-main
        //     displayName: Build Main
        //   - ${{ if eq('env', 'prod') }}:
        //     - stage: deploy-prod
        //       displayName: Deploy Prod
        //   - stage: finalize
        //     displayName: Finalize
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test triple nested If chaining on Jobs
    /// </summary>
    private class TripleNestedIfJobs_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                If.IsBranch("main")
                    .If.Equal("env", "prod")
                        .If.Equal("region", "us-west")
                            .Job(new Job("deploy-us-west")
                            {
                                DisplayName = "Deploy US West",
                                Pool = new HostedPool("ubuntu-latest")
                            }),
            }
        };
    }

    [Fact]
    public Task TripleNestedIfJobs_ShouldCreateTripleNestedStructure()
    {
        var pipeline = new TripleNestedIfJobs_Pipeline();
        
        // This should generate triple nested conditionals
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test nested If with Else on Steps
    /// </summary>
    private class NestedIfStepsWithElse_Pipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps => new()
        {
            If.IsBranch("main")
                .If.Equal("fastTests", "true")
                    .Step(new InlineBashTask("echo 'Fast tests'")
                    {
                        DisplayName = "Fast Tests"
                    })
                .Else
                    .Step(new InlineBashTask("echo 'All tests'")
                    {
                        DisplayName = "All Tests"
                    })
                .EndIf
                .Step(new InlineBashTask("echo 'Done'")
                {
                    DisplayName = "Done"
                }),
        };
    }

    [Fact]
    public Task NestedIfStepsWithElse_ShouldCreateNestedStructure()
    {
        var pipeline = new NestedIfStepsWithElse_Pipeline();
        
        // This should generate nested If/Else structure within steps
        
        return Verify(pipeline.Serialize());
    }
}
