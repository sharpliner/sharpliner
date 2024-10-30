using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Represents a condition that can be used in Azure DevOps YAML.
/// </summary>
public abstract class IfCondition : Condition
{
    private static readonly (string Start, string End)[] s_tagsToRemove =
    [
        (IfTagStart, ExpressionEnd),
        (ElseTagStart, ExpressionEnd),
        ('\'' + IfTagStart, ExpressionEnd + '\''),
        ('\'' + ElseTagStart, ExpressionEnd + '\''),
        (ExpressionStart, ExpressionEnd),
        ('\'' + ExpressionStart, ExpressionEnd + '\''),
    ];

    internal static string WithoutTags(IfCondition condition)
    {
        return WithoutTags(condition.Serialize());
    }

    /// <summary>
    /// Returns the YAML representation of the condition without the tags.
    /// </summary>
    public string WithoutTags() => WithoutTags(this);

    internal static string WithoutTags(string condition)
    {
        foreach (var (start, end) in s_tagsToRemove)
        {
            if (condition.StartsWith(start) && condition.EndsWith(end))
            {
                condition = condition.TrimStart(start).TrimEnd(end);
                break;
            }
        }

        return condition;
    }

    internal static string WrapTag(string condition) =>
        IfTagStart + condition + ExpressionEnd;
}

/// <summary>
/// Represents a condition that can be used in Azure DevOps YAML. This type is used for chaining conditions.
/// </summary>
public abstract class IfCondition<T> : IfCondition
{
    /// <summary>
    /// Creates a new instance of <see cref="IfCondition{T}"/> with the specified parent.
    /// </summary>
    protected IfCondition(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }
}
