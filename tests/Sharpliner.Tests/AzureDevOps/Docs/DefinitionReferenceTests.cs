using System.ComponentModel;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;
using YamlDotNet.Serialization;

namespace Sharpliner.Tests.AzureDevOps.Docs;

public class DefinitionReferenceTests : AzureDevOpsDefinition
{

    class ClassicPipelineSteps : SingleStagePipelineDefinition
    {
        public override string TargetFile => "classic-pipeline.yml";

        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("main")
                {
#region classic-pipeline-steps
                    Steps =
                    {
                        new AzureDevOpsTask("DotNetCoreCLI@2")
                        {
                            DisplayName = "Build solution",
                            Inputs = new()
                            {
                                { "command", "build" },
                                { "includeNuGetOrg", true },
                                { "projects", "src/MyProject.sln" },
                            },
                            Timeout = TimeSpan.FromMinutes(20)
                        },

                        new InlineBashTask("./.dotnet/dotnet test src/MySolution.sln")
                        {
                            DisplayName = "Run unit tests",
                            ContinueOnError = true,
                        },
                    }
#endregion
                }
            }
        };
    }

    class ShorthandPipelineSteps : SingleStagePipelineDefinition
    {
        public override string TargetFile => "shorthand-pipeline.yml";

        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("main")
                {
#region shorthand-pipeline-steps
                    Steps =
                    {
                        Checkout.Self,

                        Download.LatestFromBranch("internal", 23, "refs/heads/develop", artifact: "CLI.Package") with
                        {
                            AllowPartiallySucceededBuilds = true,
                            CheckDownloadedFiles = true,
                            PreferTriggeringPipeline = true,
                        },

                        // Tasks are represented as C# records so you can use the `with` keyword to override the properties
                        DotNet.Build("src/MyProject.sln", includeNuGetOrg: true) with
                        {
                            Timeout = TimeSpan.FromMinutes(20)
                        },

                        // Some of the shorthand styles define more options and a cleaner way of defining them
                        // E.g. Bash gives you several ways where to get the script from such as Bash.FromResourceFile or Bash.FromFile
                        Bash.Inline("./.dotnet/dotnet test src/MySolution.sln") with
                        {
                            DisplayName = "Run unit tests",
                            ContinueOnError = true,
                        },

                        Publish.Pipeline("ArtifactName", "bin/**/*.dll") with
                        {
                            DisplayName = "Publish build artifacts"
                        },
                    }
#endregion
                }
            }
        };
    }

    [Fact]
    public Task AzurePipelineTask_Test()
    {
        ConditionedList<Step> tasks =
        [
#region azure-pipeline-task
            Task("DotNetCoreCLI@2", "Run unit tests") with
            {
                Inputs = new()
                {
                    { "command", "test" },
                    { "projects", "src/MyProject.sln" },
                }
            }
#endregion
        ];

        return Verify(SharplinerSerializer.Serialize(tasks));
    }

    [Fact]
    public Task Dotnet_Test()
    {
        ConditionedList<Step> tasks =
        [
#region dotnet-tasks
            DotNet.Install.Sdk(parameters["version"]),

            DotNet.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
            {
                ExternalFeedCredentials = "feeds/dotnet-7",
                NoCache = true,
                RestoreDirectory = ".packages",
            },

            DotNet.Build("src/MyProject.csproj") with
            {
                Timeout = TimeSpan.FromMinutes(20)
            }
#endregion
        ];

        return Verify(SharplinerSerializer.Serialize(tasks));
    }

    [Fact]
    public void NuGet_Test()
    {
        ConditionedList<Step> tasks =
        [
#region nuget-tasks-code
            NuGet.Authenticate(new[] { "NuGetServiceConnection1", "NuGetServiceConnection2" }, forceReinstallCredentialProvider: true),

            NuGet.Restore.FromFeed("my-project/my-project-scoped-feed") with
            {
                RestoreSolution = "**/*.sln",
                IncludeNuGetOrg = false,
            },

            NuGet.Pack.ByPrereleaseNumber("3", "1", "4"),
            NuGet.Pack.ByEnvVar("VERSION"),

            NuGet.Push.ToInternalFeed("MyInternalFeed"),
            NuGet.Push.ToExternalFeed("MyExternalFeedCredentials"),

            NuGet.Custom(@"config -Set repositoryPath=c:\packages -configfile c:\my.config")
#endregion
        ];

        var yaml = SharplinerSerializer.Serialize(tasks);
        yaml.Trim().Should().Be(
#region nuget-tasks-yaml
            """
            - task: NuGetAuthenticate@1
              inputs:
                forceReinstallCredentialProvider: true
                nuGetServiceConnections: NuGetServiceConnection1,NuGetServiceConnection2

            - task: NuGetCommand@2
              inputs:
                command: restore
                feedsToUse: select
                vstsFeed: my-project/my-project-scoped-feed
                restoreSolution: '**/*.sln'
                includeNuGetOrg: false

            - task: NuGetCommand@2
              inputs:
                command: pack
                versioningScheme: byPrereleaseNumber
                majorVersion: 3
                minorVersion: 1
                patchVersion: 4

            - task: NuGetCommand@2
              inputs:
                command: pack
                versioningScheme: byEnvVar
                versionEnvVar: VERSION

            - task: NuGetCommand@2
              inputs:
                command: push
                nuGetFeedType: internal
                publishVstsFeed: MyInternalFeed

            - task: NuGetCommand@2
              inputs:
                command: push
                nuGetFeedType: external
                publishFeedCredentials: MyExternalFeedCredentials

            - task: NuGetCommand@2
              inputs:
                command: custom
                arguments: config -Set repositoryPath=c:\packages -configfile c:\my.config
            """
#endregion
        );
    }

    class PipelineVariables : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-variables.yml";

        public override SingleStagePipeline Pipeline => new()
        {
#region pipeline-variables
            Variables =
            [
                Variable("Configuration", "Release"),     // We have shorthand style like we do for build steps
                Group("PR keyvault variables"),
                new Variable("Configuration", "Release"), // We can also create the objects and reuse them too

            ]
#endregion
        };
    }

    class ReadablePipelineVariables : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-variables-readable.yml";

