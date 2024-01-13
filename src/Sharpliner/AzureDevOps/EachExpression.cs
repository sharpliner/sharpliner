using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public class EachExpression(string iterator, string collection)
{
    public string Iterator { get; } = iterator;
    public string Collection { get; } = collection;

    public override string ToString()
        => GetEachExpression(Iterator, Collection);

    internal static string GetEachExpression(string iterator, string collection)
        => $"{Condition.ExpressionStart}each {iterator} in {collection}{Condition.ExpressionEnd}";
}

public class EachBlock(string iterator, string collection) : IfCondition
{
    internal override string Serialize() => EachExpression.GetEachExpression(iterator, collection);
    internal override string TagStart => ExpressionStart;
}
