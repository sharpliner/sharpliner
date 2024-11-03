using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Class that contains references to various Azure DevOps predefined variables.
/// </summary>
public class VariablesReference
{
    /// <summary>
    /// Gets a reference to a variable with the specified name.
    /// This should be used for custom variables.
    /// </summary>
    /// <param name="variableName">The variable name.</param>
    /// <returns>A variable reference to the specified name.</returns>
    public VariableReference this[string variableName] => new(variableName);

    /// <summary>
    /// Variables connected to the agent running the current build (e.g. <c>Agent.HomeDirectory</c>)
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&amp;tabs=yaml#agent-variables-devops-services">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    /// <remarks>
    /// You can use agent variables as environment variables in your scripts and as parameters in your build tasks. You cannot use them to customize the build number or to apply a version control label or tag.
    /// </remarks>
    public AgentVariableReference Agent { get; } = new();

    /// <summary>
    /// Variables connected to the current build (e.g. <c>Build.BuildNumber</c>)
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&amp;tabs=yaml#build-variables-devops-services">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    /// <remarks>
    /// When you use a variable in a template that is not marked as available in templates, the variable will not render. The variable won't render because its value is not accessible within the template's scope.
    /// </remarks>
    public BuildVariableReference Build { get; } = new();

    /// <summary>
    /// System variables (e.g. <c>System.AccessToken</c> or <c>System.Debug</c>)
    /// </summary>
    public SystemVariableReference System { get; } = new();

    /// <summary>
    /// Variables connected to the pipeline (not build)
    /// </summary>
    public PipelineVariableReference Pipeline { get; } = new();

    /// <summary>
    /// Deployment variables
    /// These variables are scoped to a specific <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/deployment-jobs?view=azure-devops">Deployment job</see> and will be resolved only at job execution time
    /// </summary>
    public EnvironmentVariableReference Environment { get; } = new();

    /// <summary>
    /// Deployment variables
    /// These variables are scoped to a specific <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/deployment-jobs?view=azure-devops">Deployment job</see> and will be resolved only at job execution time
    /// </summary>
    public StrategyVariableReference Strategy { get; } = new();

    /// <summary>
    /// Set to <c>True</c> if the script is being run by a build task.
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public VariableReference TF_BUILD => new("TF_BUILD");
}

/// <summary>
/// Base class for all variable references.
/// </summary>
public abstract class VariableReferenceBase
{
    internal static VariableReference GetReference(string prefix, string variableName) => new($"{prefix}{variableName}");

    /// <summary>
    /// Gets a reference to a variable with the specified name.
    /// </summary>
    /// <param name="variableName">The variable name.</param>
    /// <returns>A variable reference to the specified name.</returns>
    public VariableReference this[string variableName] => GetReference(Prefix, variableName);

    /// <summary>
    /// Gets a reference to a variable with the specified name.
    /// </summary>
    /// <param name="name">The variable name</param>
    /// <returns>A variable reference to the specified name.</returns>
    protected VariableReference GetReference(string name) => GetReference(Prefix, name);

    /// <summary>
    /// The prefix of the variable.
    /// </summary>
    protected abstract string Prefix { get; }
}

