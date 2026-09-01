using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Downloads a secure file to the agent machine using the <c>DownloadSecureFile@1</c> task.
/// The task runs at the beginning of its stage regardless of where it appears in the job,
/// and Azure Pipelines deletes the downloaded file when the job completes.
/// More details can be found in the <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/download-secure-file-v1">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DownloadSecureFileV1/task.json">official task specification</see>.
/// </summary>
public record DownloadSecureFileTask : AzureDevOpsTask
{
    /// <summary>
    /// Name of the output variable exposed by this task that contains the downloaded file location.
    /// </summary>
    public const string SecureFilePathOutputVariableName = "secureFilePath";

    /// <summary>
    /// Required. The file name or GUID of the secure file to download.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SecureFile
    {
        get => GetExpression<string>("secureFile");
        init => SetProperty("secureFile", value);
    }

    /// <summary>
    /// Optional number of retries when the secure file download fails.
    /// Default value: <c>8</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? RetryCount
    {
        get => GetExpression<int>("retryCount", 8);
        init => SetProperty("retryCount", value);
    }

    /// <summary>
    /// Optional timeout in milliseconds for the secure file download socket.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? SocketTimeout
    {
        get => GetExpression<int>("socketTimeout");
        init => SetProperty("socketTimeout", value);
    }

    /// <summary>
    /// Creates a variable reference for this task's <c>secureFilePath</c> output variable in the same job.
    /// The step must set <see cref="Step.Name"/> to the value passed in <paramref name="stepName"/>.
    /// </summary>
    /// <param name="stepName">The step name that was assigned to the <c>DownloadSecureFile@1</c> step.</param>
    /// <returns>An expression in the form <c>$(stepName.secureFilePath)</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="stepName"/> is null, empty, or whitespace.</exception>
    public static string OutputSecureFilePath(string stepName)
    {
        if (string.IsNullOrWhiteSpace(stepName))
        {
            throw new ArgumentException($"'{nameof(stepName)}' cannot be null, empty, or whitespace.", nameof(stepName));
        }

        return $"$({stepName}.{SecureFilePathOutputVariableName})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadSecureFileTask"/> class with required properties.
    /// </summary>
    /// <param name="secureFile">The file name or GUID of the secure file to download.</param>
    /// <param name="retryCount">Optional number of retries when download fails. Default value: <c>8</c>.</param>
    /// <param name="socketTimeout">Optional timeout in milliseconds for the download socket.</param>
    public DownloadSecureFileTask(AdoExpression<string> secureFile, AdoExpression<int>? retryCount = null, AdoExpression<int>? socketTimeout = null)
        : base("DownloadSecureFile@1")
    {
        SecureFile = secureFile;
        RetryCount = retryCount;
        SocketTimeout = socketTimeout;
        DisplayName = "Download secure file";
    }
}
