using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class TriggerSerializationTests
{
    private class TriggerPipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Pr = new PrTrigger("main", "develop")
            {
                Drafts = true,
                AutoCancel = true,
            },

            Trigger = new Trigger("main")
            {
                Paths = new()
                {
                    Include = { "src/**/*" },
                    Exclude = { "docs/*", "*.md" }
                }
            },

            Schedules =
            {
                new("0 0 24 * *", "staging", "production")
                {
                    DisplayName = "Releases",
                    Always = true,
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Triggers_Test()
    {
        TriggerPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }
    private class NoneTriggerPipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Pr = PrTrigger.None,
            Trigger = Trigger.None,
        };
    }

    [Fact]
    public Task Serialize_None_Triggers_Test()
    {
        NoneTriggerPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }
}