/// <summary>
/// Represents the variables that have the prefix <c>Agent.</c>.
/// </summary>
public sealed class AgentVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Agent.";

    internal AgentVariableReference()
    {
    }

    /// <summary>
    /// The local path on the agent where all folders for a given build pipeline are created.
    /// This variable has the same value as <c>Pipeline.Workspace</c>.
    /// <para>
    /// For example: <c>/home/vsts/work/1</c>
    /// </para>
    /// </summary>
    public VariableReference BuildDirectory => GetReference("BuildDirectory");

    /// <summary>
    /// A mapping from container resource names in YAML to their Docker IDs at runtime.
    /// For example:
    /// <code>
    /// {
    ///   "one_container": {
    ///     "id": "bdbb357d73a0bd3550a1a5b778b62a4c88ed2051c7802a0659f1ff6e76910190"
    ///   },
    ///   "another_container": {
    ///     "id": "82652975109ec494876a8ccbb875459c945982952e0a72ad74c91216707162bb"
    ///   }
    /// }
    /// </code>
    /// </summary>
    public VariableReference ContainerMapping => GetReference("ContainerMapping");

    /// <summary>
    /// The directory the agent is installed into. This contains the agent software.
    /// For example: c:\agent.
    /// </summary>
    public VariableReference HomeDirectory => GetReference("HomeDirectory");

    /// <summary>
    /// The ID of the agent.
    /// </summary>
    public VariableReference Id => GetReference("Id");

    /// <summary>
    /// The name of the running job.
    /// This will usually be "Job" or "__default", but in multi-config scenarios, will be the configuration.
    /// </summary>
    public VariableReference JobName => GetReference("JobName");

    /// <summary>
    /// The status of the build.
    /// <list type="bullet">
    /// <item>Canceled</item>
    /// <item>Failed</item>
    /// <item>Succeeded</item>
    /// <item>SucceededWithIssues (partially successful)</item>
    /// <item>Skipped(last job)</item>
    /// </list>
    /// </summary>
    public VariableReference JobStatus => GetReference("JobStatus");

    /// <summary>
    /// The name of the machine on which the agent is installed.
    /// </summary>
    public VariableReference MachineName => GetReference("MachineName");

    /// <summary>
    /// The name of the agent that is registered with the pool.
    /// If you are using a self-hosted agent, then this name is specified by you. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/agents/agents">agents</see>.
    /// </summary>
    public VariableReference Name => GetReference("Name");

    /// <summary>
    /// The operating system of the agent host.
    /// Valid values are:
    /// <list type="bullet">
    /// <item>Windows_NT</item>
    /// <item>Linux</item>
    /// <item>Darwin</item>
    /// </list>
    /// </summary>
    public VariableReference OS => GetReference("OS");

    /// <summary>
    /// The operating system processor architecture of the agent host.
    /// Valid values are:
    /// <list type="bullet">
    /// <item>X86</item>
    /// <item>X64</item>
    /// <item>ARM</item>
    /// </list>
    /// </summary>
    public VariableReference OSArchitecture => GetReference("OSArchitecture");

    /// <summary>
    /// A temporary folder that is cleaned after each pipeline job.
    /// This directory is used by tasks such as <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/dotnet-core-cli-v2">.NET Core CLI task</see> to hold temporary items like test results before they are published.
    /// For example: <c>/home/vsts/work/_temp</c> for Ubuntu
    /// </summary>
    public VariableReference TempDirectory => GetReference("TempDirectory");

    /// <summary>
    /// The directory used by tasks such as <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/node-tool-v0">Node Tool Installer</see> and <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/use-python-version-v0">Use Python Version</see> to switch between multiple versions of a tool.
    /// <para>
    /// These tasks will add tools from this directory to PATH so that subsequent build steps can use them.
    /// </para>
    /// <para>
    /// Learn about <see href="https://go.microsoft.com/fwlink/?linkid=2008884">managing this directory on a self-hosted agent</see>.
    /// </para>
    /// </summary>
    public VariableReference ToolsDirectory => GetReference("ToolsDirectory");

    /// <summary>
    /// The working directory for this agent.
    /// For example: <c>c:\agent_work</c>
    /// </summary>
    /// <remarks>
    /// Note: This directory is not guaranteed to be writable by pipeline tasks (eg. when mapped into a container)
    /// </remarks>
    public VariableReference WorkFolder => GetReference("WorkFolder");
}

/// <summary>
/// Represents the variables that have the prefix <c>Build.</c>.
/// </summary>
public sealed class BuildVariableReference : VariableReferenceBase
{
    internal static readonly BuildVariableReference Instance = new();

    /// <inheritdoc/>
    protected override string Prefix => "Build.";

    internal BuildVariableReference()
    {
    }

    /// <summary>
    /// <para>
    /// The local path on the agent where any artifacts are copied to before being pushed to their destination.
    /// For example: <c>c:\agent_work\1\a</c>
    /// </para>
    /// <para>
    /// A typical way to use this folder is to publish your build artifacts with the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/copy-files-v2">Copy files</see> and <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-build-artifacts-v1">Publish build artifacts</see> tasks.
    /// </para>
    /// <para>
    /// Note: <c>Build.ArtifactStagingDirectory</c> and <c>Build.StagingDirectory</c> are interchangeable. This directory is purged before each new build, so you don't have to clean it up yourself.
    /// </para>
    /// <para>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/artifacts/artifacts-overview?view=azure-devops">Artifacts in Azure Pipelines</see>.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference ArtifactStagingDirectory => GetReference("ArtifactStagingDirectory");

