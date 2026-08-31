using System;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating Gradle tasks using the <c>Gradle</c> keyword.
/// The generated YAML uses the <c>Gradle@4</c> task as defined by the
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/GradleV4/task.json">official GradleV4 task specification audited on 2026-08-31</see>.
/// </summary>
public class GradleTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates a Gradle task for common build-style task lists.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.Build("clean build", displayName: "Gradle build")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   displayName: Gradle build
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: clean build
    /// </code>
    /// </summary>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/>.</returns>
    public GradleTask Build(string tasks = "build", string wrapperScript = "gradlew", AdoExpression<string>? displayName = null)
        => CreateTask(tasks, wrapperScript, displayName);

    /// <summary>
    /// Creates a Gradle task configured to publish JUnit test results to Azure Pipelines.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.Test("clean test", testRunTitle: "Gradle unit tests")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: clean test
    ///     publishJUnitResults: true
    ///     testResultsFiles: **/TEST-*.xml
    ///     testRunTitle: Gradle unit tests
    /// </code>
    /// </summary>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="testResultsFiles">JUnit results file pattern.</param>
    /// <param name="testRunTitle">Optional Azure Pipelines test run title.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/> configured for JUnit publication.</returns>
    public GradleTask Test(string tasks = "test", string wrapperScript = "gradlew", string testResultsFiles = "**/TEST-*.xml", string? testRunTitle = null, AdoExpression<string>? displayName = null)
        => CreateTask(tasks, wrapperScript, displayName) with
        {
            PublishJUnitResults = true,
            TestResultsFiles = testResultsFiles,
            TestRunTitle = testRunTitle,
        };

    /// <summary>
    /// Creates a Gradle task that discovers a JDK version on the agent and sets <c>JAVA_HOME</c> accordingly.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.UseJdkVersion("build", jdkVersion: "1.17", jdkArchitecture: JdkArchitecture.X64)
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: build
    ///     javaHomeSelection: JDKVersion
    ///     jdkVersion: '1.17'
    ///     jdkArchitecture: x64
    /// </code>
    /// </summary>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="jdkVersion">JDK version to discover. Allowed values are <c>default</c>, <c>1.17</c>, <c>1.11</c>, <c>1.10</c>, <c>1.9</c>, <c>1.8</c>, <c>1.7</c>, and <c>1.6</c>.</param>
    /// <param name="jdkArchitecture">Optional JDK architecture. This is emitted only when <paramref name="jdkVersion"/> is not <c>default</c>.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/> configured to discover a JDK version.</returns>
    public GradleTask UseJdkVersion(string tasks = "build", string wrapperScript = "gradlew", string jdkVersion = "default", JdkArchitecture? jdkArchitecture = null, AdoExpression<string>? displayName = null)
    {
        var task = CreateTask(tasks, wrapperScript, displayName) with
        {
            JavaHomeSelection = Tasks.JavaHomeSelection.JdkVersion,
            JdkVersion = jdkVersion,
        };

        if (!string.Equals(jdkVersion, "default", StringComparison.Ordinal))
        {
            task = task with
            {
                JdkArchitecture = jdkArchitecture,
            };
        }

        return task;
    }

    /// <summary>
    /// Creates a Gradle task that sets <c>JAVA_HOME</c> from a specific path.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.UseJdkPath("$(JAVA_HOME_17_X64)", "clean build")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: clean build
    ///     javaHomeSelection: Path
    ///     jdkUserInputPath: $(JAVA_HOME_17_X64)
    /// </code>
    /// </summary>
    /// <param name="jdkUserInputPath">JDK path to assign to <c>JAVA_HOME</c>.</param>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/> configured to use a specific JDK path.</returns>
    public GradleTask UseJdkPath(string jdkUserInputPath, string tasks = "build", string wrapperScript = "gradlew", AdoExpression<string>? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(jdkUserInputPath))
        {
            throw new ArgumentException($"'{nameof(jdkUserInputPath)}' cannot be null, empty, or whitespace.", nameof(jdkUserInputPath));
        }

        return CreateTask(tasks, wrapperScript, displayName) with
        {
            JavaHomeSelection = Tasks.JavaHomeSelection.Path,
            JdkUserInputPath = jdkUserInputPath,
        };
    }

    /// <summary>
    /// Creates a Gradle task that enables SonarQube or SonarCloud analysis.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.SonarQubeAnalysis("build", pluginVersionChoice: GradlePluginVersionChoice.Build)
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: build
    ///     sonarQubeRunAnalysis: true
    ///     sqGradlePluginVersionChoice: build
    /// </code>
    /// </summary>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="pluginVersionChoice">Whether to specify the SonarQube plugin version in the task or use the version declared in <c>build.gradle</c>.</param>
    /// <param name="pluginVersion">SonarQube Gradle plugin version to emit when <paramref name="pluginVersionChoice"/> is <see cref="Tasks.GradlePluginVersionChoice.Specify"/>.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/> configured for SonarQube analysis.</returns>
    public GradleTask SonarQubeAnalysis(string tasks = "build", string wrapperScript = "gradlew", GradlePluginVersionChoice pluginVersionChoice = Tasks.GradlePluginVersionChoice.Specify, string pluginVersion = "2.6.1", AdoExpression<string>? displayName = null)
    {
        var task = CreateTask(tasks, wrapperScript, displayName) with
        {
            SonarQubeRunAnalysis = true,
            SonarQubeGradlePluginVersionChoice = pluginVersionChoice,
        };

        if (pluginVersionChoice == Tasks.GradlePluginVersionChoice.Specify)
        {
            task = task with
            {
                SonarQubeGradlePluginVersion = pluginVersion,
            };
        }

        return task;
    }

    /// <summary>
    /// Creates a Gradle task that enables Checkstyle, FindBugs, and/or PMD.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.StaticAnalysis("build", checkstyle: true, pmd: true)
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: build
    ///     checkstyleAnalysisEnabled: true
    ///     pmdAnalysisEnabled: true
    /// </code>
    /// </summary>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="checkstyle">Enables Checkstyle.</param>
    /// <param name="findBugs">Enables FindBugs. This plugin was removed in Gradle 6.0; prefer SpotBugs for newer builds.</param>
    /// <param name="pmd">Enables PMD.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/> configured for static analysis.</returns>
    public GradleTask StaticAnalysis(string tasks = "build", string wrapperScript = "gradlew", bool checkstyle = false, bool findBugs = false, bool pmd = false, AdoExpression<string>? displayName = null)
    {
        if (!checkstyle && !findBugs && !pmd)
        {
            throw new ArgumentException("At least one static analysis tool must be enabled.", nameof(checkstyle));
        }

        return CreateTask(tasks, wrapperScript, displayName) with
        {
            CheckstyleAnalysisEnabled = checkstyle ? true : null,
            FindBugsAnalysisEnabled = findBugs ? true : null,
            PmdAnalysisEnabled = pmd ? true : null,
        };
    }

    /// <summary>
    /// Creates a Gradle task that enables SpotBugs analysis.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Gradle.SpotBugsAnalysis("check", pluginVersionChoice: GradlePluginVersionChoice.Build)
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Gradle@4
    ///   inputs:
    ///     gradleWrapperFile: gradlew
    ///     tasks: check
    ///     spotBugsAnalysisEnabled: true
    ///     spotBugsGradlePluginVersionChoice: build
    /// </code>
    /// </summary>
    /// <param name="tasks">Space-separated Gradle tasks to run.</param>
    /// <param name="wrapperScript">Relative path from the repository root to the Gradle Wrapper script.</param>
    /// <param name="pluginVersionChoice">Whether to specify the SpotBugs plugin version in the task or use the version declared in <c>build.gradle</c>.</param>
    /// <param name="pluginVersion">SpotBugs Gradle plugin version to emit when <paramref name="pluginVersionChoice"/> is <see cref="Tasks.GradlePluginVersionChoice.Specify"/>.</param>
    /// <param name="displayName">Optional display name of the step.</param>
    /// <returns>A new instance of <see cref="GradleTask"/> configured for SpotBugs analysis.</returns>
    public GradleTask SpotBugsAnalysis(string tasks = "build", string wrapperScript = "gradlew", GradlePluginVersionChoice pluginVersionChoice = Tasks.GradlePluginVersionChoice.Specify, string pluginVersion = "4.7.0", AdoExpression<string>? displayName = null)
    {
        var task = CreateTask(tasks, wrapperScript, displayName) with
        {
            SpotBugsAnalysisEnabled = true,
            SpotBugsGradlePluginVersionChoice = pluginVersionChoice,
        };

        if (pluginVersionChoice == Tasks.GradlePluginVersionChoice.Specify)
        {
            task = task with
            {
                SpotBugsGradlePluginVersion = pluginVersion,
            };
        }

        return task;
    }

    internal GradleTaskBuilder()
    {
    }

    private static GradleTask CreateTask(string tasks, string wrapperScript, AdoExpression<string>? displayName)
        => new(wrapperScript, tasks)
        {
            DisplayName = displayName!,
        };
}