#region pipeline-variables-readable
        static readonly Variable s_version = new("version", "5.0.100");
        public override SingleStagePipeline Pipeline => new()
        {
            Variables = [s_version],
            Jobs =
            {
                new Job("main")
                {
                    Steps =
                    {
                        DotNet.Install.Sdk(s_version),
                    }
                }
            }
        };
#endregion
    }

    class PipelineParameters : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-parameters.yml";

        public override SingleStagePipeline Pipeline => new()
        {
#region pipeline-parameters
            Parameters =
            [
                StringParameter("project", "AzureDevops project"),
                StringParameter("version", ".NET version", allowedValues: ["5.0.100", "5.0.102"]),
                BooleanParameter("restore", "Restore NuGets", defaultValue: true),
                StepParameter("afterBuild", "After steps", Bash.Inline($"cp -R logs {variables.Build.ArtifactStagingDirectory}")),
                EnumParameter<BuildConfiguration>("configuration", defaultValue: BuildConfiguration.Debug),
            ],
#endregion
        };

#region enum-definition
        // and the enum definition
        public enum BuildConfiguration
        {
            [YamlMember(Alias = "debug")]
            Debug,

            [YamlMember(Alias = "release")]
            Release,
        }
#endregion
    }

    class ReadablePipelineParameters : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-parameters-readable.yml";

#region pipeline-parameters-readable
        static readonly Parameter s_version = StringParameter("version", ".NET version", allowedValues: ["5.0.100", "5.0.102"]);
        public override SingleStagePipeline Pipeline => new()
        {
            Parameters = [s_version],
            Jobs =
            {
                new Job("main")
                {
                    Steps =
                    {
                        DotNet.Install.Sdk(s_version),
                    }
                }
            }
        };
#endregion
    }

    class ConditionedExpressionsPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-conditions.yml";

        public override SingleStagePipeline Pipeline => new()
        {
#region conditioned-expressions-code
            Variables =
            [
                // You can create one if statement and chain multiple definitions beneath it
                If.Equal(variables.Environment["Target"], "Cloud")
                    .Variable("target", "Azure")
                    .Variable("isCloud", true)

                    // You can nest another if statement beneath
                    .If.NotEqual(variables.Build.Reason, "'PullRequest'")
                        .Group("azure-int")
                    .EndIf // You can jump out of the nested section too

                    // You can use many macros such as IsBranch or IsPullRequest
                    .If.IsBranch("main")
                        .Group("azure-prod")

                    // You can also swap the previous condition with an "else"
                    // Azure Pipelines now support ${{ else }} but you can also revert to using an
                    // inverted if condition using SharplinerSerializer.UseElseExpression setting
                    .Else
                        .Group("azure-pr"),
            ]
#endregion
        };
    }

    [Fact]
    public void Serialize_ConditionedExpressionsPipeline_Test()
    {
        var pipeline = new ConditionedExpressionsPipeline();
        var yaml = SharplinerSerializer.Serialize(pipeline.Pipeline);

        yaml.Trim().Should().Be(
#region conditioned-expressions-yaml
            """
            variables:
            - ${{ if eq(variables['Environment.Target'], 'Cloud') }}:
              - name: target
                value: Azure

              - name: isCloud
                value: true

              - ${{ if ne(variables['Build.Reason'], 'PullRequest') }}:
                - group: azure-int

              - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
                - group: azure-prod

              - ${{ else }}:
                - group: azure-pr
            """
#endregion
        );
    }

    [Fact]
    public void Serialize_TemplateConditionedExpressions_Test()
    {
        Conditioned<Step> step =
#region template-conditioned-expressions-code
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
            })
