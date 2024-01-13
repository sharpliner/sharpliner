using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public class EachExpression(string iterator, string collection)
{
    public string Iterator { get; } = iterator;
    public string Collection { get; } = collection;

    public override string ToString()
        => $"{Condition.ExpressionStart}{GetEachExpression(Iterator, Collection)}{Condition.ExpressionEnd}";

    internal static string GetEachExpression(string iterator, string collection)
        => $"each {iterator} in {collection}";
}

public class EachBlock(string iterator, string collection) : IfCondition
{
    internal override string Serialize() => WrapTag(EachExpression.GetEachExpression(iterator, collection));
}
