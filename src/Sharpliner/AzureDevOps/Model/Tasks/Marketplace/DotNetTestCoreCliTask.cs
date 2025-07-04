﻿using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the `dotnet test` command.
/// </summary>
public record DotNetTestCoreCliTask : DotNetCoreCliTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetTestCoreCliTask"/> class.
    /// </summary>
    public DotNetTestCoreCliTask() : base("test")
    {
    }

    /// <summary>
    /// Provides a name for the test run
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestRunTitle
    {
        get => GetExpression<string>("testRunTitle");
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
    public AdoExpression<bool>? PublishTestResults
    {
        get => GetExpression<bool>("publishTestResults");
        init => SetProperty("publishTestResults", value);
    }
}
