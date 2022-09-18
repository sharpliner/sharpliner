using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class IfCondition : Condition
{
    internal const string IfExpressionStart = $"{ExpressionStart}if ";
    internal static string RemoveBraces(IfCondition condition)
    {
        return condition.Serialize().TrimStart($"{ExpressionStart} if ").TrimEnd($" {ExpressionEnd}");
    }

    internal string RemoveBraces() => RemoveBraces(this);

    internal static string WrapBraces(string condition) =>
        IfExpressionStart + condition + Condition.ExpressionEnd;
}

public abstract class IfCondition<T> : IfCondition
{
    protected IfCondition(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }
}
