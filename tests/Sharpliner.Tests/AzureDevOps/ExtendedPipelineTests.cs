using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class ExtendedPipelineTests
{
    private class Extended_Pipeline : ExtendsPipelineDefinition<PipelineWithExtends>
    {
        public override string TargetFile => "file.yml";

        public override PipelineWithExtends Pipeline => new()
        {
            Variables = 
            [
                Variable("key1", "value1"),
            ],
            Extends = new("templates/pipeline-template.yml", new()
            {
                ["param1"] = "value1",
                ["param2"] = false,
                ["param3"] = variables["key1"], // TODO: see https://github.com/sharpliner/sharpliner/issues/375
            }),

            Trigger = Trigger.None,
            Pool = new HostedPool(vmImage: "ubuntu-latest"),
        };
    }

    [Fact]
    public void TemplateList_Serialization_Test()
    {
        var yaml = new Extended_Pipeline().Serialize();

        yaml.Trim().Should().Be(
            """
            trigger: none

            variables:
            - name: key1
              value: value1

            pool:
              vmImage: ubuntu-latest

            extends:
              template: templates/pipeline-template.yml
              parameters:
                param1: value1
                param2: false
                param3: $(key1)
            """);
    }
}