    /// <summary>
    /// <para>
    /// The local path on the agent where any artifacts are copied to before being pushed to their destination.
    /// For example: <c>c:\agent_work\1\a</c>
    /// </para>
    /// <para>
    /// A typical way to use this folder is to publish your build artifacts with the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/copy-files-v2">Copy files</see> and <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-build-artifacts-v1">Publish build artifacts</see> tasks.
    /// </para>
    /// <para>
    /// Note: <c>Build.ArtifactStagingDirectory</c> and <c>Build.StagingDirectory</c> are interchangeable. This directory is purged before each new build, so you don't have to clean it up yourself.
    /// </para>
    /// <para>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/artifacts/artifacts-overview?view=azure-devops">Artifacts in Azure Pipelines</see>.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference StagingDirectory => GetReference("StagingDirectory");

    /// <summary>
    /// The ID of the record for the completed build.
    /// </summary>
    public VariableReference BuildId => GetReference("BuildId");

    /// <summary>
    /// <para>
    /// The name of the completed build, also known as the run number.You can specify <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/run-number?view=azure-devops">what is included</see> in this value.
    /// </para>
    /// <para>
    /// A typical use of this variable is to make it part of the label format, which you specify on the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/?view=azure-devops">repository tab</see>.
    /// </para>
    /// <para>
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the <see href="https://learn.microsoft.com/en-us/azure/devops/repos/tfvc/labels-command?view=azure-devops">label format</see> will fail.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference BuildNumber => GetReference("BuildNumber");

    /// <summary>
    /// <para>
    /// The URI for the build. For example: <c>vstfs:///Build/Build/1430</c>
    /// </para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public VariableReference BuildUri => GetReference("BuildUri");

    /// <summary>
    /// <para>
    /// The local path on the agent you can use as an output folder for compiled binaries.
    /// </para>
    /// <para>
    /// By default, new build pipelines are not set up to clean this directory. You can define your build to clean it up on the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/?view=azure-devops">Repository tab</see>.
    /// For example: <c>c:\agent_work\1\b</c>
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference BinariesDirectory => GetReference("BinariesDirectory");

    /// <summary>
    /// The ID of the container for your artifact.
    /// When you upload an artifact in your pipeline, it is added to a container that is specific for that particular artifact.
    /// </summary>
    public VariableReference ContainerId => GetReference("ContainerId");

    /// <summary>
    /// The name of the build pipeline.
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the <see href="https://learn.microsoft.com/en-us/azure/devops/repos/tfvc/labels-command?view=azure-devops">label format</see> will fail.
    /// </summary>
    public VariableReference DefinitionName => GetReference("DefinitionName");

    /// <summary>
    /// The version of the build pipeline.
    /// </summary>
    public VariableReference DefinitionVersion => GetReference("DefinitionVersion");

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/build/variables#identity_values">How are the identity variables set?</see>.
    /// </summary>
    /// <remarks>
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the <see href="https://learn.microsoft.com/en-us/azure/devops/repos/tfvc/labels-command?view=azure-devops">label format</see> will fail.
    /// </remarks>
    public VariableReference QueuedBy => GetReference("QueuedBy");

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/build/variables#identity_values">How are the identity variables set?</see>
    /// </summary>
    public VariableReference QueuedById => GetReference("QueuedById");

    /// <summary>
    /// The event that caused the build to run.
    /// <list type="bullet">
    /// <item>Manual: A user manually queued the build.</item>
    /// <item>IndividualCI: Continuous integration (CI) triggered by a Git push or a TFVC check-in.</item>
    /// <item>BatchedCI: Continuous integration (CI) triggered by a Git push or a TFVC check-in, and the Batch changes was selected.</item>
    /// <item>Schedule: Scheduled trigger.</item>
    /// <item>ValidateShelveset: A user manually queued the build of a specific TFVC shelveset.</item>
    /// <item>CheckInShelveset: Gated check-in trigger.</item>
    /// <item>PullRequest: The build was triggered by a Git branch policy that requires a build.</item>
    /// <item>ResourceTrigger: The build was triggered by a resource trigger or it was triggered by another build.</item>
    /// </list>
    /// </summary>
    public VariableReference Reason => GetReference("Reason");

