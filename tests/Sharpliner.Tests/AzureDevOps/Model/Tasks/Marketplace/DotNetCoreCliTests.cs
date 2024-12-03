using System;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class DotNetCoreCliTests
{
    private readonly DotNetTaskBuilder _builder = new();

    private class DotNet_Pipeline(Step step) : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job")
                {
                    Steps = { step }
                }
            }
        };
    }

    [Fact]
    public void Build_Command_Test()
    {
        var task = _builder.Build("project.csproj", true, "-c Release") with
        {
            WorkingDirectory = "/tmp"
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: build
                  projects: project.csproj
                  arguments: -c Release
                  includeNuGetOrg: true
                  workingDirectory: /tmp
            """);
    }

    [Fact]
    public void Install_Sdk_Command_Test()
    {
        var task = _builder.Install.Sdk("6.0.100-rc.2.21505.57", true) with
        {
            WorkingDirectory = "/tmp",
            InstallationPath = "/.dotnet",
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: UseDotNet@2
                inputs:
                  packageType: sdk
                  version: 6.0.100-rc.2.21505.57
                  includePreviewVersions: true
                  workingDirectory: /tmp
                  installationPath: /.dotnet
            """);
    }

    [Fact]
    public void Install_Runtime_Command_Test()
    {
        var task = _builder.Install.Runtime("5.0.100") with
        {
            PerformMultiLevelLookup = true,
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: UseDotNet@2
                inputs:
                  packageType: runtime
                  version: 5.0.100
                  performMultiLevelLookup: true
            """);
    }

    [Fact]
    public void Install_GlobalJson_Command_Test()
    {
        var task = _builder.Install.FromGlobalJson("/foo/global.json") with
        {
            WorkingDirectory = "/tmp",
            InstallationPath = "/.dotnet",
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: UseDotNet@2
                inputs:
                  useGlobalJson: true
                  workingDirectory: /tmp
                  installationPath: /.dotnet
            """);
    }

    [Fact]
    public void Pack_Command_Test()
    {
        var task = _builder.Pack("src/*.csproj", "-c Release") with
        {
            NoBuild = true,
            ConfigurationToPack = "Release",
            IncludeSource = true,
            IncludeSymbols = true,
            OutputDir = "/tmp/staging/",
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: pack
                  packagesToPack: src/*.csproj
                  arguments: -c Release
                  nobuild: true
                  configurationToPack: Release
                  includesource: true
                  includesymbols: true
                  outputDir: /tmp/staging/
            """);
    }

    [Fact]
    public void Publish_Command_Test()
    {
        var task = _builder.Publish("src/*.csproj", true, "-c Release") with
        {
            ModifyOutputPath = true,
            ZipAfterPublish = true,
            Timeout = TimeSpan.FromMinutes(30),
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: publish
                  projects: src/*.csproj
                  arguments: -c Release
                  publishWebProjects: true
                  modifyOutputPath: true
                  zipAfterPublish: true
                timeoutInMinutes: 30
            """);
    }

    [Fact]
    public void Push_Command_Test()
    {
        var task = _builder.Push(arguments: "-c Release") with
        {
            PublishPackageMetadata = true,
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: push
                  packagesToPush: $(Build.ArtifactStagingDirectory)/*.nupkg
                  arguments: -c Release
                  publishPackageMetadata: true
            """);
    }

    [Fact]
    public void Test_Command_Test()
    {
        var task = _builder.Test("*.sln", "/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura") with
        {
            TestRunTitle = "main-test-results"
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: test
                  projects: '*.sln'
                  arguments: /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
                  testRunTitle: main-test-results
            """);
    }

    [Fact]
    public void Restore_Projects_Command_Test()
    {
        var task = _builder.Restore.Projects("src/*.csproj") with
        {
            NoCache = true,
            IncludeNuGetOrg = true,
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: restore
                  projects: src/*.csproj
                  noCache: true
                  includeNuGetOrg: true
            """);
    }

    [Fact]
    public void Restore_Projects_With_Config_Command_Test()
    {
        var task = _builder.Restore.Projects("src/*.csproj") with
        {
            NoCache = true,
            IncludeNuGetOrg = true,
            NuGetConfigPath = "src/nuget.config",
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: restore
                  projects: src/*.csproj
                  noCache: true
                  includeNuGetOrg: true
                  nugetConfigPath: src/nuget.config
                  feedsToUse: config
            """);
    }

    [Fact]
    public void Restore_FromFeed_Command_Test()
    {
        var task = _builder.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
        {
            ExternalFeedCredentials = "feeds/dotnet-7",
            NoCache = true,
            RestoreDirectory = ".packages",
            VerbosityRestore = BuildVerbosity.Minimal
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: restore
                  includeNuGetOrg: false
                  feedsToUse: select
                  feedRestore: dotnet-7-preview-feed
                  externalFeedCredentials: feeds/dotnet-7
                  noCache: true
                  restoreDirectory: .packages
                  verbosityRestore: minimal
            """);
    }

    [Fact]
    public void Restore_FromFeed_WithNugetConfig_Command_Test()
    {
        Action action = () =>
        {
            _ = _builder.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
            {
                ExternalFeedCredentials = "feeds/dotnet-7",
                NoCache = true,
                RestoreDirectory = ".packages",
                NuGetConfigPath = "this should cause an exception",
            };
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public void Restore_FromConfig_Command_Test()
    {
        var task = _builder.Restore.FromNuGetConfig("src/NuGet.config") with
        {
            Arguments = "foo"
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: restore
                  feedsToUse: config
                  nugetConfigPath: src/NuGet.config
                  arguments: foo
            """);
    }

    [Fact]
    public void Run_Command_Test()
    {
        var task = _builder.Run with
        {
            Projects = "src/Component/Component.csproj",
            Arguments = "FailIfChanged=true"
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: run
                  projects: src/Component/Component.csproj
                  arguments: FailIfChanged=true
            """);
    }

    [Fact]
    public void Custom_Command_Test()
    {
        var task = _builder.CustomCommand("--list-sdks") with
        {
            ContinueOnError = true,
        };

        var yaml = GetYaml(task);
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: job
              steps:
              - task: DotNetCoreCLI@2
                inputs:
                  command: custom
                  custom: --list-sdks
                continueOnError: true
            """);
    }

    private static string GetYaml(Step task) => new DotNet_Pipeline(task).Serialize().Trim();
}
