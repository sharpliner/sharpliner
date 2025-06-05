using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

internal class EachExpression(string iterator, string collection)
{
    internal string Iterator { get; } = iterator;
    internal string Collection { get; } = collection;

    public override string ToString()
        => GetEachExpression(Iterator, Collection);

    internal static string GetEachExpression(string iterator, string collection)
        => $"{Condition.ExpressionStart}each {IfCondition.WithoutTags(iterator)} in {IfCondition.WithoutTags(collection)}{Condition.ExpressionEnd}";
}

/// <summary>
/// Represents an <c>each</c> block in the pipeline.
/// </summary>
public class EachBlock : IfCondition
{
    private readonly string _iterator;
    private readonly string _collection;

    internal EachBlock(string iterator, string collection)
    {
        _iterator = iterator;
        _collection = collection;
    }

    internal override string Serialize() => EachExpression.GetEachExpression(_iterator, _collection);
    internal override string TagStart => ExpressionStart;
}