    /// <summary>
    /// Variables connected to repository information
    /// </summary>
    public RepositoryVariableReference Repository => new();

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/build/variables#identity_values">How are the identity variables set?</see>
    /// </summary>
    /// <remarks>
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the <see href="https://learn.microsoft.com/en-us/azure/devops/repos/tfvc/labels-command?view=azure-devops">label format</see> will fail.
    /// </remarks>
    public VariableReference RequestedFor => GetReference("RequestedFor");

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/build/variables#identity_values">How are the identity variables set?</see>
    /// </summary>
    public VariableReference RequestedForEmail => GetReference("RequestedForEmail");

    /// <summary>
    /// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/build/variables#identity_values">How are the identity variables set?</see>
    /// </summary>
    public VariableReference RequestedForId => GetReference("RequestedForId");

    /// <summary>
    /// The branch of the triggering repo the build was queued for.
    /// Some examples:
    /// <list type="bullet">
    /// <item>Git repo branch: <c>refs/heads/master</c></item>
    /// <item>Git repo pull request: <c>refs/pull/1/merge</c></item>
    /// <item>TFVC repo branch: <c>$/teamproject/main</c></item>
    /// <item>TFVC repo gated check-in: <c>Gated_2016-06-06_05.20.51.4369;username @live.com</c></item>
    /// <item>TFVC repo shelveset build: <c>myshelveset; username @live.com</c></item>
    /// <item>When your pipeline is triggered by a tag: <c>refs/tags/your-tag-name</c></item>
    /// </list>
    /// When you use this variable in your build number format, the forward slash characters (/) are replaced with underscore characters _).
    /// </summary>
    /// <remarks>
    /// Note: In TFVC, if you are running a gated check-in build or manually building a shelveset, you cannot use this variable in your build number format.
    /// </remarks>
    public VariableReference SourceBranch => GetReference("SourceBranch");

    /// <summary>
    /// The name of the branch in the triggering repo the build was queued for.
    /// <list type="bullet">
    /// <item>Git repo branch or pull request: The last path segment in the ref. For example: in refs/heads/master this value is master.In <c>refs/heads/feature/tools</c> this value is <c>tools</c>.</item>
    /// <item>TFVC repo branch: The last path segment in the root server path for the workspace. For example: in <c>$/teamproject/main</c> this value is <c>main</c>.</item>
    /// <item>TFVC repo gated check-in or shelveset build is the name of the shelveset.For example: <c>Gated_2016-06-06_05.20.51.4369; username @live.com or myshelveset; username @live.com</c>.</item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Note: In TFVC, if you are running a gated check-in build or manually building a shelveset, you cannot use this variable in your build number format.
    /// </remarks>
    public VariableReference SourceBranchName => GetReference("SourceBranchName");

    /// <summary>
    /// <para>
    /// The local path on the agent where your source code files are downloaded. For example: <c>c:\agent_work\1\s</c>
    /// By default, new build pipelines update only the changed files.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Important note: If you check out only one Git repository, this path will be the exact path to the code.
    /// </para>
    /// If you check out multiple repositories, it will revert to its default value, which is <c>$(Pipeline.Workspace)/s</c>,
    /// even if the self (primary) repository is checked out to a custom path different from its multi-checkout default path <c>$(Pipeline.Workspace)/s/[RepoName]</c> (in this respect, the variable differs from the behavior of the <c>Build.Repository.LocalPath</c> variable).
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </remarks>
    public VariableReference SourcesDirectory => GetReference("SourcesDirectory");

    /// <summary>
    /// The latest version control change of the triggering repo that is included in this build.
    /// <list type="bullet">
    /// <item>Git: The commit ID</item>
    /// <item>TFVC: the changeset</item>
    /// </list>
    /// </summary>
    public VariableReference SourceVersion => GetReference("SourceVersion");

