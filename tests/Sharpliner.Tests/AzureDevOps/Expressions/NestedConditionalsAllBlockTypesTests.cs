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

    /// <summary>
    /// Test case for nested If with EndIf followed by Else on Stages - Else should apply to outer If
    /// </summary>
    private class NestedStagesIfEndIfElse_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                If.IsBranch("main")
                    .Stage(new Stage("outer-stage")
                    {
                        DisplayName = "Main Branch Stage"
                    })
                    .If.Equal("env", "prod")
                        .Stage(new Stage("inner-stage")
                        {
                            DisplayName = "Production Environment"
                        })
                    .EndIf
                    .Stage(new Stage("after-inner")
                    {
                        DisplayName = "Still in Main"
                    })
                .Else
                    .Stage(new Stage("fallback-stage")
                    {
                        DisplayName = "Not Main Branch"
                    }),
            }
        };
    }

    [Fact]
    public Task NestedStagesIfEndIfElse_ShouldApplyElseToOuterIf()
    {
        var pipeline = new NestedStagesIfEndIfElse_Pipeline();
        
        // This should generate:
        // stages:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - stage: outer-stage
        //     displayName: Main Branch Stage
        //   - ${{ if eq('env', 'prod') }}:
        //     - stage: inner-stage
        //       displayName: Production Environment
        //   - stage: after-inner
        //     displayName: Still in Main
        // - ${{ else }}:
        //   - stage: fallback-stage
        //     displayName: Not Main Branch
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for complex ElseIf chains with nesting on Jobs
    /// </summary>
    private class ComplexJobsElseIfNesting_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                If.IsBranch("main")
                    .Job(new Job("main-job")
                    {
                        DisplayName = "Main Branch Job",
                        Pool = new HostedPool("ubuntu-latest")
                    })
                    .If.Equal("deploy", "true")
                        .Job(new Job("deploy-main")
                        {
                            DisplayName = "Deploy Main",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                    .EndIf
                .ElseIf.IsBranch("develop")
                    .Job(new Job("develop-job")
                    {
                        DisplayName = "Develop Branch Job",
                        Pool = new HostedPool("ubuntu-latest")
                    })
                    .If.Equal("test", "true")
                        .Job(new Job("test-develop")
                        {
                            DisplayName = "Test Develop",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                    .Else
                        .Job(new Job("skip-develop")
                        {
                            DisplayName = "Skip Develop Tests",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                    .EndIf
                .Else
                    .Job(new Job("feature-job")
                    {
                        DisplayName = "Feature Branch Job",
                        Pool = new HostedPool("ubuntu-latest")
                    }),
            }
        };
    }

    [Fact]
    public Task ComplexJobsElseIfNesting_ShouldCreateProperBranchStructure()
    {
        var pipeline = new ComplexJobsElseIfNesting_Pipeline();
        
        // This should generate proper ElseIf chain with nested conditions for jobs
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for multiple parallel Stage blocks after EndIf
    /// </summary>
    private class ParallelStagesAfterEndIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                If.IsBranch("main")
                    .Stage(new Stage("first-stage")
                    {
                        DisplayName = "First Stage"
                    })
                    .If.Equal("env", "prod")
                        .Stage(new Stage("nested-stage")
                        {
                            DisplayName = "Nested Stage"
                        })
                    .EndIf
                    .Stage(new Stage("second-stage")
                    {
                        DisplayName = "Second Stage"
                    }),
                    
                // These should be separate parallel Stage blocks
                If.Equal("deploy", "true")
                    .Stage(new Stage("deploy-stage")
                    {
                        DisplayName = "Deployment Stage"
                    }),
                    
                If.Contains("feature/", variables.Build.SourceBranch)
                    .Stage(new Stage("feature-stage")
                    {
                        DisplayName = "Feature Stage"
                    }),
            }
        };
    }

    [Fact]
    public Task ParallelStagesAfterEndIf_ShouldCreateSeparateBlocks()
    {
        var pipeline = new ParallelStagesAfterEndIf_Pipeline();
        
        // This should generate separate Stage blocks at the top level
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for deep nesting with multiple strategic EndIf points on Jobs
    /// </summary>
    private class DeepJobsNestingWithMultipleEndIf_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                If.IsBranch("main")
                    .Job(new Job("level1-job")
                    {
                        DisplayName = "Level 1 - Main",
                        Pool = new HostedPool("ubuntu-latest")
                    })
                    .If.Equal("env", "prod")
                        .Job(new Job("level2-job")
                        {
                            DisplayName = "Level 2 - Prod",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                        .If.Equal("region", "us")
                            .Job(new Job("level3-job")
                            {
                                DisplayName = "Level 3 - US",
                                Pool = new HostedPool("ubuntu-latest")
                            })
                            .If.Equal("datacenter", "west")
                                .Job(new Job("level4-job")
                                {
                                    DisplayName = "Level 4 - West",
                                    Pool = new HostedPool("ubuntu-latest")
                                })
                            .EndIf  // Ends level4 (datacenter)
                            .Job(new Job("back-to-level3")
                            {
                                DisplayName = "Back to Level 3",
                                Pool = new HostedPool("ubuntu-latest")
                            })
                        .EndIf  // Ends level3 (region)
                        .Job(new Job("back-to-level2")
                        {
                            DisplayName = "Back to Level 2",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                    .ElseIf.Equal("env", "staging")
                        .Job(new Job("level2-staging")
                        {
                            DisplayName = "Level 2 - Staging",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                        .If.Equal("debug", "true")
                            .Job(new Job("level3-debug")
                            {
                                DisplayName = "Level 3 - Debug On",
                                Pool = new HostedPool("ubuntu-latest")
                            })
                        .Else
                            .Job(new Job("level3-no-debug")
                            {
                                DisplayName = "Level 3 - Debug Off",
                                Pool = new HostedPool("ubuntu-latest")
                            })
                        .EndIf  // Ends level3 (debug)
                    .EndIf  // Ends level2 (env)
                    .Job(new Job("back-to-level1")
                    {
                        DisplayName = "Back to Level 1",
                        Pool = new HostedPool("ubuntu-latest")
                    })
                .Else
                    .Job(new Job("not-main")
                    {
                        DisplayName = "Not Main Branch",
                        Pool = new HostedPool("ubuntu-latest")
                    }),
            }
        };
    }

    [Fact]
    public Task DeepJobsNestingWithMultipleEndIf_ShouldMaintainProperStructure()
    {
        var pipeline = new DeepJobsNestingWithMultipleEndIf_Pipeline();
        
        // This should generate a complex nested structure with proper EndIf termination points for jobs
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for nested If with Else inside, then EndIf on Steps
    /// </summary>
    private class NestedStepsIfElseEndIf_Pipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps => new()
        {
            If.IsBranch("main")
                .Step(new InlineBashTask("echo 'Outer step'")
                {
                    DisplayName = "Main Branch Step"
                })
                .If.Equal("env", "prod")
                    .Step(new InlineBashTask("echo 'Inner prod step'")
                    {
                        DisplayName = "Production Step"
                    })
                .Else
                    .Step(new InlineBashTask("echo 'Inner non-prod step'")
                    {
                        DisplayName = "Non-Production Step"
                    })
                .EndIf
                .Step(new InlineBashTask("echo 'Back to outer'")
                {
                    DisplayName = "Back to Main Context"
                })
                .Step(new InlineBashTask("echo 'Still in main'")
                {
                    DisplayName = "Still in Main Branch"
                }),
        };
    }

    [Fact]
    public Task NestedStepsIfElseEndIf_ShouldContainElseWithinNesting()
    {
        var pipeline = new NestedStepsIfElseEndIf_Pipeline();
        
        // This should generate:
        // steps:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - bash: echo 'Outer step'
        //     displayName: Main Branch Step
        //   - ${{ if eq('env', 'prod') }}:
        //     - bash: echo 'Inner prod step'
        //       displayName: Production Step
        //   - ${{ else }}:
        //     - bash: echo 'Inner non-prod step'
        //       displayName: Non-Production Step
        //   - bash: echo 'Back to outer'
        //     displayName: Back to Main Context
        //   - bash: echo 'Still in main'
        //     displayName: Still in Main Branch
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test nested If().If() with Each expression on Stages
    /// </summary>
    private class NestedIfWithEachStages_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                If.IsBranch("main")
                    .If.Equal("deploy", "true")
                        .Each("env", "parameters.environments")
                            .Stage(new Stage("deploy-${{ env }}")
                            {
                                DisplayName = "Deploy to ${{ env }}"
                            })
                        .EndEach,
            }
        };
    }

    [Fact]
    public Task NestedIfWithEachStages_ShouldCreateNestedStructure()
    {
        var pipeline = new NestedIfWithEachStages_Pipeline();
        
        // This should generate nested conditionals with each:
        // stages:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - ${{ if eq('deploy', true) }}:
        //     - ${{ each env in parameters.environments }}:
        //       - stage: deploy-${{ env }}
        //         displayName: Deploy to ${{ env }}
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test Each with nested If inside on Jobs
    /// </summary>
    private class EachWithNestedIfJobs_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                Each("region", "parameters.regions")
                    .If.Equal("region.enabled", "true")
                        .Job(new Job("deploy-${{ region.name }}")
                        {
                            DisplayName = "Deploy to ${{ region.name }}",
                            Pool = new HostedPool("ubuntu-latest")
                        })
                    .EndIf,
            }
        };
    }

    [Fact]
    public Task EachWithNestedIfJobs_ShouldCreateNestedStructure()
    {
        var pipeline = new EachWithNestedIfJobs_Pipeline();
        
        // This should generate each with nested if:
        // jobs:
        // - ${{ each region in parameters.regions }}:
        //   - ${{ if eq('region.enabled', true) }}:
        //     - job: deploy-${{ region.name }}
        //       displayName: Deploy to ${{ region.name }}
        //       pool:
        //         name: ubuntu-latest
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test complex nesting: If with Each, and nested If inside Each
    /// </summary>
    private class ComplexIfEachNesting_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                If.IsBranch("main")
                    .Each("env", "parameters.environments")
                        .If.Equal("env.deploy", "true")
                            .Stage(new Stage("deploy-${{ env.name }}")
                            {
                                DisplayName = "Deploy to ${{ env.name }}"
                            })
                        .EndIf
                    .EndEach,
            }
        };
    }

    [Fact]
    public Task ComplexIfEachNesting_ShouldCreateProperStructure()
    {
        var pipeline = new ComplexIfEachNesting_Pipeline();
        
        // This should generate complex nested structure:
        // stages:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - ${{ each env in parameters.environments }}:
        //     - ${{ if eq('env.deploy', true) }}:
        //       - stage: deploy-${{ env.name }}
        //         displayName: Deploy to ${{ env.name }}
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test Each with multiple nested Ifs on Steps
    /// </summary>
    private class EachWithMultipleNestedIfSteps_Pipeline : SimpleStepTestPipeline
    {
        protected override AdoExpressionList<Step> Steps => new()
        {
            Each("config", "parameters.configurations")
                .If.Equal("config.enabled", "true")
                    .If.Equal("config.test", "true")
                        .Step(new InlineBashTask("echo 'Testing ${{ config.name }}'")
                        {
                            DisplayName = "Test ${{ config.name }}"
                        })
                    .EndIf
                    .Step(new InlineBashTask("echo 'Processing ${{ config.name }}'")
                    {
                        DisplayName = "Process ${{ config.name }}"
                    })
                .EndIf
            .EndEach,
        };
    }

    [Fact]
    public Task EachWithMultipleNestedIfSteps_ShouldCreateProperStructure()
    {
        var pipeline = new EachWithMultipleNestedIfSteps_Pipeline();
        
        // This should generate each with multiple nested ifs:
        // steps:
        // - ${{ each config in parameters.configurations }}:
        //   - ${{ if eq('config.enabled', true) }}:
        //     - ${{ if eq('config.test', true) }}:
        //       - bash: echo 'Testing ${{ config.name }}'
        //         displayName: Test ${{ config.name }}
        //     - bash: echo 'Processing ${{ config.name }}'
        //       displayName: Process ${{ config.name }}
        
        return Verify(pipeline.Serialize());
    }
}
