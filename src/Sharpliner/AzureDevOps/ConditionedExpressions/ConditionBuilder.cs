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

    /// <summary>
    /// Specify any condition
    /// </summary>
    public Condition Condition(string condition)
        => Link(new CustomCondition(condition));

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

    public Condition Xor(Condition expression1, Condition expression2)
        => Link(new XorCondition(expression1, expression2));

    public Condition Xor(string expression1, string expression2)
        => Link(new XorCondition(expression1, expression2));

    public Condition StartsWith(string needle, string haystack)
        => new StartsWithCondition(needle, haystack);

    public Condition EndsWith(string needle, string haystack)
        => new EndsWithCondition(needle, haystack);

    public Condition Contains(string needle, string haystack)
        => new ContainsCondition(needle, haystack);

    public Condition ContainsValue(string needle, params string[] haystack)
        => new ContainsValueCondition(needle, haystack);

    public Condition In(string needle, string haystack)
        => new InCondition(needle, haystack);

    public Condition NotIn(string needle, params string[] haystack)
        => new NotInCondition(needle, haystack);

    public Condition Greater(string first, string second)
        => Link(new GreaterCondition(first, second));

    public Condition Less(string first, string second)
        => Link(new LessCondition(first, second));

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

    public Condition<T> Equal(string condition)
        => Link(new CustomCondition<T>(condition));

    public Condition<T> Equal(string expression1, string expression2)
        => Link(new EqualityCondition<T>(true, expression1, expression2));

    public Condition<T> NotEqual(string expression1, string expression2)
        => Link(new EqualityCondition<T>(false, expression1, expression2));

    public Condition<T> And(params Condition[] expressions)
        => Link(new AndCondition<T>(expressions));

    public Condition<T> Or(params Condition[] expressions)
        => Link(new OrCondition<T>(expressions));

    public Condition<T> Xor(Condition expression1, Condition expression2)
        => Link(new XorCondition<T>(expression1, expression2));

    public Condition<T> Xor(string expression1, string expression2)
        => Link(new XorCondition<T>(expression1, expression2));

    public Condition<T> And(params string[] expressions)
        => Link(new AndCondition<T>(expressions));

    public Condition<T> Or(params string[] expressions)
        => Link(new OrCondition<T>(expressions));

    public Condition<T> StartsWith(string needle, string haystack)
        => new StartsWithCondition<T>(needle, haystack);

    public Condition<T> EndsWith(string needle, string haystack)
        => new EndsWithCondition<T>(needle, haystack);

    public Condition<T> Contains(string needle, string haystack)
        => new ContainsCondition<T>(needle, haystack);

    public Condition In(string needle, string haystack)
        => new InCondition<T>(needle, haystack);

    public Condition NotIn(string needle, params string[] haystack)
        => new NotInCondition<T>(needle, haystack);

    public Condition<T> ContainsValue(string needle, params string[] haystack)
        => new ContainsValueCondition<T>(needle, haystack);

    public Condition<T> IsBranch(string branchName)
        => Link(new BranchCondition<T>(branchName, true));

    public Condition<T> IsNotBranch(string branchName)
        => Link(new BranchCondition<T>(branchName, false));

    public Condition<T> Greater(string first, string second)
        => Link(new GreaterCondition<T>(first, second));

    public Condition<T> Less(string first, string second)
        => Link(new LessCondition<T>(first, second));

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
