using System;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents generic definition of any arbitrary AzDO task.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=example%2Cparameter-schema#task">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record AzureDevOpsTask : Step
{
    private readonly TaskInputs _inputs = new();

    /// <summary>
    /// Task name in the form 'PublishTestResults@2'.
    /// </summary>
    [YamlMember(Order = 1)]
    public string Task { get; }

    [YamlMember(Order = 101)]
    public TaskInputs Inputs
    {
        get => _inputs;
        init
        {
            // Add inputs to the existing ones
            foreach (var item in value)
            {
                if (value == null)
                {
                    if (_inputs.ContainsKey(item.Key))
                    {
                        _inputs.Remove(item.Key);
                    }
                }
                else
                {
                    _inputs[item.Key] = item.Value;
                }
            }
        }
    }

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

    protected string? GetString(string name, string? defaultValue = null)
        => Inputs.TryGetValue(name, out var value) ? value.ToString() : defaultValue;

    protected int? GetInt(string name, int? defaultValue = null)
        => Inputs.TryGetValue(name, out var value) ? int.Parse(value.ToString()!) : defaultValue;

    protected bool GetBool(string name, bool defaultValue)
        => Inputs.TryGetValue(name, out var value) ? value.ToString() == "true" : defaultValue;

    protected void SetProperty(string name, string? value)
    {
        if (value == null)
        {
            if (Inputs.ContainsKey(name))
            {
                Inputs.Remove(name);
            }
        }
        else
        {
            Inputs[name] = value;
        }
    }
}

public class TaskInputs : ConditionedDictionary { }