    /// <summary>
    /// <para>
    /// The comment of the commit or changeset for the triggering repo. We truncate the message to the first line or 200 characters, whichever is shorter.
    /// </para>
    /// <para>
    /// The <c>Build.SourceVersionMessage</c> corresponds to the message on <c>Build.SourceVersion</c> commit.
    /// The <c>Build.SourceVersion</c> commit for a PR build is the merge commit (not the commit on the source branch).
    /// </para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// Also, this variable is only available on the step level and is neither available in the job nor stage levels (i.e. the message is not extracted until the job had started and checked out the code).
    /// </summary>
    /// <remarks>
    /// Note: This variable is available in TFS 2015.4.
    /// </remarks>
    public VariableReference SourceVersionMessage => GetReference("SourceVersionMessage");

    /// <summary>
    /// Defined if your repository is Team Foundation Version Control.
    /// If you are running a gated build or a shelveset build, this is set to the name of the shelveset you are building.
    /// </summary>
    /// <remarks>
    /// Note: This variable yields a value that is invalid for build use in a build number format.
    /// </remarks>
    public VariableReference SourceTfvcShelveset => GetReference("SourceTfvcShelveset");

    /// <summary>
    /// <para>
    /// The local path on the agent where the test results are created.
    /// For example: <c>c:\agent_work\1\TestResults</c>
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference CommonTestResultsDirectory => GetReference("Common.TestResultsDirectory");

    /// <summary>
    /// Variables connected to why the build was created.
    /// </summary>
    public TriggeredByVariableReference TriggeredBy => new();
}

/// <summary>
/// Represents the variables that have the prefix <c>Build.Repository.</c>.
/// </summary>
public sealed class RepositoryVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Build.Repository.";

    internal RepositoryVariableReference()
    {
    }

    /// <summary>
    /// <para>
    /// The value you've selected for Clean in the source repository settings.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference Clean => GetReference("Clean");

    /// <summary>
    /// <para>
    /// The local path on the agent where your source code files are downloaded.
    /// For example: c:\agent_work\1\s
    /// </para>
    /// <para>
    /// By default, new build pipelines update only the changed files.You can modify how files are downloaded on the Repository tab.
    /// </para>
    /// Important note: If you check out only one Git repository, this path will be the exact path to the code.If you check out multiple repositories, the behavior is as follows(and might differ from the value of the <c>Build.SourcesDirectory</c> variable) :
    /// <list type="bullet">
    /// <item>If the checkout step for the self(primary) repository has no custom checkout path defined, or the checkout path is the multi-checkout default path <c>$(Pipeline.Workspace)/s/[RepoName]</c> for the self repository, the value of this variable will revert to its default value, which is <c>$(Pipeline.Workspace)/s</c>.</item>
    /// <item>If the checkout step for the self (primary) repository does have a custom checkout path defined (and it's not its multi-checkout default path), this variable will contain the exact path to the self repository.</item>
    /// </list>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference LocalPath => GetReference("LocalPath");

    /// <summary>
    /// <para>
    /// The unique identifier of the repository.
    /// This won't change, even if the name of the repository does.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference ID => GetReference("ID");

    /// <summary>
    /// <para>
    /// The name of the triggering repository.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference Name => GetReference("Name");

    /// <summary>
    /// The name of the branch in the triggering repository.
    /// <list type="bullet">
    /// <item>TfsGit: TFS Git repository</item>
    /// <item>TfsVersionControl: Team Foundation Version Control</item>
    /// <item>Git: Git repository hosted on an external server</item>
    /// <item>GitHub</item>
    /// <item>Svn: Subversion</item>
    /// </list>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference Provider => GetReference("Provider");

    /// <summary>
    /// <para>
    /// Defined if your repository is Team Foundation Version Control. The name of the TFVC workspace used by the build agent.
    /// </para>
    /// <para>
    /// For example: if the Agent.BuildDirectory is c:\agent_work\12 and the Agent.Id is 8, the workspace name could be: ws_12_8
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference TfvcWorkspace => GetReference("Tfvc.Workspace");

    /// <summary>
    /// The URL for the triggering repository. For example:
    /// <list type="bullet">
    /// <item>Git: https://dev.azure.com/fabrikamfiber/_git/Scripts</item>
    /// <item>TFVC: https://dev.azure.com/fabrikamfiber/</item>
    /// </list>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public VariableReference Uri => GetReference("Uri");

    /// <summary>
    /// <para>
    /// The value you've selected for Checkout submodules on the repository tab. With multiple repos checked out, this value tracks the triggering repository's setting.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference GitSubmoduleCheckout => GetReference("Git.SubmoduleCheckout");
}

