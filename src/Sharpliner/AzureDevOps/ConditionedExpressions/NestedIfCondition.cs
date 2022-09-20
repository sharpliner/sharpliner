namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class NestedIfCondition : IfCondition
{
    private readonly IfCondition _ifCondition;

    public NestedIfCondition(IfCondition ifCondition)
    {
        _ifCondition = ifCondition;
    }

    internal override string Serialize() => _ifCondition.WithoutTags();
}

public class NestedIfCondition<T> : IfCondition<T>
{
    private readonly IfCondition<T> _ifCondition;

    public NestedIfCondition(IfCondition<T> ifCondition)
    {
        _ifCondition = ifCondition;
    }

    internal override string Serialize() => _ifCondition.WithoutTags();
}
