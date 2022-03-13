using System.IO;
using System.Reflection;
using System;
using Sharpliner.Common;
using System.Collections.Generic;

namespace Sharpliner;

/// <summary>
/// Every class that implements this interface will be serialized into a YAML file.
/// We recommend to inherit from some of the more concrete definitions such as PipelineDefinition.
/// </summary>
public interface ISharplinerDefinition
{
    /// <summary>
    /// Default YAML file header
    /// </summary>
    public static string[] GetDefaultHeader(Type type) => new[]
    {
        string.Empty,
        "DO NOT MODIFY THIS FILE!",
        string.Empty,
        $"This YAML was auto-generated from { type.Name }",
        $"To make changes, change the C# definition and rebuild its project",
        string.Empty,
    };

    /// <summary>
    /// Path to the YAML file/folder where this definition/collection will be exported to
    /// Example: "/pipelines/ci.yaml"
    /// </summary>
    string TargetFile { get; }

    /// <summary>
    /// Override this to define where the resulting YAML should be stored (together with TargetFile)
    /// Default is RelativeToGit
    /// </summary>
    TargetPathType TargetPathType { get; }

    /// <summary>
    /// Header that will be shown at the top of the generated YAML file
    /// Leave empty array to omit file header
    /// </summary>
    string[]? Header { get; }

    /// <summary>
    /// Gets the path where YAML of this definition should be published to
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
    /// Publishes the definition into a YAML file
    /// </summary>
    void Publish()
    {
        string fileName = GetTargetPath();
        var fileContents = Serialize();

        var header = Header ?? GetDefaultHeader(GetType());

        if (header.Length > 0)
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
    /// Returns the list of validations that should be run on the definition (e.g. wrong dependsOn, artifact name typos..).
    /// </summary>
    IReadOnlyCollection<IDefinitionValidation> Validations { get; }

    /// <summary>
    /// Serializes the definition into a YAML string
    /// </summary>
    string Serialize();
}
