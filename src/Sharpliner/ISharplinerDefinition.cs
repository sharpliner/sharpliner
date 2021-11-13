﻿using System.IO;
using System.Reflection;
using System;
using System.Text.RegularExpressions;

namespace Sharpliner;

/// <summary>
/// Every class that implements this interface will be serialized into a YAML file.
/// We recommend to inherit from some of the more concrete definitions such as PipelineDefinition.
/// </summary>
public interface ISharplinerDefinition
{
    /// <summary>
    /// Path to the YAML file/folder where this definition/collection will be exported to.
    /// Example: "/pipelines/ci.yaml"
    /// </summary>
    string TargetFile { get; }

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
    string[]? Header => new[]
    {
        string.Empty,
        "DO NOT MODIFY THIS FILE!",
        string.Empty,
        $"This YAML was auto-generated from { GetType().Name }",
        $"To make changes, change the C# definition and rebuild its project",
        string.Empty,
    };

    /// <summary>
    /// Gets the path where YAML of this definition should be published to.
    /// </summary>
    string GetTargetPath()
    {
        Serialize();
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
    /// Publishes the definition into a YAML file.
    /// </summary>
    void Publish()
    {
        string fileName = GetTargetPath();
        var fileContents = PrettifyYaml(Serialize());

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

    private string PrettifyYaml(string yaml)
    {
        // Add empty new lines to make text more readable
        yaml = Regex.Replace(yaml, "((\r?\n)[a-zA-Z]+:)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?[a-zA-Z]+@?[a-zA-Z\\.0-9]*:)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?\\${{ ?if[^\n]+\n)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "(:\r?\n\r?\n)", ":" + Environment.NewLine);

        return yaml;
    }
}
