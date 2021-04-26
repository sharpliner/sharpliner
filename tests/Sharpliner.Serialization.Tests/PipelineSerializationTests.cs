using System.IO;
using Xunit;

namespace Sharpliner.Serialization.Tests
{
    public class PipelineSerializationTests
    {
        [Fact]
        public void Serialize_Pipeline_Test()
        {
            MockPipeline pipeline = new();
            MemoryStream stream = new();
            pipeline.Publish(stream);

            stream.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new(stream);
            string yaml = reader.ReadToEnd();
        }
    }
}
