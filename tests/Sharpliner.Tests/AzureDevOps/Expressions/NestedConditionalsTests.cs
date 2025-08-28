using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests.AzureDevOps.Expressions;

/// <summary>
/// Tests for nested conditional expressions to demonstrate and fix the issues in #287
/// </summary>
public class NestedConditionalsTests
{
    /// <summary>
    /// Test case for issue #287: chained If().If() should merge conditions with 'and()'
    /// when no items have been added to the current block
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
    public Task ChainedIf_ShouldMergeConditions()
    {
        var pipeline = new ChainedIfTest_Pipeline();
        
        // This should generate:
        // - ${{ if and(eq(variables['Build.SourceBranch'], 'refs/heads/main'), eq('foo', 'bar')) }}:
        //   - name: test
        //     value: value
        
        return Verify(pipeline.Serialize());
    }

    /// <summary>
    /// Test case for nested If with Each
    /// </summary>
    private class IfWithEachTest_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.IsBranch("main")
                    .Each("item", new[] { "val1", "val2" })
                        .Variable("test-${{ item }}", "value-${{ item }}"),
            }
        };
    }

    [Fact]
    public Task IfWithEach_ShouldCreateNestedBlocks()
    {
        var pipeline = new IfWithEachTest_Pipeline();
        
        // This should generate:
        // - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
        //   - ${{ each item in ['val1', 'val2'] }}:
        //     - name: test-${{ item }}
        //       value: value-${{ item }}
        
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
    /// Test case for deeply nested conditions
    /// </summary>
    private class DeeplyNestedTest_Pipeline : TestPipeline
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
    public Task DeeplyNested_ShouldMergeAllConditions()
    {
        var pipeline = new DeeplyNestedTest_Pipeline();
        
        // This should generate:
        // - ${{ if and(eq(variables['Build.SourceBranch'], 'refs/heads/main'), eq('env', 'prod'), eq('region', 'us-west')) }}:
        //   - name: config
        //     value: prod-us-west
        
        return Verify(pipeline.Serialize());
    }
}