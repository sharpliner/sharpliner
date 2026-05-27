using System;

namespace Sharpliner;

/// <summary>
/// Default values used by Sharpliner pipeline and workflow definitions.
/// </summary>
public static class SharplinerDefaults
{
    /// <summary>
    /// Default YAML file header used when a definition does not provide its own.
    /// </summary>
    /// <param name="type">Type of the definition the header is being generated for.</param>
    /// <returns>The lines that make up the default header (without comment markers).</returns>
    public static string[] GetDefaultHeader(Type type) =>
    [
        string.Empty,
        "DO NOT MODIFY THIS FILE!",
        string.Empty,
        $"This YAML was auto-generated from { type.Name }",
        $"To make changes, change the C# definition and rebuild its project",
        string.Empty,
    ];
}
