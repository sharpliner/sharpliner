using System;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/npm-v1">Npm@1</see>
/// task in Azure DevOps pipelines.
/// </summary>
/// <remarks>
/// <para>
/// Audited against the <c>NpmV1</c> task specification from
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/NpmV1/task.json">microsoft/azure-pipelines-tasks</see>
/// and the current <see href="https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/npm-v1.md">Microsoft Learn YAML task reference</see>.
/// </para>
/// <para>
/// Use this task to run <c>npm ci</c>, <c>npm install</c>, <c>npm publish</c>, or a custom npm command.
/// </para>
/// </remarks>
public abstract record NpmTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmTask"/> class.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    protected NpmTask(NpmCommand command) : base("Npm@1")
    {
        Command = command;
    }

    /// <summary>
    /// Gets or sets the working folder that contains <c>package.json</c>.
    /// This corresponds to the task input <c>workingDir</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workingDir");
        init => SetProperty("workingDir", value);
    }

    [YamlIgnore]
    internal NpmCommand Command
    {
        get => GetEnum(nameof(Command), NpmCommand.Install);
        init => SetProperty("command", value);
    }
}

/// <summary>
/// Represents the <c>Npm@1</c> task with the <c>install</c> command.
/// </summary>
public record NpmInstallTask : NpmInstallCiTaskBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmInstallTask"/> class.
    /// </summary>
    public NpmInstallTask() : base(NpmCommand.Install)
    {
        CustomRegistry = NpmCustomRegistry.UseNpmrc;
    }
}

/// <summary>
/// Represents the <c>Npm@1</c> task with the <c>ci</c> command.
/// </summary>
public record NpmCiTask : NpmInstallCiTaskBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmCiTask"/> class.
    /// </summary>
    public NpmCiTask() : base(NpmCommand.Ci)
    {
        CustomRegistry = NpmCustomRegistry.UseNpmrc;
    }
}

/// <summary>
/// Represents the <c>Npm@1</c> task with the <c>custom</c> command.
/// </summary>
public record NpmCustomTask : NpmInstallCiCustomTaskBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmCustomTask"/> class.
    /// </summary>
    /// <param name="customCommand">Custom command to run, such as <c>dist-tag ls mypackage</c>.</param>
    public NpmCustomTask(AdoExpression<string> customCommand) : base(NpmCommand.Custom)
    {
        CustomRegistry = NpmCustomRegistry.UseNpmrc;
        CustomCommand = customCommand;
    }

    /// <summary>
    /// Gets or sets the custom npm command and arguments to run.
    /// Required when <c>command</c> is <c>custom</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomCommand
    {
        get => GetExpression<string>("customCommand");
        init => SetProperty("customCommand", value);
    }
}

/// <summary>
/// Represents the <c>Npm@1</c> task with the <c>publish</c> command.
/// </summary>
public record NpmPublishTask : NpmTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmPublishTask"/> class.
    /// </summary>
    public NpmPublishTask() : base(NpmCommand.Publish)
    {
        PublishRegistry = NpmPublishRegistry.UseExternalRegistry;
    }

    /// <summary>
    /// Gets or sets a value indicating whether verbose logging is enabled.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Verbose
    {
        get => GetExpression<bool>("verbose");
        init => SetProperty("verbose", value);
    }

    /// <summary>
    /// Gets or sets the publish target type.
    /// Defaults to <see cref="NpmPublishRegistry.UseExternalRegistry"/>.
    /// </summary>
    [YamlIgnore]
    public NpmPublishRegistry PublishRegistry
    {
        get => GetEnum(nameof(PublishRegistry), NpmPublishRegistry.UseExternalRegistry);
        init => SetProperty("publishRegistry", value);
    }

    /// <summary>
    /// Gets or sets the target Azure Artifacts/TFS feed.
    /// Required when <see cref="PublishRegistry"/> is <see cref="NpmPublishRegistry.UseFeed"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishFeed
    {
        get => GetExpression<string>("publishFeed");
        init => SetProperty("publishFeed", value);
    }

    /// <summary>
    /// Gets or sets whether pipeline metadata should be associated with published packages.
    /// Defaults to <c>true</c> in the task when omitted.
    /// Used when <see cref="PublishRegistry"/> is <see cref="NpmPublishRegistry.UseFeed"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PublishPackageMetadata
    {
        get => GetExpression<bool>("publishPackageMetadata");
        init => SetProperty("publishPackageMetadata", value);
    }

    /// <summary>
    /// Gets or sets the npm service connection used to publish to an external registry.
    /// Required when <see cref="PublishRegistry"/> is <see cref="NpmPublishRegistry.UseExternalRegistry"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishEndpoint
    {
        get => GetExpression<string>("publishEndpoint");
        init => SetProperty("publishEndpoint", value);
    }
}

