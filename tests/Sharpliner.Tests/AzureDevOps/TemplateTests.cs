using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class TemplateTests
{
    private class TemplateList_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
                {
                    If.Equal("foo", "bar")
                        .JobTemplate("template1.yml", new TemplateParameters
                        {
                            { "enableTelemetry", true },
                        })
                        .JobTemplate("template2.yml", new TemplateParameters
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

    private class Conditioned_Template_Reference : SimpleStepTestPipeline
    {
        protected override ConditionedList<Step> Steps => new()
        {
            If.Equal("restore", "true")
                .StepTemplate("template1.yaml"),

            If.IsBranch("main")
                .StepTemplate("template2.yaml")
                .If.IsPullRequest
                    .StepTemplate("template3.yaml"),
        };
    }

    [Fact]
    public void Conditioned_Template_Reference_Serialization_Test()
    {
        var yaml = new Conditioned_Template_Reference().Serialize();

        yaml.Should().Be(
@"jobs:
- job: testJob
  steps:
  - ${{ if eq(restore, true) }}:
    - template: template1.yaml

  - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    - template: template2.yaml

    - ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
      - template: template3.yaml
");
    }

    private class Conditioned_Parameters : SimpleStepTestPipeline
    {
        protected override ConditionedList<Step> Steps => new()
        {
            StepTemplate("template1.yaml", new()
            {
                { "some", "value" },
                {
                    If.IsPullRequest,
                    new TemplateParameters()
                    {
                        { "pr", true }
                    }
                },
                { "other", 123 },
            }),
        };
    }

    [Fact]
    public void Conditioned_Parameters_Serialization_Test()
    {
        var yaml = new Conditioned_Parameters().Serialize();

        yaml.Should().Be(
@"jobs:
- job: testJob
  steps:
  - template: template1.yaml
    parameters:
      some: value
      ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
        pr: true
      other: 123
");
    }
}
