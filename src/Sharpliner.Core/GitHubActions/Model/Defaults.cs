namespace Sharpliner.GitHubActions;

// TODO (GitHub Actions): Made internal until we get to a more complete API
internal enum Shell
{
    Default,
    Bash,
    Cmd,
    Python,
    Pwsh,
    Powershell,
    Sh,
}

/// <summary>
/// Provide the default for the run step.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record RunDefaults
{
    /// <summary>
    /// Get/Set the shell to be used.
    /// </summary>
    public Shell Shell { get; set; } = Shell.Default;

    /// <summary>
    /// If shell is set to Custom, provide the shell to execute. For example:
    /// 'perl {0}'
    /// </summary>
    public string? CustomShell { get; set; } = null;

    /// <summary>
    /// The working directory of the step.
    /// </summary>
    public string? WorkingDirectory { get; set; } = null;
}

/// <summary>
/// Default settings that will be applied to all jobs.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Defaults
{
    /// <summary>
    /// Default values for the run step.
    /// </summary>
    public RunDefaults Run { get; init; } = new();
}
