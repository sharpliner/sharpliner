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

    private class Extends_Template_Definition : ExtendsTemplateDefinition
    {
        public override string TargetFile => "extends-template.yml";
        public override List<Parameter> Parameters =>
        [
            StringParameter("some"),
            BooleanParameter("other"),
        ];

        public override Extends Definition => new("template.yml", new()
        {
            ["some"] = "value",
            ["other"] = parameters["other"],
        });
    }

    [Fact]
    public void Extends_Template_Definition_Serialization_Test()
    {
        var yaml = new Extends_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: some
              type: string

            - name: other
              type: boolean

            extends:
              template: template.yml
              parameters:
                some: value
                other: ${{ parameters.other }}
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

    private class Step_Typed_Template_Definition(StepTypedParameters? parameters = null) : StepTemplateDefinition<StepTypedParameters>(parameters)
    {
        public override string TargetFile => "step-template.yml";

        public override ConditionedList<Step> Definition => new Step_Template_Definition().Definition;
    }

    class StepTypedParameters : AzureDevOpsDefinition
    {
        public BuildConfiguration Configuration { get; init; }
        public string? Project { get; init; }

        [AllowedValues("5.0.100", "5.0.102")]
        public string? Version { get; init; }
        public bool? SkipBuild { get; init; }
        public bool UseNugetOrg { get; init; } = false;
        public bool Restore { get; init; } = true;
        public Step AfterBuild { get; init; } = Bash.Inline($"cp -R logs {variables.Build.ArtifactStagingDirectory}");
        [YamlMember(Alias = "theCounter")]
        public int Counter { get; init; } = 2;
        [AllowedValues(1, 2, 3, 4)]
        public int? DefaultCounter { get; init; }
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

            - name: theCounter
              type: number
              default: 2

            - name: defaultCounter
              type: number
              values:
              - 1
              - 2
              - 3
              - 4

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

    private class Job_Typed_Template_Definition(JobTypedParameters? parameters = null) : JobTemplateDefinition<JobTypedParameters>(parameters)
    {
        public override string TargetFile => "job-template.yml";

        public override ConditionedList<JobBase> Definition =>
        [
          ..new Job_Template_Definition().Definition,
          Job("with-templates") with
          {
            Steps =
            [
              new Step_Typed_Template_Definition(new()
              {
                AfterBuild = Bash.Inline("echo 'After build'"),
                Counter = 3,
                UseNugetOrg = true
              })
            ]
          }
        ];
    }

    class JobTypedParameters : AzureDevOpsDefinition
    {
        public ConditionedList<JobBase> SetupJobs { get; init; } = [];

        public JobBase MainJob { get; init; } = null!;

        public DeploymentJob Deployment { get; init; } = new("deploy", "Deploy job")
        {
          Environment = new("production"),
          Strategy = new RunOnceStrategy
          {
            Deploy = new()
            {
              Steps =
              {
                Bash.Inline("echo 'Deploying the application'"),
              },
            },
          }
        };

        public ConditionedList<DeploymentJob> AdditionalDeployments { get; init; } = [];
    }

    [Fact]
    public void Job_Typed_Template_Definition_Serialization_Test()
    {
         var yaml = new Job_Typed_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: setupJobs
              type: jobList
              default: []

            - name: mainJob
              type: job

            - name: deployment
              type: deployment
              default:
                deployment: deploy
                displayName: Deploy job
                environment:
                  name: production
                strategy:
                  runOnce:
                    deploy:
                      steps:
                      - bash: |-
                          echo 'Deploying the application'

            - name: additionalDeployments
              type: deploymentList
              default: []

            jobs:
            - job: initialize
              displayName: Initialize job

            - ${{ parameters.mainJob }}

            - job: finalize
              displayName: Finalize job

            - job: with-templates
              steps:
              - template: step-template.yml
                parameters:
                  useNugetOrg: true
                  afterBuild:
                    bash: |-
                      echo 'After build'
                  theCounter: 3
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

    private class Stage_Typed_Template_Definition(StageTypedParameters? parameters = null) : StageTemplateDefinition<StageTypedParameters>(parameters)
    {
      public override string TargetFile => "stage-template.yml";

        public override ConditionedList<Stage> Definition =>
        [
          ..new Stage_Template_Definition().Definition,
          Stage("with-templates") with
          {
            Jobs =
            [
              new Job_Typed_Template_Definition(new()
              {
                MainJob = new Job("main", "Main job")
                {
                  Steps =
                  [
                    Bash.Inline("echo 'Main job step'")
                  ]
                }
              })
            ]
          }
        ];
    }

    class StageTypedParameters : AzureDevOpsDefinition
    {
      public ConditionedList<Stage> SetupStages { get; init; } = [];

      public Stage MainStage { get; init; } = null!;
    }

    [Fact]
    public void Stage_Typed_Template_Definition_Serialization_Test()
    {
        var yaml = new Stage_Typed_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: setupStages
              type: stageList
              default: []

            - name: mainStage
              type: stage

            stages:
            - stage: initialize
              displayName: Initialize stage

            - ${{ parameters.mainStage }}

            - stage: finalize
              displayName: Finalize stage

            - stage: with-templates
              jobs:
              - template: job-template.yml
                parameters:
                  mainJob:
                    job: main
                    displayName: Main job
                    steps:
                    - bash: |-
                        echo 'Main job step'
            """);
    }

    private class Variable_Template_Definition : VariableTemplateDefinition
    {
        public override string TargetFile => "variables.yml";

        public override List<Parameter> Parameters =>
        [
          StringParameter("s_param"),
          BooleanParameter("b_param"),
          NumberParameter("n_param"),
        ];
        public override ConditionedList<VariableBase> Definition =>
        [
          Variable("s_variable", "value"),
          Variable("b_variable", true),
          Variable("n_variable", 42),
        ];
    }

    [Fact]
    public void Variable_Template_Definition_Serialization_Test()
    {
        var yaml = new Variable_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: s_param
              type: string

            - name: b_param
              type: boolean

            - name: n_param
              type: number

            variables:
            - name: s_variable
              value: value

            - name: b_variable
              value: true

            - name: n_variable
              value: 42
            """);
    }

    private class Variable_Typed_Template_Definition(VariableTypedParameters? parameters = null) : VariableTemplateDefinition<VariableTypedParameters>(parameters)
    {
        public override string TargetFile => "variables.yml";
        public override ConditionedList<VariableBase> Definition => new Variable_Template_Definition().Definition;
    }

    class VariableTypedParameters : AzureDevOpsDefinition
    {
        [YamlMember(Alias = "s_param")]
        public string SParam { get; init; } = "default value";
        [YamlMember(Alias = "b_param")]
        public bool BParam { get; init; } = true;
        [YamlMember(Alias = "n_param")]
        public int NParam { get; init; } = 42;
    }

    [Fact]
    public void Variable_Typed_Template_Definition_Serialization_Test()
    {
        var yaml = new Variable_Typed_Template_Definition().Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: s_param
              type: string
              default: default value

            - name: b_param
              type: boolean
              default: true

            - name: n_param
              type: number
              default: 42

            variables:
            - name: s_variable
              value: value

            - name: b_variable
              value: true

            - name: n_variable
              value: 42
            """);
    }

    private class CompletePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new Pipeline
        {
          Stages =
          [
            new Stage_Typed_Template_Definition(new()
            {
              MainStage = new Stage("main-stage")
              {
                Jobs =
                [
                  new Job_Typed_Template_Definition(new()
                  {
                    MainJob = new Job("main-job", "Main job")
                    {
                      Steps =
                      [
                        Bash.Inline("echo 'Hello world!'"),
                        new Step_Typed_Template_Definition(new()
                        {
                          AfterBuild = Bash.Inline("echo 'After build'"),
                          Counter = 3,
                          UseNugetOrg = true
                        })
                      ]
                    }
                  })
                ]
              }
            }),
          ]
        };
    }

    [Fact]
    public void CompletePipeline_Serialization_Test()
    {
        var yaml = new CompletePipeline().Serialize();

        yaml.Trim().Should().Be(
            """
            stages:
            - template: stage-template.yml
              parameters:
                mainStage:
                  stage: main-stage
                  jobs:
                  - template: job-template.yml
                    parameters:
                      mainJob:
                        job: main-job
                        displayName: Main job
                        steps:
                        - bash: |-
                            echo 'Hello world!'
                        - template: step-template.yml
                          parameters:
                            useNugetOrg: true
                            afterBuild:
                              bash: |-
                                echo 'After build'
                            theCounter: 3
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
