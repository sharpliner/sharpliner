using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Represents a condition that can be used in Azure DevOps YAML.
/// </summary>
public abstract class IfCondition : Condition
{
    internal bool IsElseIf = false;
    
    /// <summary>
    /// Starts a new <c>${{ if (...) }}</c> section for chaining conditions.
    /// This enables patterns like If.IsBranch().If.Equal() to merge conditions with 'and()'.
    /// </summary>
    public IfConditionBuilder If 
    { 
        get 
        {
            // Create a wrapper expression around this condition to enable chaining
            var wrapperExpression = new AdoExpression<object>(new object(), this);
            this.Parent = wrapperExpression;
            return new IfConditionBuilder(wrapperExpression, false);
        } 
    }

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
    public new IfConditionBuilder<T> If 
    { 
        get 
        {
            // For consecutive If().If() calls, we want to:
            // 1. If no items have been added yet, merge conditions with 'and()'
            // 2. If items exist, create a nested conditional block
            
            if (Parent is AdoExpression<T> parentExpression)
            {
                return new IfConditionBuilder<T>(parentExpression);
            }
            else
            {
                // Create a minimal expression wrapper for this condition to enable chaining
                var wrapperExpression = new AdoExpression<T>(default(T)!, this);
                this.Parent = wrapperExpression;
                return new IfConditionBuilder<T>(wrapperExpression);
            }
        } 
    }
}
