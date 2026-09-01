using System;
using System.Globalization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for Advanced Security tasks.
/// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/advanced-security-codeql-init-v1">AdvancedSecurity-Codeql-Init@1</see>.
/// </summary>
public class AdvancedSecurityTaskBuilder
{
    /// <summary>
    /// Gets a builder for CodeQL tasks.
    /// </summary>
    public AdvancedSecurityCodeqlTaskBuilder Codeql => new();

    internal AdvancedSecurityTaskBuilder()
    {
    }
}

/// <summary>
/// Builder for Advanced Security CodeQL tasks.
/// </summary>
public class AdvancedSecurityCodeqlTaskBuilder
{
    /// <summary>
    /// Creates the default CodeQL analyze task.
    /// </summary>
    public AdvancedSecurityCodeqlAnalyzeTask Analyze => new();

    /// <summary>
    /// Creates a CodeQL analyze task that waits for Advanced Security processing to complete.
    /// </summary>
    /// <param name="waitForProcessingIntervalSeconds">
    /// Optional polling interval, in seconds. Must be greater than zero when specified.
    /// </param>
    /// <param name="waitForProcessingTimeoutSeconds">
    /// Optional overall timeout, in seconds. Must be greater than zero when specified.
    /// </param>
    public AdvancedSecurityCodeqlAnalyzeTask AnalyzeAndWait(int? waitForProcessingIntervalSeconds = null, int? waitForProcessingTimeoutSeconds = null)
    {
        if (waitForProcessingIntervalSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(waitForProcessingIntervalSeconds), "The value must be greater than zero when specified.");
        }

        if (waitForProcessingTimeoutSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(waitForProcessingTimeoutSeconds), "The value must be greater than zero when specified.");
        }

        return new AdvancedSecurityCodeqlAnalyzeTask
        {
            WaitForProcessing = true,
            WaitForProcessingInterval = waitForProcessingIntervalSeconds?.ToString(CultureInfo.InvariantCulture),
            WaitForProcessingTimeout = waitForProcessingTimeoutSeconds?.ToString(CultureInfo.InvariantCulture),
        };
    }

    /// <summary>
    /// Creates <c>AdvancedSecurity-Codeql-Init@1</c> in manual build mode.
    /// </summary>
    /// <param name="languages">One or more languages to analyze.</param>
    /// <returns>A new <see cref="AdvancedSecurityCodeqlInitTask"/> instance.</returns>
    public AdvancedSecurityCodeqlInitTask Init(params CodeqlLanguage[] languages) => new(languages);

    /// <summary>
    /// Creates <c>AdvancedSecurity-Codeql-Init@1</c> in no-build mode (<c>buildtype: None</c>).
    /// </summary>
    /// <param name="languages">One or more languages to analyze.</param>
    /// <returns>A new <see cref="AdvancedSecurityCodeqlInitTask"/> instance with <see cref="AdvancedSecurityCodeqlInitTask.BuildType"/> set to <see cref="CodeqlBuildType.None"/>.</returns>
    public AdvancedSecurityCodeqlInitTask InitWithoutBuild(params CodeqlLanguage[] languages) => new(languages)
    {
        BuildType = CodeqlBuildType.None,
    };

    /// <summary>
    /// Creates <c>AdvancedSecurity-Codeql-Init@1</c> with automatic CodeQL install enabled.
    /// </summary>
    /// <param name="languages">One or more languages to analyze.</param>
    /// <param name="cleanupOldAutomaticInstalls">
    /// Optional value for <see cref="AdvancedSecurityCodeqlInitTask.CleanupOldAutomaticInstalls"/>.
    /// </param>
    /// <returns>A new <see cref="AdvancedSecurityCodeqlInitTask"/> instance.</returns>
    public AdvancedSecurityCodeqlInitTask InitWithAutomaticInstall(CodeqlLanguage[] languages, bool? cleanupOldAutomaticInstalls = null)
    {
        var task = new AdvancedSecurityCodeqlInitTask(languages)
        {
            EnableAutomaticCodeQLInstall = true,
        };

        if (cleanupOldAutomaticInstalls is not null)
        {
            task = task with
            {
                CleanupOldAutomaticInstalls = cleanupOldAutomaticInstalls,
            };
        }

        return task;
    }

    internal AdvancedSecurityCodeqlTaskBuilder()
    {
    }
}
