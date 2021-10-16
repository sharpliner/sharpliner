using System;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps
{
    public class DotNetCoreCliTests
    {
        private readonly DotNetTaskBuilder _builder = new();

        private class DotNet_Pipeline : SimpleTestPipeline
        {
            private readonly DotNetCoreCliTask _task;

            public DotNet_Pipeline(DotNetCoreCliTask task)
            {
                _task = task;
            }

            public override SingleStagePipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("job")
                    {
                        Steps = { _task }
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
            yaml.Should().Be(@"jobs:
- job: job
  steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: build
      projects: project.csproj
      arguments: -c Release
      includeNuGetOrg: true
      workingDirectory: /tmp");
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
            yaml.Should().Be(@"jobs:
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
      outputDir: /tmp/staging/");
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
            yaml.Should().Be(@"jobs:
- job: job
  steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: public
      projects: src/*.csproj
      arguments: -c Release
      publishWebProjects: true
      modifyOutputPath: true
      zipAfterPublish: true
    timeoutInMinutes: 30");
        }

        [Fact]
        public void Push_Command_Test()
        {
            var task = _builder.Push(arguments: "-c Release") with
            {
                PublishPackageMetadata = true,
            };

            var yaml = GetYaml(task);
            yaml.Should().Be(@"jobs:
- job: job
  steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: push
      packagesToPush: $(Build.ArtifactStagingDirectory)/*.nupkg
      arguments: -c Release
      publishPackageMetadata: true");
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
            yaml.Should().Be(@"jobs:
- job: job
  steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: restore
      projects: src/*.csproj
      noCache: true
      includeNuGetOrg: true");
        }

        [Fact]
        public void Restore_FromFeed_Command_Test()
        {
            var task = _builder.Restore.FromNuGetConfig("src/NuGet.config") with
            {
                Arguments = "foo"
            };

            var yaml = GetYaml(task);
            yaml.Should().Be(@"jobs:
- job: job
  steps:
  - task: DotNetCoreCLI@2
    inputs:
      command: restore
      feedsToUse: config
      nugetConfigPath: src/NuGet.config
      arguments: foo");
        }

        private static string GetYaml(DotNetCoreCliTask task) => new DotNet_Pipeline(task).Serialize().Trim();
    }
}