/// <summary>
/// Represents the variables that have the prefix <c>Build.TriggeredBy.</c>.
/// </summary>
public sealed class TriggeredByVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Build.TriggeredBy.";

    internal TriggeredByVariableReference()
    {
    }

    /// <summary>
    /// <para>
    /// If the build was <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/pipeline-triggers?view=azure-devops">triggered by another build</see>, then this variable is set to the BuildID of the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// <para>
    /// If you are triggering a YAML pipeline using <c>resources</c>, you should use the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">resources variables</see> instead.
    /// </para>
    /// </summary>
    public VariableReference BuildId => GetReference("BuildId");

    /// <summary>
    /// <para>
    /// If the build was <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/pipeline-triggers?view=azure-devops">triggered by another build</see>, then this variable is set to the DefinitionID of the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// <para>
    /// If you are triggering a YAML pipeline using <c>resources</c>, you should use the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">resources variables</see> instead.
    /// </para>
    /// </summary>
    public VariableReference DefinitionId => GetReference("DefinitionId");

    /// <summary>
    /// <para>
    /// If the build was <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/pipeline-triggers?view=azure-devops">triggered by another build</see>, then this variable is set to the name of the triggering build pipeline. In Classic pipelines, this variable is triggered by a build completion trigger.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// <para>
    /// If you are triggering a YAML pipeline using <c>resources</c>, you should use the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">resources variables</see> instead.
    /// </para>
    /// </summary>
    public VariableReference DefinitionName => GetReference("DefinitionName");

    /// <summary>
    /// <para>
    /// If the build was <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/pipeline-triggers?view=azure-devops">triggered by another build</see>, then this variable is set to the number of the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// <para>
    /// If you are triggering a YAML pipeline using <c>resources</c>, you should use the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">resources variables</see> instead.
    /// </para>
    /// </summary>
    public VariableReference BuildNumber => GetReference("BuildNumber");

    /// <summary>
    /// <para>
    /// If the build was <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/pipeline-triggers?view=azure-devops">triggered by another build</see>, then this variable is set to ID of the project that contains the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    /// </para>
    /// <para>
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// <para>
    /// If you are triggering a YAML pipeline using <c>resources</c>, you should use the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">resources variables</see> instead.
    /// </para>
    /// </summary>
    public VariableReference ProjectID => GetReference("ProjectID");
}

