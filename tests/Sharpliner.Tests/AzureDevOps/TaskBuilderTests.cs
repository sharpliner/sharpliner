using System.Drawing;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps
{
    public class TaskBuilderTests
    {
        private class BashTaskPipeline : SingleStageAzureDevOpsPipelineDefinition
        {
            public override string TargetFile => "azure-pipelines.yml";

            public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Bash.FromResourceFile("Resource full name", "Sharpliner.Tests.AzureDevOps.Resources.test-script.sh"),
                            Bash.FromResourceFile("Resource", "test-script.sh"),
                            Bash.Inline("Inline", "cat /etc/passwd", "rm -rf tests.xml"),
                            Bash.File("File", "foo.sh"),
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
    displayName: Resource full name

  - bash: |
      echo ""foo""
      git clone $bar
    displayName: Resource

  - bash: |-
      cat /etc/passwd
      rm -rf tests.xml
    displayName: Inline

  - bash: foo.sh
    displayName: File

  - bash: |
      echo ""foo""
      git clone $bar
    displayName: Path
");
        }

        private class PowershellTaskPipeline : SingleStageAzureDevOpsPipelineDefinition
        {
            public override string TargetFile => "azure-pipelines.yml";

            public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

            public override SingleStageAzureDevOpsPipeline Pipeline => new()
            {
                Jobs =
                {
                    new Job("test", "Test job")
                    {
                        Steps =
                        {
                            Powershell.FromResourceFile("Resource full name", "Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Powershell.FromResourceFile("Resource", "Test-Script.ps1"),
                            Powershell.Inline("Inline", "Connect-AzContext", "Set-AzSubscription --id foo-bar-xyz"),
                            Powershell.File("File", "foo.ps1"),
                            Powershell.FromFile("Path", "AzureDevops/Resources/Test-Script.ps1"),
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
    displayName: Resource full name

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
    displayName: Path
");
        }
    }
}
