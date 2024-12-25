using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the `dotnet publish` command.
/// </summary>
public record DotNetPublishCoreCliTask : DotNetCoreCliTask
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetPublishCoreCliTask"/> class.
    /// </summary>
    public DotNetPublishCoreCliTask() : base("publish")
    {
    }

    /// <summary>
    /// If true, the projects property value will be skipped and the task will try to find the web projects in the repository and
    /// run the publish command on them. Web projects are identified by presence of either a web.config file or wwwroot folder in the directory.
    /// In the absence of a web.config file or wwwroot folder, projects that use a web SDK, like Microsoft.NET.Sdk.Web, are selected.
    ///
    /// Note that this argument defaults to true if not specified.
    /// </summary>
    [YamlIgnore]
    public bool PublishWebProjects
    {
        get => GetBool("publishWebProjects", true);
        init => SetProperty("publishWebProjects", value);
    }

    /// <summary>
    /// If true, folder created by the publish command will be zipped and deleted.
    /// </summary>
    [YamlIgnore]
    public bool ZipAfterPublish
    {
        get => GetBool("zipAfterPublish", false);
        init => SetProperty("zipAfterPublish", value);
    }

    /// <summary>
    /// If true, folders created by the publish command will have project file name prefixed to their folder names when output path is
    /// specified explicitly in arguments.
    /// This is useful if you want to publish multiple projects to the same folder.
    /// </summary>
    [YamlIgnore]
    public bool ModifyOutputPath
    {
        get => GetBool("modifyOutputPath", false);
        init => SetProperty("modifyOutputPath", value);
    }
}
