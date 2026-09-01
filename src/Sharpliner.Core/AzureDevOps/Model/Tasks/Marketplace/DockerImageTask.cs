using System.Collections.Generic;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base for the Docker@2 commands that build and/or push an image (<c>build</c>, <c>push</c>, <c>buildAndPush</c>),
/// which share the <see cref="Repository"/> and <see cref="Tags"/> inputs.
/// </summary>
public abstract record DockerImageTask : DockerTask
{
    internal const string RepositoryProperty = "repository";
    internal const string TagsProperty = "tags";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerImageTask"/> class with the specified command.
    /// </summary>
    /// <param name="command">The Docker command</param>
    protected DockerImageTask(string command) : base(command)
    {
    }

    /// <summary>
    /// Name of the repository. Used to compose the fully qualified image name(s) together with <see cref="Tags"/>.
    /// Docker@2 input: <c>repository</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Repository
    {
        get => GetExpression<string>(RepositoryProperty);
        init => SetProperty(RepositoryProperty, value);
    }

    /// <summary>
    /// A list of tags to apply to the image(s).
    /// Docker@2 input: <c>tags</c>; a multi-line string in the raw YAML. Default: <c>$(Build.BuildId)</c>.
    /// </summary>
    [YamlIgnore]
    public IReadOnlyList<string>? Tags
    {
        get
        {
            var value = GetString(TagsProperty);
            return string.IsNullOrEmpty(value) ? null : value.Split('\n');
        }

        init => SetProperty(TagsProperty, value is null || value.Count == 0 ? null : string.Join("\n", value));
    }
}
