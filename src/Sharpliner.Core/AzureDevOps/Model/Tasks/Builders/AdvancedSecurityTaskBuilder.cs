using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides methods to create GitHub Advanced Security for Azure DevOps tasks.
/// </summary>
public class AdvancedSecurityTaskBuilder
{
    /// <summary>
    /// Creates an <see cref="AdvancedSecurityPublishTask"/> that publishes SARIF files from the specified directory.
    /// </summary>
    /// <param name="sarifsInputDirectory">The directory containing SARIF files to publish.</param>
    /// <returns>A new <see cref="AdvancedSecurityPublishTask"/> instance.</returns>
    public AdvancedSecurityPublishTask PublishResults(AdoExpression<string> sarifsInputDirectory)
    {
        return new()
        {
            SarifsInputDirectory = sarifsInputDirectory,
        };
    }

    /// <summary>
    /// Creates an <see cref="AdvancedSecurityPublishTask"/> that publishes SARIF files and waits for their processing.
    /// </summary>
    /// <param name="sarifsInputDirectory">The directory containing SARIF files to publish.</param>
    /// <param name="interval">Optional polling interval in seconds. The task defaults to <c>5</c>.</param>
    /// <param name="timeout">Optional maximum wait time in seconds. The task defaults to <c>120</c>.</param>
    /// <returns>A new <see cref="AdvancedSecurityPublishTask"/> instance configured to wait for processing.</returns>
    public AdvancedSecurityPublishTask PublishResultsAndWait(
        AdoExpression<string> sarifsInputDirectory,
        AdoExpression<string>? interval = null,
        AdoExpression<string>? timeout = null)
    {
        return new()
        {
            SarifsInputDirectory = sarifsInputDirectory,
            WaitForProcessing = true,
            WaitForProcessingInterval = interval,
            WaitForProcessingTimeout = timeout,
        };
    }
}
