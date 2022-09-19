using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class IfCondition : Condition
{
    internal const string IfExpressionStart = $"{ExpressionStart}if ";
    internal static string RemoveBraces(IfCondition condition)
    {
        return RemoveBraces(condition.Serialize());
    }

    internal string RemoveBraces() => RemoveBraces(this);

    internal static string RemoveBraces(string condition)
    {
        return condition.TrimStart($"{ExpressionStart} if ").TrimEnd($" {ExpressionEnd}");
    }

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