/// <summary>
/// Represents the variables that have the prefix <c>System.</c>.
/// </summary>
public sealed class SystemVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "System.";

    internal SystemVariableReference()
    {
    }

    /// <summary>
    /// Variables connected to a PR that initiated this build.
    /// </summary>
    public PullRequestVariableReference PullRequest { get; } = new();

    /// <summary>
    /// System.AccessToken is a special variable that carries the security token used by the running build.
    /// Use the OAuth token to access the REST API.
    /// Use System.AccessToken from YAML scripts.
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public VariableReference AccessToken => GetReference("AccessToken");

    /// <summary>
    /// Variable that can be set for a pipeline run manually (either true or false).
    /// </summary>
    public VariableReference Debug => GetReference("Debug");

    /// <summary>
    /// The GUID of the TFS collection or Azure DevOps organization.
    /// </summary>
    public VariableReference CollectionId => GetReference("CollectionId");

    /// <summary>
    /// The URI of the TFS collection or Azure DevOps organization.
    /// For example: <c>https://dev.azure.com/fabrikamfiber/</c>.
    /// </summary>
    public VariableReference CollectionUri => GetReference("CollectionUri");

    /// <summary>
    /// <para>
    /// The local path on the agent where your source code files are downloaded.
    /// For example: <c>c:\agent_work\1\s</c>
    /// </para>
    /// <para>
    /// By default, new build pipelines update only the changed files.
    /// You can modify how files are downloaded on the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/?view=azure-devops">Repository tab</see>.
    /// </para>
    /// <para>
    /// This variable is agent-scoped. It can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </para>
    /// </summary>
    public VariableReference DefaultWorkingDirectory => GetReference("DefaultWorkingDirectory");

    /// <summary>
    /// The ID of the build pipeline.
    /// </summary>
    public VariableReference DefinitionId => GetReference("DefinitionId");

    /// <summary>
    /// Set to <c>build</c> if the pipeline is a build. For a release, the values are <c>deployment</c> for a Deployment group job, gates during evaluation of gates, and <c>release</c> for other (Agent and Agentless) jobs.
    /// </summary>
    public VariableReference HostType => GetReference("HostType");

    /// <summary>
    /// Set to 1 the first time this job is attempted, and increments every time the job is retried.
    /// </summary>
    public VariableReference JobAttempt => GetReference("JobAttempt");

    /// <summary>
    /// The human-readable name given to a job.
    /// </summary>
    public VariableReference JobDisplayName => GetReference("JobDisplayName");

    /// <summary>
    /// A unique identifier for a single attempt of a single job. The value is unique to the current pipeline.
    /// </summary>
    public VariableReference JobId => GetReference("JobId");

    /// <summary>
    /// The name of the job, typically used for expressing dependencies and accessing output variables.
    /// </summary>
    public VariableReference JobName => GetReference("JobName");

    /// <summary>
    /// <para>
    /// Set to 1 the first time this phase is attempted, and increments every time the job is retried.
    /// </para>
    /// <para>
    /// Note: "Phase" is a mostly-redundant concept which represents the design-time for a job (whereas job was the runtime version of a phase).
    /// We've mostly removed the concept of "phase" from Azure Pipelines. Matrix and multi-config jobs are the only place where "phase" is still distinct from "job".
    /// One phase can instantiate multiple jobs which differ only in their inputs.
    /// </para>
    /// </summary>
    public VariableReference PhaseAttempt => GetReference("PhaseAttempt");

    /// <summary>
    /// The human-readable name given to a phase.
    /// </summary>
    public VariableReference PhaseDisplayName => GetReference("PhaseDisplayName");

    /// <summary>
    /// A string-based identifier for a job, typically used for expressing dependencies and accessing output variables.
    /// </summary>
    public VariableReference PhaseName => GetReference("PhaseName");

    /// <summary>
    /// Set to 1 the first time this stage is attempted, and increments every time the job is retried.
    /// </summary>
    public VariableReference StageAttempt => GetReference("StageAttempt");

    /// <summary>
    /// The human-readable name given to a stage.
    /// </summary>
    public VariableReference StageDisplayName => GetReference("StageDisplayName");

    /// <summary>
    /// A string-based identifier for a stage, typically used for expressing dependencies and accessing output variables.
    /// </summary>
    public VariableReference StageName => GetReference("StageName");

    /// <summary>
    /// The URI of the TFS collection or Azure DevOps organization.
    /// For example: <c>https://dev.azure.com/fabrikamfiber/</c>.
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public VariableReference TeamFoundationCollectionUri => GetReference("TeamFoundationCollectionUri");

    /// <summary>
    /// The name of the project that contains this build.
    /// </summary>
    public VariableReference TeamProject => GetReference("TeamProject");

    /// <summary>
    /// The ID of the project that this build belongs to.
    /// </summary>
    public VariableReference TeamProjectId => GetReference("TeamProjectId");
}

