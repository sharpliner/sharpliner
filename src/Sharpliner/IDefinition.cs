using System.IO;
using System.Reflection;
using System;

namespace Sharpliner;

public interface IDefinition
{
    /// <summary>
    /// Path to the YAML file/folder where this definition/collection will be exported to.
    /// Example: "/pipelines/ci.yaml"
    /// </summary>
    internal string TargetPath { get; }

    /// <summary>
    /// Override this to define where the resulting YAML should be stored (together with TargetFile).
    /// Default is RelativeToCurrentDir.
    /// </summary>
    TargetPathType TargetPathType { get; }

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// 
    /// Leave null or empty to omit file header.
    /// </summary>
    string[]? Header { get; }

    /// <summary>
    /// Gets the path where YAML of this definition should be published to.
    /// </summary>
    string GetTargetPath()
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

                return Path.Combine(currentDir.FullName, TargetPath);

            case TargetPathType.RelativeToCurrentDir:
                return TargetPath;

            case TargetPathType.RelativeToAssembly:
                return Path.Combine(Assembly.GetExecutingAssembly().Location, TargetPath);

            case TargetPathType.Absolute:
                return TargetPath;

            default:
                throw new ArgumentOutOfRangeException(nameof(TargetPathType));
        }
    }

    /// <summary>
    /// Publishes the definition into a YAML file.
    /// </summary>
    void Publish()
    {
        string fileName = GetTargetPath();
        var fileContents = Serialize();

        var header = Header;
        if (header?.Length > 0)
        {
            const string hash = "### ";
            fileContents = hash + string.Join(Environment.NewLine + hash, header) + Environment.NewLine + Environment.NewLine + fileContents;
            fileContents = fileContents.Replace(" \r\n", "\r\n").Replace(" \n", "\n"); // Remove trailing spaces from the default template
        }

        var targetDirectory = Path.GetDirectoryName(fileName)!;
        if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        File.WriteAllText(fileName, fileContents);
    }

    /// <summary>
    /// Validates the definition for runtime errors (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    void Validate();

    /// <summary>
    /// Serializes the definition into a YAML string.
    /// </summary>
    string Serialize();
}
