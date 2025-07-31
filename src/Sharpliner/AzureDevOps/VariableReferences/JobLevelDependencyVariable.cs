using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.VariableReferences;

/// <summary>
/// Class that contains references for dependencies meant for usage within the "job" section
/// </summary>
public class JobLevelDependencyVariable
{
    /// <summary>
    /// <para>
    /// Allows the <c>;${{ dependencies.&lt;stage-name&gt;.outputs['&lt;job-name&gt;.&lt;job-name&gt;.&lt;step-name&gt;.&lt;variable-name&gt;'] }}</c> or <c>;${{ dependencies.&lt;stage-name&gt;.outputs['&lt;job-name&gt;.&lt;resource-name&gt;.&lt;step-name&gt;.&lt;variable-name&gt;'] }}</c> notation for dependency output variables.
    /// This is meant to be used in `conditions` section of a `stage` and only for variables that came from deployment jobs.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// dependencies.stage.deploy["resource", "stage", "job", "step", "variable"]
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// ${{ stageDependencies.stage.outputs['job.resource.step.variable'] }}
    /// </code>
    /// </summary>
    public readonly JobLevelDeploymentDependencyVariable deploy = new();

    /// <summary>
    /// Gets a reference to a variable created by another job within the same stage with the specified name.
    /// This should be used for custom variables.
    /// </summary>
    /// <param name="jobName">The name of the job that set the variable.</param>
    /// <param name="stepName">The name of the step that set the variable.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <returns>A variable reference to the specified name.</returns>
    public JobToJobSameStageDependencyVariableReference this[string jobName, string stepName, string variableName] => new(jobName, stepName, variableName);


    /// <summary>
    /// Gets a reference to a variable created by another job within a different stage with the specified name.
    /// This should be used for custom variables.
    /// </summary>
    /// <param name="stageName">The name of the stage that set the variable.</param>
    /// <param name="jobName">The name of the job that set the variable.</param>
    /// <param name="stepName">The name of the step that set the variable.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <returns>A variable reference to the specified name.</returns>
    public JobToJobDifferentStageDependencyVariableReference this[string stageName, string jobName, string stepName, string variableName] => new(stageName, jobName, stepName, variableName);
}
