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
    public void Serialize_Triggers_Test()
    {
        TriggerPipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
            """
            trigger:
              branches:
                include:
                - main
              paths:
                include:
                - src/**/*
                exclude:
                - docs/*
                - '*.md'

            pr:
              branches:
                include:
                - main
                - develop

            schedules:
            - cron: 0 0 24 * *
              displayName: Releases
              branches:
                include:
                - staging
                - production
              always: true
            """);
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
    public void Serialize_None_Triggers_Test()
    {
        NoneTriggerPipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
            """
            trigger: none

            pr: none
            """);
    }
}
