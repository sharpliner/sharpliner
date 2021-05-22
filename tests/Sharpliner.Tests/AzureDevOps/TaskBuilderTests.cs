using System;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps
{
    public class TaskBuilderTests
    {
        private abstract class TestPipeline : SingleStageAzureDevOpsPipelineDefinition
        {
            public override string TargetFile => "azure-pipelines.yml";

            public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
        }

        private class BashTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Bash.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.test-script.sh"),
                            Bash.FromResourceFile("Resource", "test-script.sh"),
                            Bash.Inline("Inline", "cat /etc/passwd", "rm -rf tests.xml"),
                            Bash.File("foo.sh"),
                            Bash.FromFile("Path", "AzureDevops/Resources/test-script.sh"),
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Bash_Builders_Test()
        {
            BashTaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - bash: |
      echo ""foo""
      git clone $bar

  - bash: |
      echo ""foo""
      git clone $bar
    displayName: Resource

  - bash: |-
      cat /etc/passwd
      rm -rf tests.xml
    displayName: Inline

  - bash: foo.sh

  - bash: |
      echo ""foo""
      git clone $bar
    displayName: Path
");
        }

        private class PowershellTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Powershell.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Powershell.FromResourceFile("Resource", "Test-Script.ps1"),
                            Powershell.Inline("Inline", "Connect-AzContext", "Set-AzSubscription --id foo-bar-xyz"),
                            Powershell.File("File", "foo.ps1"),
                            Powershell.FromFile("AzureDevops/Resources/Test-Script.ps1"),
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Powershell_Builders_Test()
        {
            PowershellTaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - powershell: |
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""

  - powershell: |
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
    displayName: Resource

  - powershell: |-
      Connect-AzContext
      Set-AzSubscription --id foo-bar-xyz
    displayName: Inline

  - powershell: foo.ps1
    targetType: filepath
    displayName: File

  - powershell: |
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
");
        }

        private class PwshTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Pwsh.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Pwsh.FromResourceFile("Resource", "Test-Script.ps1"),
                            Pwsh.Inline("Inline", "Connect-AzContext", "Set-AzSubscription --id foo-bar-xyz"),
                            Pwsh.File("File", "foo.ps1"),
                            Pwsh.FromFile("AzureDevops/Resources/Test-Script.ps1"),
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Pwsh_Builders_Test()
        {
            PwshTaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - powershell: |
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
    pwsh: true

  - powershell: |
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
    displayName: Resource
    pwsh: true

  - powershell: |-
      Connect-AzContext
      Set-AzSubscription --id foo-bar-xyz
    displayName: Inline
    pwsh: true

  - powershell: foo.ps1
    targetType: filepath
    displayName: File
    pwsh: true

  - powershell: |
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
    pwsh: true
");
        }

        private class PublishTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Publish("Publish artifact", "bin/Debug/net5.0/") with
                            {
                                ContinueOnError = false,
                                ArtifactType = ArtifactType.Pipeline,
                            },
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Publish_Builder_Test()
        {
            PublishTaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - publish: bin/Debug/net5.0/
    displayName: Publish artifact
    artifact: drop
");
        }

        private class CheckoutTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Checkout.None,
                            Checkout.Self,
                            Checkout.Repository("https://github.com/sharpliner/sharpliner.git") with
                            {
                                Submodules = SubmoduleCheckout.Recursive,
                                Clean = true,
                                FetchDepth = 1,
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Checkout_Builder_Test()
        {
            CheckoutTaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - checkout: none

  - checkout: self

  - checkout: https://github.com/sharpliner/sharpliner.git
    clean: true
    fetchDepth: 1
    submodules: recursive
");
        }

        private class DownloadTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Download.None,
                            Download.Current with
                            {
                                Tags = new()
                                {
                                    "release",
                                    "nightly",
                                },
                                AllowFailedBuilds = true,
                                Artifact = "Frontend",
                                Patterns = new()
                                {
                                    "frontend/**/*",
                                    "frontend.config",
                                }
                            },
                            Download.SpecificBuild("dotnet-xharness") with
                            {
                                RunVersion = RunVersion.Latest,
                                Project = "2a73171e-15d1-41f9-b283-49aa0633d1a2",
                                BranchName = "main",
                                Path = "$(Pipeline.Workspace)/xharness"
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Download_Builder_Test()
        {
            DownloadTaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - download: none

  - download: current
    artifact: Frontend
    patterns: |-
      frontend/**/*
      frontend.config
    tags: release,nightly
    allowFailedBuilds: true

  - download: dotnet-xharness
    path: $(Pipeline.Workspace)/xharness
    project: 2a73171e-15d1-41f9-b283-49aa0633d1a2
    runVersion: latest
    runBranch: main
");
        }

        private class TaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Task("Build", "VSBuild@1") with
                            {
                                Timeout = TimeSpan.FromHours(2),
                                Inputs = new()
                                {
                                    { "solution", "**/*.sln" }
                                }
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public void Serialize_Task_Builder_Test()
        {
            TaskPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
  displayName: Test job
  steps:
  - task: VSBuild@1
    displayName: Build
    inputs:
      solution: '**/*.sln'
    timeoutInMinutes: 120
");
        }
    }
}
