using Sharpliner.AzureDevOps;

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
    public Task TemplateList_Serialization_Test()
    {
        var pipeline = new Extended_Pipeline().Serialize();

        return Verify(pipeline);
    }
}
