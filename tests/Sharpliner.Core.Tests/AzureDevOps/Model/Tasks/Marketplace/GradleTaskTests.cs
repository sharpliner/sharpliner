using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class GradleTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new GradleTask();

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_JUnit_Results_Test()
    {
        var task = new GradleTask("eng/gradlew", "clean test")
        {
            PublishJUnitResults = true,
            TestResultsFiles = "**/build/test-results/test/TEST-*.xml",
            TestRunTitle = "Gradle tests",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Jdk_Path_Test()
    {
        var task = new GradleTask("gradlew", "build")
        {
            JavaHomeSelection = JavaHomeSelection.Path,
            JdkUserInputPath = "$(JAVA_HOME_17_X64)",
            WorkingDirectory = "src/app",
            GradleOptions = "-Xmx2048m",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Jdk_Version_And_Architecture_Test()
    {
        var task = new GradleTask(new BuildVariableReference().SourcesDirectory, new ParameterReference("gradleTasks"))
        {
            JavaHomeSelection = JavaHomeSelection.JdkVersion,
            JdkVersion = "1.17",
            JdkArchitecture = JdkArchitecture.Arm64,
            Options = "--info",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_SonarQube_Analysis_Test()
    {
        var task = new GradleTask("gradlew", "build sonar")
        {
            SonarQubeRunAnalysis = true,
            SonarQubeGradlePluginVersionChoice = GradlePluginVersionChoice.Specify,
            SonarQubeGradlePluginVersion = "4.4.1.3373",
            CheckstyleAnalysisEnabled = true,
            PmdAnalysisEnabled = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_SpotBugs_Analysis_Test()
    {
        var task = new GradleTask("gradlew", "check")
        {
            SpotBugsAnalysisEnabled = true,
            SpotBugsGradlePluginVersionChoice = GradlePluginVersionChoice.Build,
            FindBugsAnalysisEnabled = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
