using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests.AzureDevOps.Expressions;

/// <summary>
/// Comprehensive tests for the nested conditional expressions fix for issue #287
/// </summary>
public class ComprehensiveNestedConditionalsTests
{
    /// <summary>
    /// Test 1: Basic If().If() chaining (the main issue) - should merge with and()
    /// </summary>
    private class BasicChainedIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .If.Equal("foo", "bar")
                        .Variable("merged", "success"),
            }
        };
    }

    [Fact]
    public Task BasicChainedIf_ShouldMergeConditions()
    {
        var pipeline = new BasicChainedIf_Pipeline();
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test 2: Triple chaining If().If().If() - should merge all three with and()
    /// </summary>
    private class TripleChainedIf_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .If.Equal("env", "prod") 
                    .If.Equal("region", "us-west")
                        .Variable("config", "prod-us-west-main"),
            }
        };
    }

    [Fact]
    public Task TripleChainedIf_ShouldMergeAllConditions()
    {
        var pipeline = new TripleChainedIf_Pipeline();
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test 3: Existing working pattern (with Variable in between) - should create nested blocks
    /// </summary>
    private class ExistingNestedPattern_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Variable("first", "value1")
                    .If.Equal("env", "prod")
                        .Variable("second", "value2"),
            }
        };
    }

    [Fact]
    public Task ExistingNestedPattern_ShouldCreateSeparateBlocks()
    {
        var pipeline = new ExistingNestedPattern_Pipeline();
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test 4: Mixed pattern - merge then nest
    /// </summary>
    private class MixedPattern_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .If.Equal("env", "prod")           // Should merge with first If
                        .Variable("config", "merged")   // Materializes the merged condition
                        .If.Equal("region", "us")       // Should create nested block
                            .Variable("region-config", "nested"),
            }
        };
    }

    [Fact]
    public Task MixedPattern_ShouldMergeThenNest()
    {
        var pipeline = new MixedPattern_Pipeline();
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test 5: Multiple separate chains in same collection
    /// </summary>
    private class MultipleSeparateChains_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .If.Equal("foo", "bar")
                        .Variable("first-chain", "merged1"),
                        
                If.IsBranch("dev")
                    .If.Equal("baz", "qux")
                        .Variable("second-chain", "merged2"),
            }
        };
    }

    [Fact]
    public Task MultipleSeparateChains_ShouldBothWork()
    {
        var pipeline = new MultipleSeparateChains_Pipeline();
        return Verify(pipeline.Serialize());
    }
}