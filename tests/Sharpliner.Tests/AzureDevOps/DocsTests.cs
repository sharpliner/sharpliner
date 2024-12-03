using System;
using System.Collections.Generic;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class DocsTests : AzureDevOpsDefinition
{
    // Just override prepared abstract classes and `dotnet build` the project, nothing else is needed!
    // For a full list of classes you can override
    //    see https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs
    // You can also generate collections of definitions dynamically
    //    see https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionCollections.md
    class PullRequestPipeline : SingleStagePipelineDefinition
    {
        // Say where to publish the YAML to
        public override string TargetFile => "eng/pr.yml";
        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        private static readonly Variable DotnetVersion = new Variable("DotnetVersion", string.Empty);

        public override SingleStagePipeline Pipeline => new()
        {
            Pr = new PrTrigger("main"),

            Variables =
            [
                // YAML ${{ if }} conditions are available with handy macros that expand into the
                // expressions such as comparing branch names. We also have "else"
                If.IsBranch("net-6.0")
                    .Variable(DotnetVersion with { Value = "6.0.100" })
                    .Group("net6-keyvault")
                .Else
                    .Variable(DotnetVersion with { Value = "5.0.202" }),
            ],

            Jobs =
            [
                new Job("Build")
                {
                    Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                    Steps =
                    [
                        // Many tasks have helper methods for shorter notation
                        DotNet.Install.Sdk(DotnetVersion),

                        NuGet.Authenticate(["myServiceConnection"]),

                        // You can also specify any pipeline task in full too
                        Task("DotNetCoreCLI@2", "Build and test") with
                        {
                            Inputs = new()
                            {
                                { "command", "test" },
                                { "projects", "src/MyProject.sln" },
                            }
                        },

                        // Frequently used ${{ if }} statements have readable macros
                        If.IsPullRequest
                            // You can load script contents from a .ps1 file and inline them into YAML
                            // This way you can write scripts with syntax highlighting separately
                            .Step(Powershell.FromResourceFile("New-Report.ps1", "Create build report")),
                    ]
                }
            ],
        };
    }

    [Fact]
    public void TestDotnetFluentApi()
    {
        ConditionedList<Step> steps =
        [
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
            },
        ];

        var yaml = SharplinerSerializer.Serialize(steps);
        // TODO: assert yaml
    }

    [Fact]
    public void TestUsefulMacros()
    {
        ConditionedList<VariableBase> variables =
        [
            If.IsBranch("production")
                .Variable("rg-suffix", "-pr")
            .Else
                .Variable("rg-suffix", "-prod")
        ];

        var yaml = SharplinerSerializer.Serialize(variables);
        yaml.Trim().Should().Be(
            """
            - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/production') }}:
              - name: rg-suffix
                value: -pr

            - ${{ else }}:
              - name: rg-suffix
                value: -prod
            """);
    }

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

    class PipelineUsingLibrary : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-wth-library.yml";

        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            [
                new Job("Build")
                {
                    Steps =
                    {
                        Script.Inline("echo 'Hello World'"),

                        StepLibrary<ProjectBuildSteps>(),

                        Script.Inline("echo 'Goodbye World'"),
                    }
                }
            ]
        };
    }

    class SourcingFromFiles : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline-from-files.yml";

        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            [
                new Job("Build")
                {
                    Steps =
                    {
                        Bash.FromResourceFile("embedded-script.sh") with
                        {
                            DisplayName = "Run post-build clean-up",
                            Timeout = TimeSpan.FromMinutes(5),
                        }
                    }
                }
            ]
        };
    }

    class YourCustomConfiguration : SharplinerConfiguration
    {
        public override void Configure()
        {
            // You can set severity for various validations
            Validations.DependsOnFields = ValidationSeverity.Off;
            Validations.NameFields = ValidationSeverity.Warning;

            // You can also further customize serialization
            Serialization.PrettifyYaml = false;
            Serialization.UseElseExpression = true;
            Serialization.IncludeHeaders = false;

            // You can add hooks that execute during the publish process
            Hooks.BeforePublish = (definition, path) => {};
            Hooks.AfterPublish = (definition, path, yaml) => {};
        }
    }
}
