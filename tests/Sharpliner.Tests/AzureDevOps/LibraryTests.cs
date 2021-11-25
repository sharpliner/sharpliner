using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class LibraryTests
{
    private class DotNet_Step_Library : StepLibrary
    {
        public override List<Conditioned<Step>> Steps => new()
        {
            DotNet.Install.Sdk("6.0.100"),

            DotNet.Restore.Projects("src/MyProject.sln"),

            DotNet.Build("src/MyProject.sln"),
        };
    }

    private class SimpleDotNetPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Foo")
                {
                    Steps =
                    {
                        Script.Inline("echo 'Hello World'"),

                        StepLibrary<DotNet_Step_Library>(),

                        Script.Inline("echo 'Goodbye World'"),
                    }
                }
            }
        };
    }

    [Fact]
    public void Step_Library_Test()
    {
        var yaml = new SimpleDotNetPipeline().Serialize();

        yaml.Should().Be(
@"jobs:
- job: Foo
  steps:
  - script: |-
      echo 'Hello World'

  - task: UseDotNet@2
    inputs:
      packageType: sdk
      version: 6.0.100

  - task: DotNetCoreCLI@2
    inputs:
      command: restore
      projects: src/MyProject.sln

  - task: DotNetCoreCLI@2
    inputs:
      command: build
      projects: src/MyProject.sln

  - script: |-
      echo 'Goodbye World'
");
    }
}
