using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetAuthenticate@1 task in Azure DevOps pipelines.
    /// </summary>
    public record NuGetAuthenticateTask : AzureDevOpsTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetAuthenticateTask"/> class.
        /// </summary>
        public NuGetAuthenticateTask() : base("NuGetAuthenticate@1")
        {
        }

        /// <summary>
        /// Gets or sets the NuGet service connections.
        /// </summary>
        [YamlIgnore]
        public string[]? NuGetServiceConnections
        {
            get => GetString("nuGetServiceConnections")?.Split(',');
            init => SetProperty("nuGetServiceConnections", string.Join(",", value!));
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force reinstall of the credential provider even if already installed. Default: false.
        /// </summary>
        [YamlIgnore]
        public bool ForceReinstallCredentialProvider
        {
            get => GetBool("forceReinstallCredentialProvider", false);
            init => SetProperty("forceReinstallCredentialProvider", value);
        }
    }
}
