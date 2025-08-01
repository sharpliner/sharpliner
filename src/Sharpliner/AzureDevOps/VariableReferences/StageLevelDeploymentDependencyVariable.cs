﻿using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.VariableReferences;

/// <summary>
/// Class that contains references for dependencies meant for usage within the "stages" section that came from a deployment job.
/// </summary>
public class StageLevelDeploymentDependencyVariable
{
    /// <summary>
    /// Gets a reference to a variable created by a deployment job with the specified name.
    /// Meant for usage with the `condition` section within a `stage`.
    /// This should be used for custom variables.
    /// </summary>
    /// <param name="stageName">The name of the stage that set the variable.</param>
    /// <param name="jobName">The name of the job that set the variable.</param>
    /// <param name="stepName">The name of the step that set the variable.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <returns>A variable reference to the specified name.</returns>
    public StageToStageDeployDependencyVariableReference this[string stageName, string jobName, string stepName, string variableName] => new(stageName, jobName, stepName, variableName);

    /// <summary>
    /// Gets a reference to a variable created by a deployment job with the specified name and containing a resource with the specified name.
    /// Meant for usage with the `condition` section within a `stage`.
    /// This should be used for custom variables.
    /// </summary>
    /// <param name="resourceName">The name of the resource that was specified in the deployment.</param>
    /// <param name="stageName">The name of the stage that set the variable.</param>
    /// <param name="jobName">The name of the job that set the variable.</param>
    /// <param name="stepName">The name of the step that set the variable.</param>
    /// <param name="variableName">The name of the variable.</param>
    /// <returns>A variable reference to the specified name.</returns>
    public StageToStageDeployResourceDependencyVariableReference this[string stageName, string jobName, string stepName, string variableName, string resourceName] => new(stageName, jobName, stepName, variableName, resourceName);
}
