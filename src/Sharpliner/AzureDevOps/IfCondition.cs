using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Represents a condition that can be used in Azure DevOps YAML.
/// </summary>
public abstract class IfCondition : Condition
{
    internal bool IsElseIf = false;

    internal override string TagStart => IsElseIf ? ElseIfTagStart : IfTagStart;

    private static readonly (string Start, string End)[] s_tagsToRemove =
    [
        (IfTagStart, ExpressionEnd),
        (ElseIfTagStart, ExpressionEnd),
        (ElseTagStart, ExpressionEnd),
        ('\'' + IfTagStart, ExpressionEnd + '\''),
        ('\'' + ElseIfTagStart, ExpressionEnd + '\''),
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

    internal string WrapTag(string condition) =>
        TagStart + condition + ExpressionEnd;
}

/// <summary>
/// Represents a condition that can be used in Azure DevOps YAML. This type is used for chaining conditions.
/// </summary>
public abstract class IfCondition<T> : IfCondition
{
    /// <summary>
    /// Creates a new instance of <see cref="IfCondition{T}"/> with the specified parent.
    /// </summary>
    protected IfCondition(AdoExpression<T>? parent = null)
    {
        Parent = parent;
    }
    
    /// <summary>
    /// Starts a new <c>${{ if (...) }}</c> section for chaining conditions.
    /// This enables patterns like If().If() to merge conditions with 'and()'.
    /// </summary>
    public IfConditionBuilder<T> If 
    { 
        get 
        {
            // Create a conditional expression block if it doesn't exist yet
            // This allows for the eager materialization pattern
            var parentExpression = Parent as AdoExpression<T>;
            if (parentExpression == null && Parent != null)
            {
                // Create an empty conditional expression to hold the current condition
                parentExpression = new AdoExpression<T>(default!, this);
                this.Parent = parentExpression;
            }
            
            return new IfConditionBuilder<T>(parentExpression);
        } 
    }
}
