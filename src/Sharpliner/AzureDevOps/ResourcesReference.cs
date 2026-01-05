using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Class that contains references to Azure DevOps pipeline resources.
/// Provides access to pipeline resource metadata variables.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">official Azure DevOps pipelines documentation</see>.
/// </summary>
public class ResourcesReference
{
    /// <summary>
    /// Access to pipeline resource metadata variables.
    /// Use the indexer with the pipeline alias to access metadata for a specific pipeline resource.
    /// </summary>
    public PipelineResourceReference Pipeline { get; } = new();
}

/// <summary>
/// Provides access to pipeline resource metadata through string indexer.
/// </summary>
public sealed class PipelineResourceReference
{
    /// <summary>
    /// Gets a reference to pipeline resource metadata with the specified alias.
    /// </summary>
    /// <param name="alias">The alias of the pipeline resource as defined in the resources section.</param>
    /// <returns>A pipeline resource metadata reference for the specified alias.</returns>
    public PipelineResourceMetadata this[string alias] => new(alias);
}

/// <summary>
/// Represents metadata variables for a pipeline resource.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/resources-pipelines-pipeline#the-pipeline-resource-metadata-as-predefined-variables">official Azure DevOps pipelines documentation</see>.
/// </summary>
public sealed class PipelineResourceMetadata
{
    private readonly string _prefix;

    internal PipelineResourceMetadata(string alias)
    {
        _prefix = "resources.pipeline." + alias + ".";
    }

    private VariableReference GetReference(string propertyName) => new(_prefix + propertyName);

    /// <summary>
    /// The project name of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.projectName)
    /// </summary>
    public VariableReference ProjectName => GetReference("projectName");

    /// <summary>
    /// The project ID of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.projectID)
    /// </summary>
    public VariableReference ProjectID => GetReference("projectID");

    /// <summary>
    /// The pipeline name of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.pipelineName)
    /// </summary>
    public VariableReference PipelineName => GetReference("pipelineName");

    /// <summary>
    /// The pipeline ID of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.pipelineID)
    /// </summary>
    public VariableReference PipelineID => GetReference("pipelineID");

    /// <summary>
    /// The run name of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.runName)
    /// </summary>
    public VariableReference RunName => GetReference("runName");

    /// <summary>
    /// The run ID of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.runID)
    /// </summary>
    public VariableReference RunID => GetReference("runID");

    /// <summary>
    /// The run URI of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.runURI)
    /// </summary>
    public VariableReference RunURI => GetReference("runURI");

    /// <summary>
    /// The source branch of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.sourceBranch)
    /// </summary>
    public VariableReference SourceBranch => GetReference("sourceBranch");

    /// <summary>
    /// The source commit of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.sourceCommit)
    /// </summary>
    public VariableReference SourceCommit => GetReference("sourceCommit");

    /// <summary>
    /// The source provider of the pipeline resource.
    /// Example: $(resources.pipeline.source-pipeline.sourceProvider)
    /// </summary>
    public VariableReference SourceProvider => GetReference("sourceProvider");

    /// <summary>
    /// The user who requested the pipeline run.
    /// Example: $(resources.pipeline.source-pipeline.requestedFor)
    /// </summary>
    public VariableReference RequestedFor => GetReference("requestedFor");

    /// <summary>
    /// The ID of the user who requested the pipeline run.
    /// Example: $(resources.pipeline.source-pipeline.requestedForID)
    /// </summary>
    public VariableReference RequestedForID => GetReference("requestedForID");
}
