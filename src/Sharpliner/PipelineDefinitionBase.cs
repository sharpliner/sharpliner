using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Sharpliner;

/// <summary>
/// Common ancestor for all serializable definitions.
/// Do not override this class, we have a Azure DevOps/GitHub specific child classes for that.
/// </summary>
public abstract class PipelineDefinitionBase
{
    /// <summary>
    /// Path to the YAML file where this pipeline will be exported to.
    /// When you build the project, the pipeline will be saved into a file on this path.
    /// Example: "/pipelines/ci.yaml"
    /// </summary>
    public abstract string TargetFile { get; }

    /// <summary>
    /// Override this to define where the resulting YAML should be stored (together with TargetFile).
    /// Default is RelativeToCurrentDir.
    /// </summary>
    public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

    // No inheritance outside of this project to not confuse people.
    internal PipelineDefinitionBase()
    {
    }

    public void Publish()
    {
        string fileName;
        switch (TargetPathType)
        {
            case TargetPathType.RelativeToGitRoot:
                var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (!Directory.Exists(Path.Combine(currentDir.FullName, ".git")))
                {
                    currentDir = currentDir.Parent;

                    if (currentDir == null)
                    {
                        throw new Exception($"Failed to find git repository in {Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName}");
                    }
                }

                fileName = Path.Combine(currentDir.FullName, TargetFile);
                break;

            case TargetPathType.RelativeToCurrentDir:
                fileName = TargetFile;
                break;

            case TargetPathType.RelativeToAssembly:
                fileName = Path.Combine(Assembly.GetExecutingAssembly().Location, TargetFile);
                break;

            case TargetPathType.Absolute:
                if (!Path.IsPathRooted(TargetFile))
                {
                    throw new Exception($"Failed to publish pipeline to {TargetFile}, path is not absolute.");
                }

                fileName = TargetFile;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(TargetPathType));
        }

        var fileContents = Serialize();

        var header = Header;
        if (header?.Length > 0)
        {
            const string hash = "### ";
            fileContents = hash + string.Join(Environment.NewLine + hash, header) + Environment.NewLine + Environment.NewLine + fileContents;
            fileContents = fileContents.Replace(" \r\n", "\r\n").Replace(" \n", "\n"); // Remove trailing spaces from the default template
        }

        File.WriteAllText(fileName, fileContents);
    }

    /// <summary>
    /// Gets the path where YAML of this definition should be published to.
    /// </summary>
    public string GetTargetPath()
    {
        switch (TargetPathType)
        {
            case TargetPathType.RelativeToGitRoot:
                var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (!Directory.Exists(Path.Combine(currentDir.FullName, ".git")))
                {
                    currentDir = currentDir.Parent;

                    if (currentDir == null)
                    {
                        throw new Exception($"Failed to find git repository in {Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName}");
                    }
                }

                return Path.Combine(currentDir.FullName, TargetFile);

            case TargetPathType.RelativeToCurrentDir:
                return TargetFile;

            case TargetPathType.RelativeToAssembly:
                return Path.Combine(Assembly.GetExecutingAssembly().Location, TargetFile);

            case TargetPathType.Absolute:
                return TargetFile;

            default:
                throw new ArgumentOutOfRangeException(nameof(TargetPathType));
        }
    }

    /// <summary>
    /// Validates the pipeline for runtime errors (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    public abstract void Validate();

    /// <summary>
    /// Serializes the pipeline into a YAML string.
    /// </summary>
    public abstract string Serialize();

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file.
    /// Leave null or empty to omit file header.
    /// </summary>
    protected virtual string[]? Header => new[]
    {
            string.Empty,
            "DO NOT MODIFY THIS FILE!",
            string.Empty,
            $"This YAML was auto-generated from { GetType().Name }.cs",
            $"To make changes, change the C# definition and rebuild its project",
            string.Empty,
        };

    protected static string PrettifyYaml(string yaml)
    {
        // Add empty new lines to make text more readable
        yaml = Regex.Replace(yaml, "((\r?\n)[a-zA-Z]+:)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?[a-zA-Z]+@?[a-zA-Z\\.0-9]*:)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?\\${{ ?if[^\n]+\n)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "(:\r?\n\r?\n)", ":" + Environment.NewLine);

        return yaml;
    }
}
