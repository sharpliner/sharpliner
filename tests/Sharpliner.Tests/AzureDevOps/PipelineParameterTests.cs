using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineParameterTests
{
    private class PipelineParameterTests_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Parameters =
            {
                StringParameter("project", "AzureDevops project"),
                StringParameter("version", ".NET version", allowedValues: [ "5.0.100", "5.0.102" ]),
                BooleanParameter("restore", "Restore NuGets", defaultValue: true),
                StringListParameter("list", "List input", ["Azure" , "DevOps"]),
                ObjectParameter("object", "Object input", new TemplateParameters
                {
                    { "key1", "value1" },
                    { "key2", 2 },
                    { "key3", true },
                    { "key4", new[] {1, 2, 3 } },
                    { "key5", new Dictionary<string, object?> { { "subkey1", "subvalue1" } } }
                }),
                StepParameter("afterBuild", "After steps", Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)")),
                StringListParameter("environments", "Target environments", ["dev", "staging", "production"]),
            }
        };
    }

    [Fact]
    public Task PipelineParameter_Serialization_Test()
    {
        var pipeline = new PipelineParameterTests_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class StringListParameterTests_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Parameters =
            {
                StringListParameter("tags", "Build tags"),
                StringListParameter("configs", "Configuration list", ["Debug", "Release"]),
                StringListParameter("platforms", "Platform list", defaultValue: ["x64", "ARM64"]),
            }
        };
    }

    [Fact]
    public Task StringListParameter_Serialization_Test()
    {
        var pipeline = new StringListParameterTests_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class StringListParameterWithAllowedValues_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Parameters =
            {
                StringListParameter("environments", "Target environments", 
                    defaultValue: ["dev"],
                    allowedValues: ["dev", "staging", "production"]),
                StringListParameter("configs", "Build configurations",
                    allowedValues: ["Debug", "Release", "MinSizeRel"]),
            }
        };
    }

    [Fact]
    public Task StringListParameter_With_AllowedValues_Test()
    {
        var pipeline = new StringListParameterWithAllowedValues_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
