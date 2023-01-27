using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the `dotnet test` command.
/// </summary>
public record DotNetTestCoreCliTask : DotNetCoreCliTask
{
    public DotNetTestCoreCliTask() : base("test")
    {
    }

    /// <summary>
    /// Provides a name for the test run
    /// </summary>
    [YamlIgnore]
    public string? TestRunTitle
    {
        get => GetString("testRunTitle");
        init => SetProperty("testRunTitle", value);
    }

    /// <summary>
    /// Enabling this option will generate a test results TRX file in $(Agent.TempDirectory) and results will be published to the server.
    /// This option appends --logger trx --results-directory $(Agent.TempDirectory) to the command line arguments.
    /// Code coverage can be collected by adding --collect "Code coverage" to the command line arguments.
    ///
    /// This is currently only available on the Windows platform.
    /// </summary>
    [YamlIgnore]
    public bool PublishTestResults
    {
        get => GetBool("publishTestResults", false);
        init => SetProperty("publishTestResults", value ? "true" : "false");
    }
}
