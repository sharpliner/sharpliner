using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public class EachExpression(string iterator, string collection)
{
    public string Iterator { get; } = iterator;
    public string Collection { get; } = collection;

    public override string ToString()
        => $"{Condition.ExpressionStart}each {Iterator} in {Collection}{Condition.ExpressionEnd}";
}

public class EachBlock(string iterator, string collection) : Condition
{
    internal override string Serialize() => throw new System.NotImplementedException();
}
