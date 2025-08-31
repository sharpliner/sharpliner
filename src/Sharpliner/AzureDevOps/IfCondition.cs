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
    /// This enables patterns like If.IsBranch().If.Equal() to create nested conditional structures.
    /// </summary>
    public IfConditionBuilder If 
    { 
        get 
        {
            // The key insight is that we need to create a parent-child relationship
            // where this condition becomes the parent of any subsequent conditions.
            // We'll use a dummy AdoExpression as a container.
            var containerExpression = new AdoExpression<object>(new object());
            containerExpression.Condition = this;
            
            // The container becomes the "current context" for building nested conditions
            return new IfConditionBuilder(containerExpression, false);
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
    /// This enables patterns like If().If() to create nested conditional structures.
    /// </summary>
    public new IfConditionBuilder<T> If 
    { 
        get 
        {
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
