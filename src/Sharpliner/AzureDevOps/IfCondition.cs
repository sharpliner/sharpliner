using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public abstract class IfCondition : Condition
{
}

public abstract class IfCondition<T> : IfCondition
{
    protected IfCondition(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }
}
