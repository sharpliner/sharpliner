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
}