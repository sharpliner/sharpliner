using Xunit;

namespace Sharpliner.Serialization.Tests
{
    public class PipelineSerializationTests
    {
        [Fact]
        public void Serialize_Pipeline_Test()
        {
            MockPipeline pipeline = new();
            string yaml = pipeline.Publish();
        }
    }
}
