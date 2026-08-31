using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>UseNode@1</c> tasks.
/// </summary>
public class NodeTaskBuilder
{
    internal NodeTaskBuilder()
    {
    }

    /// <summary>
    /// Gets a <see cref="NodeInstallerBuilder"/> instance to create Node.js installer tasks.
    /// </summary>
    public NodeInstallerBuilder Install => new();

    /// <summary>
    /// Builder for creating <c>UseNode@1</c> tasks.
    /// </summary>
    public class NodeInstallerBuilder
    {
        internal NodeInstallerBuilder()
        {
        }

        /// <summary>
        /// Creates a <see cref="UseNodeTask"/> that installs Node.js based on a version specification.
        /// </summary>
        /// <param name="version">Version spec of Node.js to use (for example <c>20.x</c>, <c>20.18.0</c>, or <c>&gt;=20.10.0</c>).</param>
        /// <param name="checkLatest">When true, always checks online for the latest version matching <paramref name="version"/>. Default: <c>false</c>.</param>
        /// <param name="architecture">Desired architecture. Serialized as the task's <c>force32bit</c> input. Default: <see cref="NodeArchitecture.X64"/>.</param>
        /// <param name="nodejsMirror">Alternative mirror URL used to download Node.js binaries. Default: <c>https://nodejs.org/dist</c>.</param>
        /// <param name="retryCountOnDownloadFails">Number of retries when downloads fail. Default: <c>5</c>.</param>
        /// <param name="delayBetweenRetries">Delay between retries in milliseconds. Default: <c>1000</c>.</param>
        public UseNodeTask Version(
            AdoExpression<string> version,
            AdoExpression<bool>? checkLatest = null,
            NodeArchitecture architecture = NodeArchitecture.X64,
            AdoExpression<string>? nodejsMirror = null,
            AdoExpression<int>? retryCountOnDownloadFails = null,
            AdoExpression<int>? delayBetweenRetries = null)
            => new()
            {
                VersionSource = UseNodeVersionSource.Spec,
                Version = version,
                CheckLatest = checkLatest,
                Architecture = architecture,
                NodejsMirror = nodejsMirror,
                RetryCountOnDownloadFails = retryCountOnDownloadFails,
                DelayBetweenRetries = delayBetweenRetries,
            };

        /// <summary>
        /// Creates a <see cref="UseNodeTask"/> that reads the version from a file (for example <c>.nvmrc</c>).
        /// </summary>
        /// <param name="versionFilePath">Path to a file containing the Node.js version.</param>
        /// <param name="checkLatest">When true, always checks online for the latest version matching the file content. Default: <c>false</c>.</param>
        /// <param name="architecture">Desired architecture. Serialized as the task's <c>force32bit</c> input. Default: <see cref="NodeArchitecture.X64"/>.</param>
        /// <param name="nodejsMirror">Alternative mirror URL used to download Node.js binaries. Default: <c>https://nodejs.org/dist</c>.</param>
        /// <param name="retryCountOnDownloadFails">Number of retries when downloads fail. Default: <c>5</c>.</param>
        /// <param name="delayBetweenRetries">Delay between retries in milliseconds. Default: <c>1000</c>.</param>
        public UseNodeTask FromFile(
            AdoExpression<string> versionFilePath,
            AdoExpression<bool>? checkLatest = null,
            NodeArchitecture architecture = NodeArchitecture.X64,
            AdoExpression<string>? nodejsMirror = null,
            AdoExpression<int>? retryCountOnDownloadFails = null,
            AdoExpression<int>? delayBetweenRetries = null)
            => new()
            {
                VersionSource = UseNodeVersionSource.FromFile,
                VersionFilePath = versionFilePath,
                CheckLatest = checkLatest,
                Architecture = architecture,
                NodejsMirror = nodejsMirror,
                RetryCountOnDownloadFails = retryCountOnDownloadFails,
                DelayBetweenRetries = delayBetweenRetries,
            };
    }
}
