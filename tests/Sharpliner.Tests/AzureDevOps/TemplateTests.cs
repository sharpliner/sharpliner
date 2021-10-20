using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class TemplateTests
{
    private class Template_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
                {
                    new Job("job")
                    {
                        Pool =
                            If.Equal("foo", "bar")
                                .Template<Pool>("pool1.yml")
                            .Else
                                .Template<Pool>("pool2.yml")
                    }
                }
        };
    }

    [Fact]
    public void Template_Serialization_Test()
    {
        var yaml = new Template_Pipeline().Serialize();

        yaml.Should().Be(
@"jobs:
- job: job
  pool:
    ${{ if eq(foo, bar) }}:
      template: pool1.yml
    ${{ if ne(foo, bar) }}:
      template: pool2.yml
");
    }

    private class TemplateList_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
                {
                    If.Equal("foo", "bar")
                        .Template<JobBase>("template1.yml", new TemplateParameters
                        {
                            { "enableTelemetry", true },
                        })
                        .Template<JobBase>("template2.yml", new TemplateParameters
                        {
                            { "enableTelemetry", false },
                        })
                }
        };
    }

    [Fact]
    public void TemplateList_Serialization_Test()
    {
        var yaml = new TemplateList_Pipeline().Serialize();

        yaml.Should().Be(
@"jobs:
- ${{ if eq(foo, bar) }}:
  - template: template1.yml
    parameters:
      enableTelemetry: true

  - template: template2.yml
    parameters:
      enableTelemetry: false
");
    }
    private class Template_Definition : StepTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        public override List<TemplateParameter> Parameters => new()
        {
            StringParameter("project"),
            StringParameter("version", allowedValues: new[] { "5.0.100", "5.0.102" }),
            BooleanParameter("restore", defaultValue: true),
            StepParameter("afterBuild", Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)")),
        };

        public override ConditionedList<Step> Definition => new()
        {
            DotNet.Install.Sdk(parameters["version"]),

            If.Equal(parameters["restore"], "true")
                .Step(DotNet.Restore.Projects(parameters["project"])),

            DotNet.Build(parameters["project"]),

            StepParameterReference("afterBuild"),
        };
    }

    [Fact]
    public void Template_Definition_Serialization_Test()
    {
        var yaml = new Template_Definition().Serialize();

        yaml.Should().Be(
@"parameters:
- name: project
  type: string

- name: version
  type: string
  values:
  - 5.0.100
  - 5.0.102

- name: restore
  type: boolean
  default: true

- name: afterBuild
  type: step
  default:
    bash: |-
      cp -R logs $(Build.ArtifactStagingDirectory)

steps:
- task: UseDotNet@2
  inputs:
    packageType: sdk
    version: ${{ parameters.version }}

- ${{ if eq(${{ parameters.restore }}, true) }}:
  - task: DotNetCoreCLI@2
    inputs:
      command: restore
      projects: ${{ parameters.project }}

- task: DotNetCoreCLI@2
  inputs:
    command: build
    projects: ${{ parameters.project }}
- ${{ parameters.afterBuild }}
");
    }
}
