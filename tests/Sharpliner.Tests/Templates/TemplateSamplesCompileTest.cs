namespace SharplinerPipelineProject;

public class TemplateSamplesCompileTest
{
    [Fact]
    public Task Sample_Pipeline_Generates_Correct_Yaml_Test()
    {
        var pipeline = new SamplePipeline();
        return Verify(pipeline.Serialize());
    }
}
