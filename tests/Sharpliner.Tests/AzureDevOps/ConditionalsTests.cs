using System.Linq;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps
{
    public class ConditionalsTests
    {
        private class And_Condition_Test_Pipeline : TestPipeline
        {
            public override Pipeline Pipeline => new()
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
            public override Pipeline Pipeline => new()
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
            public override Pipeline Pipeline => new()
            {
                Variables =
                {
                    If.IsBranch("main")
                      .Variable("feature", "on"),

                    If.And(IsPullRequest, IsNotBranch("main"))
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

            variable1.Condition.Should().Be("eq(variables['Build.SourceBranch'], 'refs/heads/main')");
            variable2.Condition.Should().Be("and(eq(variables['Build.Reason'], 'PullRequest'), ne(variables['Build.SourceBranch'], 'refs/heads/main'))");
        }

        private class Else_Test_Pipeline : TestPipeline
        {
            public override Pipeline Pipeline => new()
            {
                Variables =
                {
                    If.Equal("a", "b")
                        .Variable("feature", "on")
                        .Variable("feature2", "on")
                    .Else
                        .Variable("feature", "off")
                        .Variable("feature2", "off"),

                    If.NotEqual("c", "d")
                      .Variable("feature", "on")
                      .Variable("feature2", "on")
                      .If.And(Equal("e", "f"), NotEqual("g", "h"))
                          .Variable("feature", "on")
                          .Variable("feature2", "on")
                      .Else
                          .Variable("feature", "off")
                          .Variable("feature2", "off"),
                }
            };
        }

        [Fact]
        public void Else_Test()
        {
            var pipeline = new Else_Test_Pipeline();
            pipeline.Serialize().Should().Be(
@"variables:
- ${{ if eq(a, b) }}:
  - name: feature
    value: on

  - name: feature2
    value: on

- ${{ if ne(a, b) }}:
  - name: feature
    value: off

  - name: feature2
    value: off

- ${{ if ne(c, d) }}:
  - name: feature
    value: on

  - name: feature2
    value: on

  - ${{ if and(eq(e, f), ne(g, h)) }}:
    - name: feature
      value: on

    - name: feature2
      value: on

  - ${{ if not(and(eq(e, f), ne(g, h))) }}:
    - name: feature
      value: off

    - name: feature2
      value: off
");
        }
    }
}
