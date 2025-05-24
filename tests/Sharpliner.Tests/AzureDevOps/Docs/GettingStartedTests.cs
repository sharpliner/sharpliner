using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.Tests.AzureDevOps.Docs;

public class GettingStartedTests : AzureDevOpsDefinition
{
    class TestPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "test-pipeline.yaml";

#region single-stage-pipeline-example-csharp
        private static readonly Variable DotnetVersion = new Variable("DotnetVersion", string.Empty);

        public override SingleStagePipeline Pipeline => new()
        {
            Pr = new PrTrigger("main"),

            Variables =
            [
                If.IsBranch("net-6.0")
                    .Variable(DotnetVersion with { Value = "6.0.100" })
                    .Group("net6-kv")
                .Else
                    .Variable(DotnetVersion with { Value = "5.0.202" }),
            ],

            Jobs =
            [
                new Job("Build", "Build and test")
                {
                    Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                    Steps =
                    [
                        If.IsPullRequest
                            .Step(Powershell.Inline(
                                    "Write-Host 'Hello'",
                                    "Write-Host 'World'")
                                .DisplayAs("Hello world")),

                        DotNet.Install
                            .Sdk(DotnetVersion)
                            .DisplayAs("Install .NET SDK"),

                        DotNet
                            .Build("src/MyProject.sln", includeNuGetOrg: true)
                            .DisplayAs("Build"),

                        DotNet
                            .Test("src/MyProject.sln")
                            .DisplayAs("Test"),
                    ]
                }
            ],
        };
#endregion
    }

    [Fact]
    public void Serialize_TestPipeline_Test()
    {
        var pipeline = new TestPipeline();
        var yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
#region single-stage-pipeline-example-yaml
            """
            pr:
              branches:
                include:
                - main

            variables:
            - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/net-6.0') }}:
              - name: DotnetVersion
                value: 6.0.100

              - group: net6-kv

            - ${{ else }}:
              - name: DotnetVersion
                value: 5.0.202

            jobs:
            - job: Build
              displayName: Build and test
              pool:
                name: Azure Pipelines
                vmImage: windows-latest
              steps:
              - ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
                - powershell: |-
                    Write-Host 'Hello'
                    Write-Host 'World'
                  displayName: Hello world

              - task: UseDotNet@2
                displayName: Install .NET SDK
                inputs:
                  packageType: sdk
                  version: $(DotnetVersion)

              - task: DotNetCoreCLI@2
                displayName: Build
                inputs:
                  command: build
                  projects: src/MyProject.sln
                  includeNuGetOrg: true

              - task: DotNetCoreCLI@2
                displayName: Test
                inputs:
                  command: test
                  projects: src/MyProject.sln
            """
#endregion
        );
    }

#region configuration
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
            Hooks.BeforePublish = (definition, path) => { };
            Hooks.AfterPublish = (definition, path, yaml) => { };
        }
    }
#endregion

    [Fact]
    public void Step_ValidateYamlsArePublished_Test()
    {
        ConditionedList<Step> steps =
        [
#region validate-yaml-step
            ValidateYamlsArePublished("src/MyProject.Pipelines.csproj")
#endregion
        ];

        var yaml = SharplinerSerializer.Serialize(steps);
        yaml.Trim().Should().Be(
            """
            - task: DotNetCoreCLI@2
              displayName: Validate YAML has been published
              inputs:
                command: build
                projects: src/MyProject.Pipelines.csproj
                arguments: -p:FailIfChanged=true
            """
        );
    }
}
