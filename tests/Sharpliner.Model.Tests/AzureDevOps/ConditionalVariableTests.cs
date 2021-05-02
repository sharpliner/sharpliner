using System.Linq;
using FluentAssertions;
using Sharpliner.Model.AzureDevOps;
using Xunit;

namespace Sharpliner.Model.Tests.AzureDevOps
{
    public class PipelineSerializationTests
    {
        private class And_Condition_Test_Pipeline : AzureDevOpsPipelineDefinition
        {
            public override string TargetFile => "azure-pipelines.yml";

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

        private class Or_Condition_Test_Pipeline : AzureDevOpsPipelineDefinition
        {
            public override string TargetFile => "azure-pipelines.yml";

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
    }
}
