using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class IfCondition : Condition
{
    internal static string RemoveBraces(IfCondition condition)
    {
        return condition.Serialize().TrimStart($"{ExpressionStart} if ").TrimEnd($" {ExpressionEnd}");
    }

    internal string RemoveBraces() => RemoveBraces(this);
}

public abstract class IfCondition<T> : IfCondition
{
    protected IfCondition(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }
}
