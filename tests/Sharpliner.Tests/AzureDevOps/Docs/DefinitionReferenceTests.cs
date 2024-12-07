using System;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

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
    public void AzurePipelineTask_Test()
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

        var yaml = SharplinerSerializer.Serialize(tasks);
        yaml.Trim().Should().Be(
            """
            - task: DotNetCoreCLI@2
              displayName: Run unit tests
              inputs:
                command: test
                projects: src/MyProject.sln
            """
        );
    }

    [Fact]
    public void Dotnet_Test()
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

        var yaml = SharplinerSerializer.Serialize(tasks);
        yaml.Trim().Should().Be(
            """
            - task: UseDotNet@2
              inputs:
                packageType: sdk
                version: ${{ parameters.version }}

            - task: DotNetCoreCLI@2
              inputs:
                command: restore
                includeNuGetOrg: false
                feedsToUse: select
                feedRestore: dotnet-7-preview-feed
                externalFeedCredentials: feeds/dotnet-7
                noCache: true
                restoreDirectory: .packages

            - task: DotNetCoreCLI@2
              inputs:
                command: build
                projects: src/MyProject.csproj
              timeoutInMinutes: 20
            """
        );
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
}
