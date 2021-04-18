using FluentAssertions;
using Sharpliner.Model.AzureDevOps;
using Xunit;

namespace Sharpliner.Model.Tests.AzureDevOps
{
    public class PipelineSerializationTests
    {
        [Fact]
        public void And_Condition_Test()
        {
            var builder = new VariableDefinitionConditionBuilder();
            var condition = builder.And(
                new EqualityVariableDefinitionCondition("variables['Build.SourceBranch']", "'refs/heads/production'", true),
                new EqualityVariableDefinitionCondition("variables['Configuration']", "'Debug'", false));

            ConditionedVariableDefinition def = condition.Variable("foo", "bar");
            def.Condition.Should().Be("and(eq(variables['Build.SourceBranch'], 'refs/heads/production'), ne(variables['Configuration'], 'Debug'))");
        }

        [Fact]
        public void Or_Condition_Test()
        {
            var builder = new VariableDefinitionConditionBuilder();
            var condition = builder.Or(
                new AndVariableDefinitionCondition(
                    new EqualityVariableDefinitionCondition("true", "true", false),
                    new EqualityVariableDefinitionCondition("variables['Build.SourceBranch']", "'refs/heads/production'", true)),
                new EqualityVariableDefinitionCondition("variables['Configuration']", "'Debug'", false));

            ConditionedVariableDefinition def = condition.Variable("foo", "bar");
            def.Condition.Should().Be("or(and(ne(true, true), eq(variables['Build.SourceBranch'], 'refs/heads/production')), ne(variables['Configuration'], 'Debug'))");
        }
    }
}
