using System;
using System.Text.RegularExpressions;

namespace Sharpliner;

/// <summary>
/// Common ancestor for all serializable definitions.
/// Do not override this class, we have a Azure DevOps/GitHub specific child classes for that.
/// </summary>
public abstract class DefinitionBase : IDefinition
{
    internal static string[] DefaultHeader(string name) => new[]
    {
        string.Empty,
        "DO NOT MODIFY THIS FILE!",
        string.Empty,
        $"This YAML was auto-generated from { name }",
        $"To make changes, change the C# definition and rebuild its project",
        string.Empty,
    };

    /// <summary>
    /// Path to the YAML file where this pipeline will be exported to.
    /// When you build the project, the pipeline will be saved into a file on this path.
    /// Example: "/pipelines/ci.yaml"
    /// </summary>
    public abstract string TargetFile { get; }

    public virtual string[]? Header => DefaultHeader(GetType().Name);

    string IDefinition.TargetPath => TargetFile;

    public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

    // No inheritance outside of this project to not confuse people.
    internal DefinitionBase()
    {
    }

    public abstract void Validate();

    public abstract string Serialize();

    protected virtual string PrettifyYaml(string yaml)
    {
        // Add empty new lines to make text more readable
        yaml = Regex.Replace(yaml, "((\r?\n)[a-zA-Z]+:)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?[a-zA-Z]+@?[a-zA-Z\\.0-9]*:)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "((\r?\n) {0,8}- ?\\${{ ?if[^\n]+\n)", Environment.NewLine + "$1");
        yaml = Regex.Replace(yaml, "(:\r?\n\r?\n)", ":" + Environment.NewLine);

        return yaml;
    }

    void IDefinition.Validate() => throw new NotImplementedException();
    string IDefinition.Serialize() => throw new NotImplementedException();
}
