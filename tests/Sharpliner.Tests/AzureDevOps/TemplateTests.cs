using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;
using YamlDotNet.Serialization;

namespace Sharpliner.Tests.AzureDevOps;

public class TemplateTests
{
    private class TemplateList_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                If.Equal(parameters["restore"], "bar")
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

        yaml.Trim().Should().Be(
            """
            jobs:
            - ${{ if eq(parameters.restore, 'bar') }}:
              - template: template1.yml
                parameters:
                  enableTelemetry: true

              - template: template2.yml
                parameters:
                  enableTelemetry: false
            """);
    }

    private class Step_Template_Definition : StepTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        protected Parameter configuration = EnumParameter<BuildConfiguration>("configuration", defaultValue: BuildConfiguration.Debug);
        protected Parameter project = StringParameter("project");
        protected Parameter version = StringParameter("version", allowedValues: [ "5.0.100", "5.0.102" ]);
        protected Parameter skipBuild = BooleanParameter("skipBuild");
        protected Parameter useNugetOrg = BooleanParameter("useNugetOrg", defaultValue: false);
        protected Parameter restore = BooleanParameter("restore", defaultValue: true);
        protected Parameter<Step> afterBuild = StepParameter("afterBuild", Bash.Inline($"cp -R logs {variables.Build.ArtifactStagingDirectory}"));

        public override List<Parameter> Parameters =>
        [
            configuration,
            project,
            version,
            skipBuild,
            useNugetOrg,
            restore,
            afterBuild,
        ];

        public override ConditionedList<Step> Definition =>
        [
            DotNet.Install.Sdk(version),

            If.Equal(restore, "true")
                .Step(DotNet.Restore.Projects(project)),

            DotNet.Build(project),

            StepParameterReference(afterBuild),
        ];
    }

    private enum BuildConfiguration
    {
        [YamlMember(Alias = "debug")]
        Debug,

        [YamlMember(Alias = "release")]
        Release,
    }

    [Fact]
    public void Step_Template_Definition_Serialization_Test()
    {
        var yaml = new Step_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: configuration
              type: string
              default: debug
              values:
              - debug
              - release

            - name: project
              type: string

            - name: version
              type: string
              values:
              - 5.0.100
              - 5.0.102

            - name: skipBuild
              type: boolean

            - name: useNugetOrg
              type: boolean
              default: false

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

            - ${{ if eq(parameters.restore, true) }}:
              - task: DotNetCoreCLI@2
                inputs:
                  command: restore
                  projects: ${{ parameters.project }}

            - task: DotNetCoreCLI@2
              inputs:
                command: build
                projects: ${{ parameters.project }}

            - ${{ parameters.afterBuild }}
            """);
    }

    private class Step_Typed_Template_Definition : StepTemplateDefinition<StepTypedParameters>
    {
        public override string TargetFile => "template.yml";

        public override ConditionedList<Step> Definition => new Step_Template_Definition().Definition;
    }

    class StepTypedParameters : AzureDevOpsDefinition
    {
        public BuildConfiguration Configuration { get; init; }
        public string? Project { get; init; }

        [AllowedValues("5.0.100", "5.0.102")]
        public string? Version { get; init; }
        public bool? SkipBuild { get; init; }
        public bool? UseNugetOrg { get; init; } = false;
        public bool? Restore { get; init; } = true;
        public Step AfterBuild { get; init; } = Bash.Inline($"cp -R logs {variables.Build.ArtifactStagingDirectory}");
    }

    [Fact]
    public void Step_Typed_Template_Definition_Serialization_Test()
    {
        var yaml = new Step_Typed_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: configuration
              type: string
              default: debug
              values:
              - debug
              - release

            - name: project
              type: string

            - name: version
              type: string
              values:
              - 5.0.100
              - 5.0.102

            - name: skipBuild
              type: boolean

            - name: useNugetOrg
              type: boolean
              default: false

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

            - ${{ if eq(parameters.restore, true) }}:
              - task: DotNetCoreCLI@2
                inputs:
                  command: restore
                  projects: ${{ parameters.project }}

            - task: DotNetCoreCLI@2
              inputs:
                command: build
                projects: ${{ parameters.project }}

            - ${{ parameters.afterBuild }}
            """);
    }

    private class Job_Template_Definition : JobTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        Parameter<JobBase> mainJob = JobParameter("mainJob");

        public override List<Parameter> Parameters =>
        [
            mainJob,
        ];

        public override ConditionedList<JobBase> Definition =>
        [
            Job("initialize") with
            {
                DisplayName = "Initialize job",
            },
            JobParameterReference(mainJob),
            Job("finalize") with
            {
                DisplayName = "Finalize job",
            },
        ];
    }

    [Fact]
    public void Job_Template_Definition_Serialization_Test()
    {
         var yaml = new Job_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: mainJob
              type: job

            jobs:
            - job: initialize
              displayName: Initialize job

            - ${{ parameters.mainJob }}

            - job: finalize
              displayName: Finalize job
            """);
    }

    private class Stage_Template_Definition : StageTemplateDefinition
    {
        public override string TargetFile => "template.yml";

        Parameter<Stage> mainStage = StageParameter("mainStage");

        public override List<Parameter> Parameters =>
        [
            mainStage,
        ];

        public override ConditionedList<Stage> Definition =>
        [
            Stage("initialize") with
            {
                DisplayName = "Initialize stage",
            },
            StageParameterReference(mainStage),
            Stage("finalize") with
            {
                DisplayName = "Finalize stage",
            },
        ];
    }

    [Fact]
    public void Stage_Template_Definition_Serialization_Test()
    {
        var yaml = new Stage_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: mainStage
              type: stage

            stages:
            - stage: initialize
              displayName: Initialize stage

            - ${{ parameters.mainStage }}

            - stage: finalize
              displayName: Finalize stage
            """);
    }

    private class Conditioned_Template_Reference : SimpleStepTestPipeline
    {
        protected override ConditionedList<Step> Steps =>
        [
            If.Equal("restore", "true")
                .StepTemplate("template1.yaml"),

            If.IsBranch("main")
                .StepTemplate("template2.yaml")
                .If.IsPullRequest
                    .StepTemplate("template3.yaml"),
        ];
    }

    [Fact]
    public void Conditioned_Template_Reference_Serialization_Test()
    {
        var yaml = new Conditioned_Template_Reference().Serialize();

        yaml.Trim().Should().Be(
            """
            jobs:
            - job: testJob
              steps:
              - ${{ if eq('restore', true) }}:
                - template: template1.yaml

              - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
                - template: template2.yaml

                - ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
                  - template: template3.yaml
            """);
    }

    private class Conditioned_Parameters : SimpleStepTestPipeline
    {
        protected override ConditionedList<Step> Steps =>
        [
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
                {
                    "other",
                    If.Equal(parameters["container"], "")
                        .Value(new TemplateParameters
                        {
                            { "image", "ubuntu-16.04-cross-arm64-20210719121212-8a8d3be" }
                        })
                    .Else
                        .Value(new TemplateParameters
                        {
                            { "image", parameters["container"] }
                        })
                },
            }),
        ];
    }

    [Fact]
    public void Conditioned_Parameters_Serialization_Test()
    {
        var pipeline = new Conditioned_Parameters();
        var yaml = pipeline.Serialize();

        yaml.Trim().Should().Be(
            """
            jobs:
            - job: testJob
              steps:
              - template: template1.yaml
                parameters:
                  some: value
                  ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
                    pr: true
                  other:
                    ${{ if eq(parameters.container, '') }}:
                      image: ubuntu-16.04-cross-arm64-20210719121212-8a8d3be
                    ${{ else }}:
                      image: ${{ parameters.container }}
            """);
    }
}
