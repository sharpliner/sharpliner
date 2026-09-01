using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/advanced-security-codeql-analyze-v1">AdvancedSecurity-Codeql-Analyze@1</see>
/// task in Azure DevOps pipelines.
/// </summary>
/// <remarks>
/// <para>
/// Audited against the current <see href="https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/advanced-security-codeql-analyze-v1.md">Microsoft Learn YAML task reference</see>
/// (<c>ms.date: 07/28/2026</c>).
/// </para>
/// <para>
/// Official task inputs are <c>WaitForProcessing</c> (boolean, default <c>false</c>), <c>WaitForProcessingInterval</c>
/// (string, default <c>5</c>, used when <c>WaitForProcessing = true</c>), and <c>WaitForProcessingTimeout</c>
/// (string, default <c>120</c>, used when <c>WaitForProcessing = true</c>).
/// </para>
/// </remarks>
public record AdvancedSecurityCodeqlAnalyzeTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancedSecurityCodeqlAnalyzeTask"/> class.
    /// </summary>
    public AdvancedSecurityCodeqlAnalyzeTask()
        : base("AdvancedSecurity-Codeql-Analyze@1")
    {
        DisplayName = "Advanced Security Perform CodeQL analysis";
    }

    /// <summary>
    /// Gets or sets whether the task waits for Advanced Security to process the published SARIF file before completing.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? WaitForProcessing
    {
        get => GetExpression<bool>("WaitForProcessing");
        init => SetProperty("WaitForProcessing", value);
    }

    /// <summary>
    /// Gets or sets the wait interval in seconds between checks for SARIF processing status.
    /// Used when <see cref="WaitForProcessing"/> is <c>true</c>.
    /// Default value: <c>5</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WaitForProcessingInterval
    {
        get => GetExpression<string>("WaitForProcessingInterval");
        init => SetProperty("WaitForProcessingInterval", value);
    }

    /// <summary>
    /// Gets or sets the maximum wait timeout in seconds for SARIF processing.
    /// Used when <see cref="WaitForProcessing"/> is <c>true</c>.
    /// Default value: <c>120</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WaitForProcessingTimeout
    {
        get => GetExpression<string>("WaitForProcessingTimeout");
        init => SetProperty("WaitForProcessingTimeout", value);
    }
}
