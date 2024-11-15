using System;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents generic definition of any arbitrary AzDO task.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-task">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record AzureDevOpsTask : Step
{
    /// <summary>
    /// Task name in the form 'PublishTestResults@2'.
    /// </summary>
    [YamlMember(Order = 1)]
    public string Task { get; }

    /// <summary>
    /// The inputs for the task.
    /// </summary>
    [YamlMember(Order = 101)]
    public TaskInputs Inputs
    {
        get;
        init
        {
            // Add inputs to the existing ones
            foreach (var item in value)
            {
                if (value == null)
                {
                    field.Remove(item.Key);
                }
                else
                {
                    field[item.Key] = item.Value;
                }
            }
        }
    } = [];

    /// <summary>
    /// Number of retries if the task fails.
    /// Default is 0
    /// </summary>
    [YamlMember(Order = 230)]
    public Conditioned<int>? RetryCountOnTaskFailure { get; init; }

    /// <summary>
    /// Instantiates a new instance of <see cref="AzureDevOpsTask"/> with the specified task name.
    /// </summary>
    /// <param name="task">The name of the task.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="task"/> is null or empty.</exception>
    public AzureDevOpsTask(string task) : base()
    {
        if (string.IsNullOrEmpty(task))
        {
            throw new ArgumentException($"'{nameof(task)}' cannot be null or empty.", nameof(task));
        }

        Task = task;
    }

    // This is intended for cases where we need to copy some other task as its full AzureDevOpsTask version
    internal AzureDevOpsTask(string task, Step original)
        : base(original)
    {
        if (string.IsNullOrEmpty(task))
        {
            throw new ArgumentException($"'{nameof(task)}' cannot be null or empty.", nameof(task));
        }

        Task = task;
    }

    /// <summary>
    /// Gets the value of the input parameter.
    /// </summary>
    /// <param name="name">The name of the input parameter.</param>
    /// <param name="defaultValue">The default value to return if the input parameter is not found.</param>
    /// <returns>The value of the input parameter or the default value if the input parameter is not found.</returns>
    protected string? GetString(string name, string? defaultValue = null)
        => Inputs.TryGetValue(name, out var value) ? value.ToString() : defaultValue;

    /// <summary>
    /// Gets the value of the input parameter and parses it as an integer.
    /// </summary>
    /// <param name="name">The name of the input parameter.</param>
    /// <param name="defaultValue">The default value to return if the input parameter is not found.</param>
    /// <returns>The value of the input parameter or the default value if the input parameter is not found.</returns>
    protected int? GetInt(string name, int? defaultValue = null)
        => Inputs.TryGetValue(name, out var value) ? int.Parse(value.ToString()!) : defaultValue;

    /// <summary>
    /// Gets the value of the input parameter and returns <c>true</c> if the value is the literal <c>"true"</c>.
    /// </summary>
    /// <param name="name">The name of the input parameter.</param>
    /// <param name="defaultValue">The default value to return if the input parameter is not found.</param>
    /// <returns><c>true</c> if the value is the literal <c>"true"</c>; otherwise, <c>false</c>.</returns>
    protected bool GetBool(string name, bool defaultValue)
        => Inputs.TryGetValue(name, out var value) ? value.ToString() == "true" : defaultValue;

    /// <summary>
    /// Sets the value of a string input parameter.
    /// </summary>
    /// <param name="name">The name of the input parameter.</param>
    /// <param name="value">The string value to set.</param>
    protected void SetProperty(string name, string? value)
    {
        if (value == null)
        {
            Inputs.Remove(name);
        }
        else
        {
            Inputs[name] = value;
        }
    }

    /// <summary>
    /// Sets the value of an integer input parameter.
    /// </summary>
    /// <param name="name">The name of the input parameter.</param>
    /// <param name="value">The integer value to set.</param>
    protected void SetProperty(string name, int? value) => SetProperty(name, value?.ToString());

    /// <summary>
    /// Sets the value of a boolean input parameter.
    /// </summary>
    /// <param name="name">The name of the input parameter.</param>
    /// <param name="value">The boolean value to set.</param>
    protected void SetProperty(string name, bool? value) => SetProperty(name, value?.ToString().ToLowerInvariant());
}

/// <summary>
/// Represents a collection of task inputs, supports <see cref="Conditioned{T}"/> values.
/// </summary>
public class TaskInputs : ConditionedDictionary { }