#endregion
        ;

        var yaml = SharplinerSerializer.Serialize(step);
        yaml.Trim().Should().Be(
#region template-conditioned-expressions-yaml
            """
            template: template1.yaml

            parameters:
              some: value
              ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
                pr: true
              other: 123
            """
#endregion
        );
    }

    [Fact]
    public void Serialize_Conditions_Test()
    {
        var condition =
#region conditions-code
        If.Or(
            And(NotEqual("true", "true"), Equal(variables["Build.SourceBranch"], "'refs/heads/production'")),
            NotEqual(variables["Configuration"], "'Debug'"))
#endregion
        ;

        var yaml = SharplinerSerializer.Serialize(condition);
        yaml.Trim().Should().Be(
#region conditions-yaml
            """
            ${{ if or(and(ne(true, true), eq(variables['Build.SourceBranch'], 'refs/heads/production')), ne(variables['Configuration'], 'Debug')) }}
            """
#endregion
        );
    }

    [Fact]
    public void Serialize_ConditionsMacros_Test()
    {
        object[] conditions =
        [
#region conditions-macros
            // eq(variables['Build.SourceBranch'], 'refs/heads/production')
            If.IsBranch("production"),
            If.IsNotBranch("production"),

            // eq(variables['Build.Reason'], 'PullRequest')
            If.IsPullRequest,
            If.IsNotPullRequest,

            // You can mix these too
            If.And(IsNotPullRequest, IsBranch("production")),

            // You can specify any custom condition in case we missed an API :)
            If.Condition("containsValue(...)")
#endregion
        ];
    }

    class EachExpressionPipeline : PipelineDefinition
    {
        ObjectParameter Stages = ObjectParameter("stages", "Environment names", new ConditionedDictionary()
        {
            { "Dev", string.Empty }
        });

        public override string TargetFile => "pipeline-each.yml";

        public override Pipeline Pipeline => new()
        {
#region each-expression-code
            Stages =
            {
                If.IsBranch("main")
                    .Each("env", parameters[Stages.Name])
                        .Stage(new Stage("stage-${{ env.name }}"))
                        .Stage(new Stage("stage2-${{ env.name }}")
                        {
                            Jobs =
                            {
                                Each("foo", "bar")
                                    .Job(new Job("job-${{ foo }}"))
                                .EndEach
                                .If.Equal("foo", "bar")
                                    .Job(new Job("job2-${{ foo }}"))
                            }
                        })
            }
#endregion
        };
    }

    [Fact]
    public void Serialize_EachExpressionPipeline_Test()
    {
        var pipeline = new EachExpressionPipeline();
        var yaml = SharplinerSerializer.Serialize(pipeline.Pipeline);

        yaml.Trim().Should().Be(
#region each-expression-yaml
            """
            stages:
            - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
              - ${{ each env in parameters.stages }}:
                - stage: stage-${{ env.name }}

                - stage: stage2-${{ env.name }}
                  jobs:
                  - ${{ each foo in bar }}:
                    - job: job-${{ foo }}

                  - ${{ if eq('foo', 'bar') }}:
                    - job: job2-${{ foo }}
            """
#endregion
        );
    }

#region strongly-typed-parameters-code
    // Template parameters
    // The parameters do not need to inherit from AzureDevOpsDefinition,
    // but it gives you nice abilities such as the Bash.Inline() macro.
    class InstallDotNetParameters : AzureDevOpsDefinition
    {
        public BuildConfiguration Configuration { get; init; } = BuildConfiguration.Release;
        public string? Project { get; init; }

        [DisplayName(".NET version")]
        [AllowedValues("5.0.100", "5.0.102")]
        public string? Version { get; init; }
        public bool Restore { get; init; } = true;
        public Step AfterBuild { get; init; } = Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)");
    }

    enum BuildConfiguration
    {
        [YamlMember(Alias = "debug")]
        Debug,

        [YamlMember(Alias = "release")]
        Release,
    }

    // Template itself - the passed in parameters are the values used when referencing the template
    class StronglyTypedInstallDotNetTemplate(InstallDotNetParameters? parameters = null)
        : StepTemplateDefinition<InstallDotNetParameters>(parameters)
    {
        // Where to publish the YAML to
        public override string TargetFile => "templates/install-dotnet.yml";

        public override ConditionedList<Step> Definition =>
        [
            DotNet.Install.Sdk(parameters["version"]),

            If.Equal(parameters["restore"], "true")
                .Step(DotNet.Restore.Projects(parameters["project"])),

            DotNet.Build(parameters["project"]),

            parameters["afterBuild"],
        ];
    }
