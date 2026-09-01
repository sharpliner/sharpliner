using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class MavenTaskTests
{
    private readonly MavenTaskBuilder _builder = new();

    private class Maven_Pipeline(Step step) : SimpleTestPipeline
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

    private class Maven_Builder_Pipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job")
                {
                    Steps =
                    {
                        Maven.Build()
                    }
                }
            }
        };
    }

    [Fact]
    public Task Builder_Default_Maven4_Test()
        => Verify(new Maven_Builder_Pipeline().Serialize());

    [Fact]
    public Task Maven4_JUnit_And_JaCoCo_Coverage_Test()
    {
        var task = _builder.Build("eng/Sharpliner.CI/pom.xml", "verify", "-DskipITs=false") with
        {
            PublishJUnitResults = true,
            TestResultsFiles = "**/surefire-reports/TEST-*.xml",
            TestRunTitle = "Sharpliner Maven tests",
            AllowBrokenSymbolicLinks = false,
            CodeCoverageTool = MavenCodeCoverageTool.JaCoCo,
            ClassFilter = "+:com.sharpliner.*,-:com.sharpliner.generated.*",
            ClassFilesDirectories = "target/classes,target/test-classes",
            SrcDirectories = "src/main/java,src/test/java",
            FailIfCoverageEmpty = true,
            RestoreOriginalPomXml = true,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Maven4_Advanced_Options_And_Feed_Authentication_Test()
    {
        var task = _builder.Build(goals: "package", options: "-DskipTests") with
        {
            JavaHomeSelection = MavenJavaHomeSelection.Path,
            JdkUserInputPath = "$(Agent.ToolsDirectory)/jdk-21",
            MavenVersionSelection = MavenVersionSelection.Path,
            MavenPath = "/usr/share/maven-custom",
            MavenSetM2Home = true,
            MavenOpts = "-Xmx2048m -Dhttps.protocols=TLSv1.2",
            MavenFeedAuthenticate = true,
            SkipEffectivePom = true,
            ConnectedServiceName = "sharpliner-integration-tests",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Maven4_SonarQube_And_SpotBugs_Test()
    {
        var task = _builder.Build("pom.xml", "verify", "-Pci") with
        {
            CodeCoverageTool = MavenCodeCoverageTool.JaCoCo,
            SqAnalysisEnabled = true,
            IsJacocoCoverageReportXML = true,
            SqMavenPluginVersionChoice = MavenSonarQubeMavenPluginVersionChoice.Pom,
            CheckstyleAnalysisEnabled = true,
            PmdAnalysisEnabled = true,
            FindbugsAnalysisEnabled = true,
            SpotBugsAnalysisEnabled = true,
            SpotBugsMavenPluginVersion = "4.9.8.1",
            SpotBugsGoal = MavenSpotBugsGoal.Check,
            SpotBugsFailWhenBugsFound = false,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Maven1_Deprecated_SonarQube_Test()
    {
        var task = _builder.BuildV1("legacy/pom.xml", "package sonar:sonar") with
        {
            JdkVersion = MavenV1JdkVersion.Jdk8,
            JdkArchitecture = MavenV1JdkArchitecture.X64,
            SqAnalysisEnabled = true,
            SonarQubeServiceEndpoint = "legacy-sonarqube",
            SonarQubeProjectName = "Sharpliner.Legacy",
            SonarQubeProjectKey = "Sharpliner.Legacy",
            SonarQubeProjectVersion = "1.0.0",
            SonarQubeSpecifyDB = true,
            SonarQubeDBUrl = "jdbc:sqlserver://legacy-db:1433;databaseName=sonarqube",
            SonarQubeDBUsername = "sonar",
            SonarQubeDBPassword = "$(SonarPassword)",
            SonarQubeIncludeFullReport = false,
            SonarQubeFailWhenQualityGateFails = true,
            CheckstyleAnalysisEnabled = true,
            PmdAnalysisEnabled = true,
            FindbugsAnalysisEnabled = true,
        };

        return Verify(GetYaml(task));
    }

    private static string GetYaml(Step task) => new Maven_Pipeline(task).Serialize();
}
