using System;
using System.Security.Cryptography.X509Certificates;
using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Base class that makes it possible to use dependency based output variables in other jobs.
/// </summary>
public class DependencyVariableReference : VariableReference
{
    /// <summary>
    /// Gets the name of the step where the output variable was set.
    /// </summary>
    public string StepName { get; }

    /// <summary>
    /// Gets the name of the job where the output variable was set.
    /// </summary>
    public string JobName { get; }

    internal DependencyVariableReference(string jobName, string stepName, string variableName) : base(variableName)
    {
        StepName = stepName;
        JobName = jobName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"dependencies.{JobName}.outputs['{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({StepName}.{VariableName})";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is DependencyVariableReference other &&
            VariableName == other.VariableName &&
            JobName == other.JobName &&
            StepName == other.StepName;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => (VariableName + JobName + StepName).GetHashCode();
}

/// <summary>
/// Reference an output variable from a different stage within a stage condition area.  <a href="https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#stage-to-stage-dependencies">More Info</a>
/// </summary>
public class StageToStageDependencyVariableReference : DependencyVariableReference
{
    /// <summary>
    /// Gets the name of the stage where the output variable was set.
    /// </summary>
    public string StageName { get; }

    internal StageToStageDependencyVariableReference(string stageName, string jobName, string stepName, string variableName) : base(jobName, stepName, variableName)
    {
        StageName = stageName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"dependencies.{StageName}.outputs['{JobName}.{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({JobName}.{StepName}.{VariableName})";
}

/// <summary>
/// Reference an output variable from a different job within a job.
/// </summary>
public class JobToJobSameStageDependencyVariableReference : DependencyVariableReference
{
    internal JobToJobSameStageDependencyVariableReference(string jobName, string stepName, string variableName) : base(jobName, stepName, variableName)
    {
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"dependencies.{JobName}.outputs['{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({StepName}.{VariableName})";
}

/// <summary>
/// Reference an output variable from a different stage and job within a job.
/// </summary>
public class JobToJobDifferentStageDependencyVariableReference : DependencyVariableReference
{
    /// <summary>
    /// Gets the name of the stage where the output variable was set.
    /// </summary>
    public string StageName { get; }

    internal JobToJobDifferentStageDependencyVariableReference(string stageName, string jobName, string stepName, string variableName) : base(jobName, stepName, variableName)
    {
        StageName = stageName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"stageDependencies.{StageName}.{JobName}.outputs['{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({StageName}.{JobName}.{StepName}.{VariableName})";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is JobToJobDifferentStageDependencyVariableReference other &&
            VariableName == other.VariableName &&
            JobName == other.JobName &&
            StepName == other.StepName &&
            StageName == other.StageName;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => (VariableName + JobName + StepName + StageName).GetHashCode();
}

/// <summary>
/// Reference an output variable from a deployment job within a job in a different stage.
/// </summary>
public class JobToJobDeployDependencyVariableReference : DependencyVariableReference
{
    /// <summary>
    /// Gets the name of the stage where the output variable was set.
    /// </summary>
    public string StageName { get; }

    internal JobToJobDeployDependencyVariableReference(string stageName, string jobName, string stepName, string variableName) : base(jobName, stepName, variableName)
    {
        StageName = stageName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"stageDependencies.{StageName}.{JobName}.outputs['{JobName}.{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({StageName}.{JobName}.{JobName}.{StepName}.{VariableName})";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is JobToJobDeployDependencyVariableReference other &&
            VariableName == other.VariableName &&
            JobName == other.JobName &&
            StepName == other.StepName &&
            StageName == other.StageName;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => (VariableName + JobName + StepName + StageName).GetHashCode();
}

/// <summary>
/// Reference an output variable from a deployment job within a stage.
/// </summary>
public class StageToStageDeployDependencyVariableReference : DependencyVariableReference
{
    /// <summary>
    /// Gets the name of the stage where the output variable was set.
    /// </summary>
    public string StageName { get; }

    internal StageToStageDeployDependencyVariableReference(string stageName, string jobName, string stepName, string variableName) : base(jobName, stepName, variableName)
    {
        StageName = stageName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"dependencies.{StageName}.outputs['{JobName}.{JobName}.{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({StageName}.{JobName}.{JobName}.{StepName}.{VariableName})";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is StageToStageDeployDependencyVariableReference other &&
            VariableName == other.VariableName &&
            JobName == other.JobName &&
            StepName == other.StepName &&
            StageName == other.StageName;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => (VariableName + JobName + StepName + StageName).GetHashCode();
}

/// <summary>
/// Reference an output variable from a deployment job that includes a resource in different stage.
/// </summary>
public class StageToStageDeployResourceDependencyVariableReference : StageToStageDeployDependencyVariableReference
{
    /// <summary>
    /// Gets the name of the resource used in the deployment where the output variable was set.
    /// </summary>
    public string ResourceName { get; }

    internal StageToStageDeployResourceDependencyVariableReference(string stageName, string jobName, string stepName, string variableName, string resourceName) : base(stageName, jobName, stepName, variableName)
    {
        ResourceName = resourceName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"dependencies.{StageName}.outputs['{JobName}.Deploy_{ResourceName}.{StepName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({StageName}.{ResourceName}.{JobName}.{StepName}.{VariableName})";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is StageToStageDeployResourceDependencyVariableReference other &&
            VariableName == other.VariableName &&
            JobName == other.JobName &&
            StepName == other.StepName &&
            StageName == other.StageName &&
            ResourceName == other.ResourceName;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => (VariableName + JobName + StepName + StageName + ResourceName).GetHashCode();
}
