using System;
using System.Globalization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>AdvancedSecurity-Codeql-Analyze@1</c> tasks.
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

    internal AdvancedSecurityCodeqlTaskBuilder()
    {
    }
}
