namespace Sharpliner.AzureDevOps;

public sealed class VariableReference
{
    internal static string GetReference(string prefix, string variableName) => $"variables['{prefix}{variableName}']";

    public string this[string variableName] => $"variables['{variableName}']";

    /// <summary>
    /// Variables connected to the agent running the current build (e.g. Agent.HomeDirectory)
    /// Read more at https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#agent-variables-devops-services
    /// </summary>
    public AgentVariableReference Agent { get; } = new();

    /// <summary>
    /// Variables connected to the current build (e.g. build number)
    /// Read more at https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#build-variables-devops-services
    /// </summary>
    public BuildVariableReference Build { get; } = new();
}


public sealed class AgentVariableReference
{
    public string this[string variableName] => GetReference(variableName);

    /// <summary>
    /// The local path on the agent where all folders for a given build pipeline are created.
    /// This variable has the same value as Pipeline.Workspace.
    /// For example: /home/vsts/work/1
    /// </summary>
    public string BuildDirectory => GetReference("BuildDirectory");

    /// <summary>
    /// A mapping from container resource names in YAML to their Docker IDs at runtime.
    /// For example:
    /// {
    ///   "one_container": {
    ///     "id": "bdbb357d73a0bd3550a1a5b778b62a4c88ed2051c7802a0659f1ff6e76910190"
    ///   },
    ///   "another_container": {
    ///     "id": "82652975109ec494876a8ccbb875459c945982952e0a72ad74c91216707162bb"
    ///   }
    /// }
    /// </summary>
    public string ContainerMapping => GetReference("ContainerMapping");

    /// <summary>
    /// The directory the agent is installed into. This contains the agent software.
    /// For example: c:\agent.
    /// </summary>
    public string HomeDirectory => GetReference("HomeDirectory");

    /// <summary>
    /// The ID of the agent.
    /// </summary>
    public string Id => GetReference("Id");

    /// <summary>
    /// The name of the running job.
    /// This will usually be "Job" or "__default", but in multi-config scenarios, will be the configuration.
    /// </summary>
    public string JobName => GetReference("JobName");

    /// <summary>
    /// The status of the build.
    ///   - Canceled
    ///   - Failed
    ///   - Succeeded
    ///   - SucceededWithIssues (partially successful) 
    /// </summary>
    public string JobStatus => GetReference("JobStatus");

    /// <summary>
    /// The name of the machine on which the agent is installed.
    /// </summary>
    public string MachineName => GetReference("MachineName");

    /// <summary>
    /// The name of the agent that is registered with the pool.
    /// If you are using a self-hosted agent, then this name is specified by you. See agents.
    /// </summary>
    public string Name => GetReference("Name");

    /// <summary>
    /// The operating system of the agent host.
    /// Valid values are:
    ///   - Windows_NT
    ///   - Darwin
    ///   - Linux
    /// </summary>
    public string OS => GetReference("OS");

    /// <summary>
    /// The operating system processor architecture of the agent host.
    /// Valid values are:
    ///   - X86
    ///   - X64
    ///   - ARM
    /// </summary>
    public string OSArchitecture => GetReference("OSArchitecture");

    /// <summary>
    /// A temporary folder that is cleaned after each pipeline job.
    /// This directory is used by tasks such as .NET Core CLI task to hold temporary items like test results before they are published.
    /// For example: /home/vsts/work/_temp for Ubuntu
    /// </summary>
    public string TempDirectory => GetReference("TempDirectory");

    /// <summary>
    /// The directory used by tasks such as Node Tool Installer and Use Python Version to switch between multiple versions of a tool.
    /// These tasks will add tools from this directory to PATH so that subsequent build steps can use them.
    /// Learn about managing this directory on a self-hosted agent.
    /// </summary>
    public string ToolsDirectory => GetReference("ToolsDirectory");

    /// <summary>
    /// The working directory for this agent.
    /// For example: c:\agent_work
    /// Note: This directory is not guaranteed to be writable by pipeline tasks (eg. when mapped into a container)
    /// </summary>
    public string WorkFolder => GetReference("WorkFolder");

    private static string GetReference(string name) => VariableReference.GetReference("Agent.", name);
}


public sealed class BuildVariableReference
{
    public string this[string variableName] => GetReference(variableName);



    private static string GetReference(string name) => VariableReference.GetReference("Build.", name);
}
