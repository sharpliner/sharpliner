using System;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class TaskBuilderTests
{
    private abstract class TestPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
    }

    private class BashTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
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
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: test
              steps:
              - bash: |
                  echo "foo"
                  git clone $bar

              - bash: |
                  echo "foo"
                  git clone $bar

              - bash: |-
                  cat /etc/passwd
                  rm -rf tests.xml

              - task: Bash@3
                inputs:
                  targetType: filePath
                  filePath: foo.sh

              - bash: |
                  echo "foo"
                  git clone $bar
            """);
    }

    private class PowershellTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
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
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: test
              steps:
              - powershell: |+
                  Set-ErrorActionPreference Stop
                  Write-Host "Lorem ipsum dolor sit amet"

              - powershell: |+
                  Set-ErrorActionPreference Stop
                  Write-Host "Lorem ipsum dolor sit amet"

              - powershell: |-
                  Connect-AzContext
                  Set-AzSubscription --id foo-bar-xyz

              - task: PowerShell@2
                inputs:
                  targetType: filePath
                  filePath: foo.ps1

              - powershell: |+
                  Set-ErrorActionPreference Stop
                  Write-Host "Lorem ipsum dolor sit amet"
            """);
    }

    private class PwshTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
                {
                    new Job("test")
                    {
                        Steps =
                        {
                            Pwsh.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Pwsh.FromResourceFile("Test-Script.ps1", "A display name"),
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
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: test
              steps:
              - pwsh: |+
                  Set-ErrorActionPreference Stop
                  Write-Host "Lorem ipsum dolor sit amet"

              - pwsh: |+
                  Set-ErrorActionPreference Stop
                  Write-Host "Lorem ipsum dolor sit amet"
                displayName: A display name

              - pwsh: |-
                  Connect-AzContext
                  Set-AzSubscription --id foo-bar-xyz

              - task: PowerShell@2
                inputs:
                  targetType: filePath
                  filePath: foo.ps1
                  pwsh: true

              - pwsh: |+
                  Set-ErrorActionPreference Stop
                  Write-Host "Lorem ipsum dolor sit amet"
            """);
    }

    private class ScriptTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Script.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.test-script"),
                        Script.FromResourceFile("test-script", "A display name"),
                        Script.Inline("echo 'Hello, world!'", "echo 'Goodbye, world!'") with { DisplayName = "A display name" },
                        Script.FromFile("AzureDevOps/Resources/test-script"),
                    }
                }
            }
        };
    }

    [Fact]
    public void Serialize_Script_Builders_Test()
    {
        ScriptTaskPipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: test
              steps:
              - script: |+
                  dir src
                  echo "Hello World!"

              - script: |+
                  dir src
                  echo "Hello World!"
                displayName: A display name

              - script: |-
                  echo 'Hello, world!'
                  echo 'Goodbye, world!'

              - script: |+
                  dir src
                  echo "Hello World!"
            """);
    }

    private class PublishTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Publish("Binary", "bin/Debug/net8.0/", "Publish artifact") with
                        {
                            ContinueOnError = false,
                            ArtifactType = ArtifactType.Pipeline,
                        },

                        Publish("artifactName", "some/file/path.txt"),
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
        yaml.Trim().Should().Be(
        """
        jobs:
        - job: test
          steps:
          - publish: bin/Debug/net8.0/
            displayName: Publish artifact
            artifact: Binary
            continueOnError: false

          - publish: some/file/path.txt
            artifact: artifactName
        """);
    }

    private class CheckoutTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
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
                            FetchDepth = 0,
                            FetchFilter = "tree:0",
                            FetchTags = true,
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
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: test
              steps:
              - checkout: none

              - checkout: self

              - checkout: https://github.com/sharpliner/sharpliner.git
                clean: true
                fetchDepth: 0
                fetchFilter: tree:0
                fetchTags: true
                submodules: recursive
            """);
    }

    private class DownloadTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
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
                            Tags =
                            [
                                "release",
                                "nightly",
                            ],
                            Artifact = "Frontend",
                            Patterns =
                            [
                                "frontend/**/*",
                                "frontend.config",
                            ]
                        },
                        Download.SpecificBuild("public", 56, 1745, "MyProject.CLI", patterns: [ "**/*.dll", "**/*.exe" ]) with
                        {
                            AllowPartiallySucceededBuilds = true,
                            RetryDownloadCount = 3,
                            Tags = ["non-release", "preview"],
                        },
                        Download.LatestFromBranch("internal", 23, "refs/heads/develop", path: variables.Build.ArtifactStagingDirectory) with
                        {
                            AllowFailedBuilds = true,
                            CheckDownloadedFiles = true,
                            PreferTriggeringPipeline = true,
                            Artifact = "Another.CLI",
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
        yaml.Trim().Should().Be(
            """
            jobs:
            - job: test
              steps:
              - download: none

              - download: current
                artifact: Frontend
                patterns: |-
                  frontend/**/*
                  frontend.config
                tags: release,nightly

              - task: DownloadPipelineArtifact@2
                inputs:
                  runVersion: specific
                  project: public
                  pipeline: 56
                  runId: 1745
                  artifact: MyProject.CLI
                  patterns: |-
                    **/*.dll
                    **/*.exe
                  allowPartiallySucceededBuilds: true
                  retryDownloadCount: 3
                  tags: non-release,preview

              - task: DownloadPipelineArtifact@2
                inputs:
                  runVersion: latestFromBranch
                  project: internal
                  pipeline: 23
                  runBranch: refs/heads/develop
                  path: $(Build.ArtifactStagingDirectory)
                  allowFailedBuilds: true
                  checkDownloadedFiles: true
                  preferTriggeringPipeline: true
                  artifact: Another.CLI
            """);
    }

    private class TaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
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
        yaml.Trim().Should().Be(
        """
        jobs:
        - job: test
          steps:
          - task: VSBuild@1
            displayName: Build
            inputs:
              solution: '**/*.sln'
            timeoutInMinutes: 120
        """);
    }
}