/// <summary>
/// Base task for <c>Npm@1</c> commands that support custom registries and verbose logging.
/// </summary>
public abstract record NpmInstallCiTaskBase : NpmInstallCiCustomTaskBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmInstallCiTaskBase"/> class.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    protected NpmInstallCiTaskBase(NpmCommand command) : base(command)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether verbose logging is enabled.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Verbose
    {
        get => GetExpression<bool>("verbose");
        init => SetProperty("verbose", value);
    }
}

/// <summary>
/// Base task for <c>Npm@1</c> commands that support custom registries.
/// </summary>
public abstract record NpmInstallCiCustomTaskBase : NpmTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmInstallCiCustomTaskBase"/> class.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    protected NpmInstallCiCustomTaskBase(NpmCommand command) : base(command)
    {
    }

    /// <summary>
    /// Gets or sets the registry source mode.
    /// Defaults to <see cref="NpmCustomRegistry.UseNpmrc"/>.
    /// </summary>
    [YamlIgnore]
    public NpmCustomRegistry CustomRegistry
    {
        get => GetEnum(nameof(CustomRegistry), NpmCustomRegistry.UseNpmrc);
        init => SetProperty("customRegistry", value);
    }

    /// <summary>
    /// Gets or sets the Azure Artifacts/TFS feed used when <see cref="CustomRegistry"/> is <see cref="NpmCustomRegistry.UseFeed"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CustomFeed
    {
        get => GetExpression<string>("customFeed");
        init => SetProperty("customFeed", value);
    }

    /// <summary>
    /// Gets or sets npm service connection names for external registries listed in <c>.npmrc</c>.
    /// The task expects a comma-separated value in <c>customEndpoint</c>; this property exposes a strongly typed array.
    /// </summary>
    [YamlIgnore]
    public string[]? CustomEndpoint
    {
        get => GetString("customEndpoint")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        init => SetProperty("customEndpoint", ToCustomEndpointValue(value));
    }

    private static string? ToCustomEndpointValue(string[]? endpoints)
    {
        if (endpoints is null)
        {
            return null;
        }

        var normalizedEndpoints = endpoints
            .Where(endpoint => !string.IsNullOrWhiteSpace(endpoint))
            .Select(endpoint => endpoint.Trim())
            .ToArray();

        return normalizedEndpoints.Length == 0 ? null : string.Join(",", normalizedEndpoints);
    }
}

/// <summary>
/// Available command values for <c>Npm@1</c>.
/// </summary>
public enum NpmCommand
{
    /// <summary>
    /// <c>npm ci</c>.
    /// </summary>
    [YamlMember(Alias = "ci")]
    Ci,

    /// <summary>
    /// <c>npm install</c>.
    /// </summary>
    [YamlMember(Alias = "install")]
    Install,

    /// <summary>
    /// <c>npm publish</c>.
    /// </summary>
    [YamlMember(Alias = "publish")]
    Publish,

    /// <summary>
    /// Custom npm command from <c>customCommand</c>.
    /// </summary>
    [YamlMember(Alias = "custom")]
    Custom,
}

/// <summary>
/// Registry source options for <c>Npm@1</c> install/ci/custom commands.
/// </summary>
public enum NpmCustomRegistry
{
    /// <summary>
    /// Use registries from repository <c>.npmrc</c>.
    /// </summary>
    [YamlMember(Alias = "useNpmrc")]
    UseNpmrc,

    /// <summary>
    /// Use an explicitly selected Azure Artifacts/TFS feed.
    /// </summary>
    [YamlMember(Alias = "useFeed")]
    UseFeed,
}

/// <summary>
/// Publish target options for <c>Npm@1</c> publish command.
/// </summary>
public enum NpmPublishRegistry
{
    /// <summary>
    /// Publish to an external npm registry via <c>publishEndpoint</c>.
    /// </summary>
    [YamlMember(Alias = "useExternalRegistry")]
    UseExternalRegistry,

    /// <summary>
    /// Publish to an Azure Artifacts/TFS feed via <c>publishFeed</c>.
    /// </summary>
    [YamlMember(Alias = "useFeed")]
    UseFeed,
}