/// <summary>
/// Represents the variables that have the prefix <c>System.PullRequest.</c>.
/// </summary>
public sealed class PullRequestVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "System.PullRequest.";

    internal PullRequestVariableReference()
    {
    }

    /// <summary>
    /// If the pull request is from a fork of the repository, this variable is set to True. Otherwise, it is set to False.
    /// </summary>
    public VariableReference IsFork => GetReference("IsFork");

    /// <summary>
    /// The ID of the pull request that caused this build.
    /// For example: <c>17</c>.
    /// (This variable is initialized only if the build ran because of a <see href="https://learn.microsoft.com/en-us/azure/devops/repos/git/branch-policies?view=azure-devops#build-validation">Git PR affected by a branch policy</see>).
    /// </summary>
    public VariableReference PullRequestId => GetReference("PullRequestId");

    /// <summary>
    /// The number of the pull request that caused this build.
    /// This variable is populated for pull requests from GitHub which have a different pull request ID and pull request number.
    /// This variable is only available in a YAML pipeline if the PR is a affected by a branch policy.
    /// </summary>
    public VariableReference PullRequestNumber => GetReference("PullRequestNumber");

    /// <summary>
    /// The branch that is being reviewed in a pull request.
    /// For example: <c>refs/heads/users/raisa/new-feature</c> for Azure Repos.
    /// (This variable is initialized only if the build ran because of a <see href="https://learn.microsoft.com/en-us/azure/devops/repos/git/branch-policies?view=azure-devops#build-validation">Git PR affected by a branch policy</see>).
    /// This variable is only available in a YAML pipeline if the PR is affected by a branch policy.
    /// </summary>
    public VariableReference SourceBranch => GetReference("SourceBranch");

    /// <summary>
    /// The URL to the repo that contains the pull request.
    /// For example: https://dev.azure.com/ouraccount/_git/OurProject.
    /// </summary>
    public VariableReference SourceRepositoryURI => GetReference("SourceRepositoryURI");

    /// <summary>
    /// The branch that is the target of a pull request.
    /// For example: <c>refs/heads/master</c> when your repository is in Azure Repos and master when your repository is in GitHub.
    /// This variable is initialized only if the build ran because of a <see href="https://learn.microsoft.com/en-us/azure/devops/repos/git/branch-policies?view=azure-devops#build-validation">Git PR affected by a branch policy</see>.
    /// This variable is only available in a YAML pipeline if the PR is affected by a branch policy.
    /// </summary>
    public VariableReference TargetBranch => GetReference("TargetBranch");
}

/// <summary>
/// Represents the variables that have the prefix <c>Pipeline.</c>.
/// </summary>
public sealed class PipelineVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Pipeline.";

    internal PipelineVariableReference()
    {
    }

    /// <summary>
    /// Workspace directory for a particular pipeline. This variable has the same value as Agent.BuildDirectory.
    /// For example: <c>/home/vsts/work/1</c>
    /// </summary>
    public VariableReference Workspace => GetReference("Workspace");
}

/// <summary>
/// Represents the variables that have the prefix <c>Environment.</c>.
/// </summary>
public sealed class EnvironmentVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Environment.";

    internal EnvironmentVariableReference()
    {
    }

    /// <summary>
    /// Name of the environment targeted in the deployment job to run the deployment steps and record the deployment history.
    /// For example: <c>smarthotel-dev</c>
    /// </summary>
    public VariableReference Name => GetReference("Name");

    /// <summary>
    /// ID of the environment targeted in the deployment job.
    /// For example: <c>10</c>
    /// </summary>
    public VariableReference Id => GetReference("Id");

    /// <summary>
    /// Name of the specific resource within the environment targeted in the deployment job to run the deployment steps and record the deployment history.
    /// For example, <c>bookings</c> which is a Kubernetes namespace that has been added as a resource to the environment <c>smarthotel-dev</c>.
    /// </summary>
    public VariableReference ResourceName => GetReference("ResourceName");

    /// <summary>
    /// ID of the specific resource within the environment targeted in the deployment job to run the deployment steps.
    /// For example: <c>4</c>
    /// </summary>
    public VariableReference ResourceId => GetReference("ResourceId");
}

/// <summary>
/// Represents the variables that have the prefix <c>Strategy.</c>.
/// </summary>
public sealed class StrategyVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Strategy.";

    internal StrategyVariableReference()
    {
    }

    /// <summary>
    /// The name of the deployment strategy: <c>canary</c>, <c>runOnce</c>, or <c>rolling</c>.
    /// </summary>
    public VariableReference Name => GetReference("Name");

    /// <summary>
    /// The current cycle name in a deployment: <c>PreIteration</c>, <c>Iteration</c>, or <c>PostIteration</c>.
    /// </summary>
    public VariableReference CycleName => GetReference("CycleName");
}

/// <summary>
/// Represents the variables that have the prefix <c>Checks.</c>.
/// </summary>
public sealed class ChecksVariableReference : VariableReferenceBase
{
    /// <inheritdoc/>
    protected override string Prefix => "Checks.";

    internal ChecksVariableReference()
    {
    }

    /// <summary>
    /// Set to 1 the first time this stage is attempted, and increments every time the stage is retried.
    /// This variable can only be used within an approval or check for an environment.For example, you could use $(Checks.StageAttempt) within an Invoke REST API check.
    /// </summary>
    public VariableReference StageAttempt => GetReference("StageAttempt");
}
