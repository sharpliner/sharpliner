using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests.AzureDevOps.Expressions;

/// <summary>
/// Tests for nested conditional expressions to ensure proper nesting structure
/// that matches the C# structure in YAML output (issue #469)
/// </summary>
public class NestedConditionalsTests
{
    /// <summary>
    /// Test case for issue #469: chained If().If() should create nested conditional structures
    /// in YAML that match the C# structure
    /// </summary>
    private class ChainedIfTest_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .If.Equal("foo", "bar")
                        .Variable("test", "value"),
            }
        };
    }

    [Fact]
    public Task ChainedIf_ShouldCreateNestedStructure()
    {
        var pipeline = new ChainedIfTest_Pipeline();
        
        // This should generate nested conditionals:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - ${{ if eq('foo', 'bar') }}:
        //     - name: test
        //       value: value
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for If().If() with items in between - this should work already
    /// </summary>
    private class IfIfWithItemsBetweenTest_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("first", "value1")
                    .If.Equal("foo", "bar")
                        .Variable("second", "value2"),
            }
        };
    }

    [Fact]
    public Task IfIfWithItemsBetween_ShouldCreateSeparateBlocks()
    {
        var pipeline = new IfIfWithItemsBetweenTest_Pipeline();
        
        // This should generate separate blocks:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - name: first
        //     value: value1
        //   - ${{ if eq('foo', 'bar') }}:
        //     - name: second
        //       value: value2
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for triple nested If chains
    /// </summary>
    private class TripleNestedTest_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .If.Equal("env", "prod")
                        .If.Equal("region", "us-west")
                            .Variable("config", "prod-us-west"),
            }
        };
    }

    [Fact]
    public Task TripleNested_ShouldCreateTripleNestedStructure()
    {
        var pipeline = new TripleNestedTest_Pipeline();
        
        // This should generate triple nested conditionals:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - ${{ if eq('env', 'prod') }}:
        //     - ${{ if eq('region', 'us-west') }}:
        //       - name: config
        //         value: prod-us-west
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for nested If with EndIf followed by Else - Else should apply to outer If
    /// </summary>
    private class NestedIfEndIfElse_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("outer", "main-branch")
                    .If.Equal("env", "prod")
                        .Variable("inner", "prod-env")
                    .EndIf
                    .Variable("after-inner", "still-in-main")
                .Else
                    .Variable("outer", "not-main")
                    .Variable("fallback", "default"),
            }
        };
    }

    [Fact]
    public Task NestedIfEndIfElse_ShouldApplyElseToOuterIf()
    {
        var pipeline = new NestedIfEndIfElse_Pipeline();
        
        // This should generate:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - name: outer
        //     value: main-branch
        //   - ${{ if eq('env', 'prod') }}:
        //     - name: inner
        //       value: prod-env  
        //   - name: after-inner
        //     value: still-in-main
        // - ${{ else }}:
        //   - name: outer
        //     value: not-main
        //   - name: fallback
        //     value: default
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for nested If with Else inside, then EndIf
    /// </summary>
    private class NestedIfElseEndIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("outer", "main-branch")
                    .If.Equal("env", "prod")
                        .Variable("inner", "prod-env")
                    .Else
                        .Variable("inner", "non-prod-env")
                    .EndIf
                    .Variable("after-nested", "back-to-outer")
                    .Variable("still-outer", "yes"),
            }
        };
    }

    [Fact]
    public Task NestedIfElseEndIf_ShouldContainElseWithinNesting()
    {
        var pipeline = new NestedIfElseEndIf_Pipeline();
        
        // This should generate:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - name: outer
        //     value: main-branch
        //   - ${{ if eq('env', 'prod') }}:
        //     - name: inner
        //       value: prod-env
        //   - ${{ else }}:
        //     - name: inner
        //       value: non-prod-env
        //   - name: after-nested
        //     value: back-to-outer
        //   - name: still-outer
        //     value: yes
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for complex ElseIf chains with nesting
    /// </summary>
    private class ComplexElseIfNesting_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("branch-type", "main")
                    .If.Equal("deploy", "true")
                        .Variable("action", "deploy-main")
                    .EndIf
                .ElseIf.IsBranch("develop")
                    .Variable("branch-type", "develop")
                    .If.Equal("test", "true")
                        .Variable("action", "test-develop")
                    .Else  
                        .Variable("action", "skip-develop")
                    .EndIf
                .Else
                    .Variable("branch-type", "feature")
                    .Variable("action", "build-only"),
            }
        };
    }

    [Fact]
    public Task ComplexElseIfNesting_ShouldCreateProperBranchStructure()
    {
        var pipeline = new ComplexElseIfNesting_Pipeline();
        
        // This should generate proper ElseIf chain with nested conditions
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for multiple parallel If blocks after EndIf
    /// </summary>
    private class ParallelIfBlocksAfterEndIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("first", "main-branch")
                    .If.Equal("env", "prod")
                        .Variable("nested", "prod-config")
                    .EndIf
                    .Variable("second", "after-nested"),
                    
                // These should be separate parallel If blocks
                If.Equal("deploy", "true")
                    .Variable("deployment", "enabled"),
                    
                If.Contains("feature/", variables.Build.SourceBranch)
                    .Variable("feature-flag", "on"),
            }
        };
    }

    [Fact]
    public Task ParallelIfBlocksAfterEndIf_ShouldCreateSeparateBlocks()
    {
        var pipeline = new ParallelIfBlocksAfterEndIf_Pipeline();
        
        // This should generate separate If blocks at the top level
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for deep nesting with multiple strategic EndIf points
    /// </summary>
    private class DeepNestingWithMultipleEndIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("level1", "main")
                    .If.Equal("env", "prod")
                        .Variable("level2", "prod")
                        .If.Equal("region", "us")
                            .Variable("level3", "us")
                            .If.Equal("datacenter", "west")
                                .Variable("level4", "west")
                            .EndIf  // Ends level4 (datacenter)
                            .Variable("back-to-level3", "us-config")
                        .EndIf  // Ends level3 (region)
                        .Variable("back-to-level2", "prod-config")
                    .ElseIf.Equal("env", "staging")
                        .Variable("level2", "staging")  
                        .If.Equal("debug", "true")
                            .Variable("level3", "debug-on")
                        .Else
                            .Variable("level3", "debug-off")
                        .EndIf  // Ends level3 (debug)
                    .EndIf  // Ends level2 (env)
                    .Variable("back-to-level1", "final-main-config")
                .Else
                    .Variable("level1", "not-main"),
            }
        };
    }

    [Fact]
    public Task DeepNestingWithMultipleEndIf_ShouldMaintainProperStructure()
    {
        var pipeline = new DeepNestingWithMultipleEndIf_Pipeline();
        
        // This should generate a complex nested structure with proper EndIf termination points
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for mixed conditional patterns in variables
    /// </summary>
    private class MixedConditionalsInVariables_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("branch-setup", "main")
                    .If.Equal(parameters["Environment"], "Production")
                        .Variable("env-setup", "prod")
                        .Variable("deploy-mode", "production")
                    .Else
                        .Variable("env-setup", "non-prod")
                        .Variable("deploy-mode", "staging")
                    .EndIf
                    .Variable("finalize", "main-branch")
                .ElseIf.IsBranch("develop")
                    .Variable("branch-setup", "develop")  
                    .If.Equal(parameters["RunTests"], "true")
                        .Variable("tests", "enabled")
                    .EndIf
                .Else
                    .Variable("branch-setup", "feature"),
                    
                // Separate conditional block
                If.Equal(parameters["DeployAfterBuild"], "true")
                    .Variable("deployment", "enabled")
            }
        };
    }

    [Fact]
    public Task MixedConditionalsInVariables_ShouldWorkProperly()
    {
        var pipeline = new MixedConditionalsInVariables_Pipeline();
        
        // This tests complex conditional patterns within variables
        
        return Verify(pipeline.Serialize());
    }
}