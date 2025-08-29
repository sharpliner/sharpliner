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
                ObjectParameter<string>("list", "List input", ["Azure" , "DevOps"]),
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
}
