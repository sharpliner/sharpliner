using System.Linq;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps
{
    public class ConditionalVariableTests
    {
        private class And_Condition_Test_Pipeline : TestPipeline
        {
            public override AzureDevOpsPipeline Pipeline => new()
            {
                Variables =
                {
                    If.And(
                        Equal(variables["Build.SourceBranch"], "'refs/heads/production'"),
                        NotEqual(variables["Configuration"], "'Debug'"))
                      .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)"),
                }
            };
        }

        [Fact]
        public void And_Condition_Test()
        {
            var pipeline = new And_Condition_Test_Pipeline();
            var variable = pipeline.Pipeline.Variables.First();
            variable.Condition.Should().Be(
                "and(eq(variables['Build.SourceBranch'], 'refs/heads/production'), ne(variables['Configuration'], 'Debug'))");
        }

        private class Or_Condition_Test_Pipeline : TestPipeline
        {
            public override AzureDevOpsPipeline Pipeline => new()
            {
                Variables =
                {
                    If.Or(
                        And(
                            NotEqual("true", "true"),
                            Equal(variables["Build.SourceBranch"], "'refs/heads/production'")),
                        NotEqual(variables["Configuration"], "'Debug'"))
                        .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)"),
                }
            };
        }

        [Fact]
        public void Or_Condition_Test()
        {
            var pipeline = new Or_Condition_Test_Pipeline();
            var variable = pipeline.Pipeline.Variables.First();
            variable.Condition.Should().Be(
                "or(and(ne(true, true), eq(variables['Build.SourceBranch'], 'refs/heads/production')), " +
                "ne(variables['Configuration'], 'Debug'))");
        }

        private class Branch_Condition_Test_Pipeline : TestPipeline
        {
            public override AzureDevOpsPipeline Pipeline => new()
            {
                Variables =
                {
                    If.BranchIs("refs/head/main")
                      .Variable("feature", "on"),

                    If.And(IsPullRequest, BranchIsNot("main"))
                      .Group("pr-group"),
                }
            };
        }

        [Fact]
        public void Branch_Condition_Test()
        {
            var pipeline = new Branch_Condition_Test_Pipeline();

            var variable1 = pipeline.Pipeline.Variables.ElementAt(0);
            var variable2 = pipeline.Pipeline.Variables.ElementAt(1);

            variable1.Condition.Should().Be("eq(variables['Build.SourceBranch'], refs/head/main)");
            variable2.Condition.Should().Be("and(eq(variables['Build.Reason'], PullRequest), ne(variables['Build.SourceBranch'], main))");
        }
    }
}
