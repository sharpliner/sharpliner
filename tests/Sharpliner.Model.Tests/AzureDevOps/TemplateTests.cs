using FluentAssertions;
using Sharpliner.Model.AzureDevOps;
using Sharpliner.Model.Definition;
using Xunit;

namespace Sharpliner.Model.Tests.AzureDevOps
{
    public class TemplateTests
    {
        [Fact]
        public void Template_Serialization_Test()
        {
            var condition = new EqualityCondition<Job>("foo", "bar", true);
            var template = new Template<Job>(condition.ToString(), "/eng/common/templates/jobs/jobs.yml")
            {
                Parameters =
                {
                    { "enableTelemetry", true },
                    { "sources", "src/*" },
                    { "nestedParameters", new TemplateParameters
                        {
                            { "enable", false },
                            { "continueOnError", false },
                        }
                    }
                }
            };

            var yaml = SharplinerSerializer.Serialize(template);

            yaml.Should().Be(
@"${{ if eq(foo, bar) }}:
- template: /eng/common/templates/jobs/jobs.yml
  parameters:
    enableTelemetry: true
    sources: src/*
    nestedParameters:
      enable: false
      continueOnError: false
");
        }
    }
}
