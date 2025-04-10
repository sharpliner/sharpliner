using System;
using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Class that makes it possible to use dependency based output variables in other jobs.
/// </summary>
public class JobDependencyVariableReference : VariableReference
{
    /// <summary>
    /// Gets the name of the task where the output variable was declared.
    /// </summary>
    public string TaskName { get; }

    /// <summary>
    /// Gets the name of the job where the output variable was declared.
    /// </summary>
    public string JobName { get; }

    internal JobDependencyVariableReference(string variableName, string taskName, string jobName) : base(variableName)
    {
        TaskName = taskName;
        JobName = jobName;
    }

    /// <inheritdoc/>
    public override string RuntimeExpression => $"dependencies.{JobName}.outputs['{TaskName}.{VariableName}']";

    /// <inheritdoc/>
    public override string MacroExpression => $"$({TaskName}.{VariableName})";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is JobDependencyVariableReference other)
        {
            return (
                VariableName == other.VariableName &&
                JobName == other.JobName &&
                TaskName == other.TaskName);
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => VariableName.GetHashCode();
}
