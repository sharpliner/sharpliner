using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps
{
    public class TemplateTests
    {
        private class Template_Pipeline : SimpleTestPipeline
        {
            public override SingleStagePipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("job")
                    {
                        Pool =
                            If.Equal("foo", "bar")
                                .Template<Pool>("pool1.yml")
                            .Else
                                .Template<Pool>("pool2.yml")
                    }
                }
            };
        }

        [Fact]
        public void Template_Serialization_Test()
        {
            var yaml = new Template_Pipeline().Serialize();

            yaml.Should().Be(
@"jobs:
- job: job
  pool:
    ${{ if eq(foo, bar) }}:
      template: pool1.yml
    ${{ if ne(foo, bar) }}:
      template: pool2.yml
");
        }

        private class TemplateList_Pipeline : SimpleTestPipeline
        {
            public override SingleStagePipeline Pipeline => new()
            {
                Jobs =
                {
                    If.Equal("foo", "bar")
                        .Template<Job>("template1.yml", new TemplateParameters
                        {
                            { "enableTelemetry", true },
                        })
                        .Template<Job>("template2.yml", new TemplateParameters
                        {
                            { "enableTelemetry", false },
                        })
                }
            };
        }

        [Fact]
        public void TemplateList_Serialization_Test()
        {
            var yaml = new TemplateList_Pipeline().Serialize();

            yaml.Should().Be(
@"jobs:
- ${{ if eq(foo, bar) }}:
  - template: template1.yml
    parameters:
      enableTelemetry: true

  - template: template2.yml
    parameters:
      enableTelemetry: false
");
        }
    }
}
