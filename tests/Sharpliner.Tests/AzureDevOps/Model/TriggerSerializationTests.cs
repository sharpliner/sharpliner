using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

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

            Schedule =
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
    public void Serialize_Pipeline_Test()
    {
        TriggerPipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Should().Be(
@"trigger:
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

schedule:
- cron: 0 0 24 * *
  displayName: Releases
  branches:
    include:
    - staging
    - production
  always: true
");
    }
}
