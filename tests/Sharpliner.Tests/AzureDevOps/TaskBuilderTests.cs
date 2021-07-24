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
                    new Job("test")
                    {
                        Steps =
                        {
                            Bash.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.test-script.sh"),
                            Bash.FromResourceFile("test-script.sh"),
                            Bash.Inline("cat /etc/passwd", "rm -rf tests.xml"),
                            Bash.File("foo.sh"),
                            Bash.FromFile( "AzureDevops/Resources/test-script.sh"),
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
  steps:
  - bash: |
      echo ""foo""
      git clone $bar

  - bash: |
      echo ""foo""
      git clone $bar

  - bash: |-
      cat /etc/passwd
      rm -rf tests.xml

  - bash: foo.sh

  - bash: |
      echo ""foo""
      git clone $bar
");
        }

        private class PowershellTaskPipeline : TestPipeline
        {
            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test")
                    {
                        Steps =
                        {
                            Powershell.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Powershell.FromResourceFile("Test-Script.ps1"),
                            Powershell.Inline("Connect-AzContext", "Set-AzSubscription --id foo-bar-xyz"),
                            Powershell.File("foo.ps1"),
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
  steps:
  - powershell: |+
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""

  - powershell: |+
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""

  - powershell: |-
      Connect-AzContext
      Set-AzSubscription --id foo-bar-xyz

  - powershell: foo.ps1
    targetType: filepath

  - powershell: |+
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
                    new Job("test")
                    {
                        Steps =
                        {
                            Pwsh.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Pwsh.FromResourceFile("Test-Script.ps1"),
                            Pwsh.Inline("Connect-AzContext", "Set-AzSubscription --id foo-bar-xyz"),
                            Pwsh.File("foo.ps1"),
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
  steps:
  - powershell: |+
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
    pwsh: true

  - powershell: |+
      Set-ErrorActionPreference Stop
      Write-Host ""Lorem ipsum dolor sit amet""
    pwsh: true

  - powershell: |-
      Connect-AzContext
      Set-AzSubscription --id foo-bar-xyz
    pwsh: true

  - powershell: foo.ps1
    targetType: filepath
    pwsh: true

  - powershell: |+
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
                    new Job("test")
                    {
                        Steps =
                        {
                            Publish("bin/Debug/net5.0/", "Publish artifact") with
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
                    new Job("test")
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
                    new Job("test")
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
                    new Job("test")
                    {
                        Steps =
                        {
                            Task("VSBuild@1", "Build") with
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
  steps:
  - task: VSBuild@1
    displayName: Build
    inputs:
      solution: '**/*.sln'
    timeoutInMinutes: 120
");
        }

        [Fact]
        public void Serialize_XHarness_Pipeline_Test()
        {
            XHarnessPipeline pipeline = new();
            string yaml = pipeline.Serialize();
            yaml.Should().Be(
@"jobs:
- job: test
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