#endregion

    [Fact]
    public void Serialize_StronglyTypedInstallDotNetTemplate_Test()
    {
        var template = new StronglyTypedInstallDotNetTemplate();
        var yaml = template.Serialize();

        yaml.Trim().Should().Be(
#region strongly-typed-parameters-yaml
            """
            parameters:
            - name: configuration
              type: string
              default: release
              values:
              - debug
              - release

            - name: project
              type: string

            - name: version
              displayName: .NET version
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
            """
#endregion
        );
    }

#region untyped-parameters-template
    class InstallDotNetTemplate : StepTemplateDefinition
    {
        // Where to publish the YAML to
        public override string TargetFile => "templates/build-csproj.yml";

        private static readonly Parameter configuration = EnumParameter<BuildConfiguration>("configuration", defaultValue: BuildConfiguration.Release);
        private static readonly Parameter project = StringParameter("project");
        private static readonly Parameter version = StringParameter("version", allowedValues: ["5.0.100", "5.0.102"]);
        private static readonly Parameter restore = BooleanParameter("restore", defaultValue: true);
        private static readonly Parameter<Step> afterBuild = StepParameter("afterBuild", defaultValue: Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)"));

        public override List<Parameter> Parameters =>
        [
            configuration,
            project,
            version,
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
#endregion

    [Fact]
    public void Serialize_UntypedInstallDotNetTemplate_Test()
    {
        var template = new InstallDotNetTemplate();
        var yaml = template.Serialize();

        yaml.Trim().Should().Be(
            """
            parameters:
            - name: configuration
              type: string
              default: release
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

    [Fact]
    public void UseTypedTemplate()
    {
        var job = new Job("main")
        {
#region use-typed-template
            // The strong-typed version
            Steps =
            [
                new StronglyTypedInstallDotNetTemplate(new()
                {
                    Project = "src/MyProject.csproj",
                    Version = "5.0.100",
                })
            ]
#endregion
        };

        var yaml = SharplinerSerializer.Serialize(job);
        yaml.Trim().Should().Be(
            """
            job: main

            steps:
            - template: templates/install-dotnet.yml
              parameters:
                project: src/MyProject.csproj
                version: 5.0.100
            """
        );
    }

    [Fact]
    public void UseUntypedTemplate()
    {
        var job = new Job("main")
        {
#region use-untyped-template
            // The non-strong-typed version (second example of the InstallDotNet definition)
            Steps =
            [
                StepTemplate("templates/install-dotnet.yml", new()
                {
                    { "project", "src/MyProject.csproj" },
                    { "version", "5.0.100" },
                })
            ]
#endregion
        };

        var yaml = SharplinerSerializer.Serialize(job);
        yaml.Trim().Should().Be(
            """
            job: main

            steps:
            - template: templates/install-dotnet.yml
              parameters:
                project: src/MyProject.csproj
                version: 5.0.100
            """
        );
    }

#region definition-library
    class ProjectBuildSteps : StepLibrary
    {
        public override List<Conditioned<Step>> Steps =>
        [
            DotNet.Install.Sdk("6.0.100"),

            If.IsBranch("main")
                .Step(DotNet.Restore.Projects("src/MyProject.sln")),

            DotNet.Build("src/MyProject.sln"),
        ];
    }
#endregion

    [Fact]
    public void ReferenceDefinitionLibrary()
    {
        var job =
#region definition-library-usage
        new Job("Build")
        {
            Steps =
            [
                Script.Inline("echo 'Hello World'"),

                StepLibrary<ProjectBuildSteps>(),

                Script.Inline("echo 'Goodbye World'"),
            ]
        }
#endregion
        ;

        var yaml = SharplinerSerializer.Serialize(job);
        yaml.Trim().Should().Be(
            """
            job: Build

            steps:
            - script: |-
                echo 'Hello World'

            - task: UseDotNet@2
              inputs:
                packageType: sdk
                version: 6.0.100

            - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
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
            """
        );
    }
}
