using System;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.AzureDevOps.Tasks
{
    public class NuGetTaskBuilder
    {
        public NuGetAuthenticateTask Authenticate(string[]? nuGetServiceConnections = null, bool forceReinstallCredentialProvider = false)
        {
            return new NuGetAuthenticateTask
            {
                NuGetServiceConnections = nuGetServiceConnections,
                ForceReinstallCredentialProvider = forceReinstallCredentialProvider
            };
        }

        public NuGetRestoreBuilder Restore => new();
        public NuGetPushBuilder Push => new();
        public NuGetPackBuilder Pack => new();
        public NuGetCustomBuilder Custom => new();
    }

    public class NuGetAuthenticateTask : AzureDevOpsTask
    {
        public string[]? NuGetServiceConnections { get; init; }
        public bool ForceReinstallCredentialProvider { get; init; }

        public NuGetAuthenticateTask() : base("NuGetAuthenticate@1")
        {
        }
    }

    public class NuGetRestoreBuilder
    {
        public NuGetRestoreCommandTask FromFeed(string feed, bool? includeNuGetOrg = null)
        {
            return new NuGetRestoreCommandTask
            {
                Feed = feed,
                IncludeNuGetOrg = includeNuGetOrg
            };
        }

        public NuGetRestoreCommandTask FromNuGetConfig(string nugetConfigPath)
        {
            return new NuGetRestoreCommandTask
            {
                NuGetConfigPath = nugetConfigPath
            };
        }
    }

    public class NuGetPushBuilder
    {
        public NuGetPushCommandTask ToInternalFeed(string targetFeed)
        {
            return new NuGetPushCommandTask
            {
                TargetFeed = targetFeed
            };
        }

        public NuGetPushCommandTask ToExternalFeed(string targetFeedCredentials)
        {
            return new NuGetPushCommandTask
            {
                TargetFeedCredentials = targetFeedCredentials
            };
        }
    }

    public class NuGetPackBuilder
    {
        public NuGetPackCommandTask Pack(string packagesToPack = "**/*.csproj", string? arguments = null)
        {
            return new NuGetPackCommandTask
            {
                PackagesToPack = packagesToPack,
                Arguments = arguments
            };
        }
    }

    public class NuGetCustomBuilder
    {
        public NuGetCustomCommandTask CustomCommand(string command, string? arguments = null)
        {
            return new NuGetCustomCommandTask
            {
                Command = command,
                Arguments = arguments
            };
        }
    }

    public abstract class NuGetCommandTask : AzureDevOpsTask
    {
        protected NuGetCommandTask(string taskName) : base(taskName)
        {
        }
    }

    public class NuGetRestoreCommandTask : NuGetCommandTask
    {
        public string? Feed { get; init; }
        public bool? IncludeNuGetOrg { get; init; }
        public string? NuGetConfigPath { get; init; }

        public NuGetRestoreCommandTask() : base("NuGetCommand@2")
        {
        }
    }

    public class NuGetPushCommandTask : NuGetCommandTask
    {
        public string? TargetFeed { get; init; }
        public string? TargetFeedCredentials { get; init; }

        public NuGetPushCommandTask() : base("NuGetCommand@2")
        {
        }
    }

    public class NuGetPackCommandTask : NuGetCommandTask
    {
        public string? PackagesToPack { get; init; }
        public string? Arguments { get; init; }

        public NuGetPackCommandTask() : base("NuGetCommand@2")
        {
        }
    }

    public class NuGetCustomCommandTask : NuGetCommandTask
    {
        public string? Command { get; init; }
        public string? Arguments { get; init; }

        public NuGetCustomCommandTask() : base("NuGetCommand@2")
        {
        }
    }
}
