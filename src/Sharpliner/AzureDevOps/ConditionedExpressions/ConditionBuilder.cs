namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// The builder is what let's us start the definition with the "If."
/// and then forces us to add a condition. The condition then forces us to add
/// an actual definition.
/// </summary>
public class ConditionBuilder
{
    internal Conditioned? Parent { get; }

    internal ConditionBuilder(Conditioned? parent = null)
    {
        Parent = parent;
    }

    public Condition Equal(Condition condition)
        => Link(condition);

    public Condition NotEqual(Condition condition)
        => Link(condition);

    public Condition Equal(string expression1, string expression2)
        => Link(new EqualityCondition(true, expression1, expression2));

    public Condition NotEqual(string expression1, string expression2)
        => Link(new EqualityCondition(false, expression1, expression2));

    public Condition And(params Condition[] expressions)
        => Link(new AndCondition(expressions));

    public Condition Or(params Condition[] expressions)
        => Link(new OrCondition(expressions));

    public Condition And(params string[] expressions)
        => Link(new AndCondition(expressions));

    public Condition Or(params string[] expressions)
        => Link(new OrCondition(expressions));

    public Condition IsBranch(string branchName)
        => Link(new BranchCondition(branchName, true));

    public Condition IsNotBranch(string branchName)
        => Link(new BranchCondition(branchName, false));

    public Condition IsPullRequest
        => Link(new BuildReasonCondition("'PullRequest'", true));

    public Condition IsNotPullRequest
        => Link(new BuildReasonCondition("'PullRequest'", false));

    private Condition Link(Condition condition)
    {
        condition.Parent = Parent;
        return condition;
    }
}

public class ConditionBuilder<T>
{
    internal Conditioned<T>? Parent { get; }

    internal ConditionBuilder(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }

    public Condition<T> Equal(string expression1, string expression2)
        => Link(new EqualityCondition<T>(true, expression1, expression2));

    public Condition<T> NotEqual(string expression1, string expression2)
        => Link(new EqualityCondition<T>(false, expression1, expression2));

    public Condition<T> And(params Condition[] expressions)
        => Link(new AndCondition<T>(expressions));

    public Condition<T> Or(params Condition[] expressions)
        => Link(new OrCondition<T>(expressions));

    public Condition<T> And(params string[] expressions)
        => Link(new AndCondition<T>(expressions));

    public Condition<T> Or(params string[] expressions)
        => Link(new OrCondition<T>(expressions));

    public Condition<T> IsBranch(string branchName)
        => Link(new BranchCondition<T>(branchName, true));

    public Condition<T> IsNotBranch(string branchName)
        => Link(new BranchCondition<T>(branchName, false));

    public Condition<T> IsPullRequest
        => Link(new BuildReasonCondition<T>("'PullRequest'", true));

    public Condition<T> IsNotPullRequest
        => Link(new BuildReasonCondition<T>("'PullRequest'", false));

    private Condition<T> Link(Condition<T> condition)
    {
        condition.Parent = Parent;
        return condition;
    }
}
