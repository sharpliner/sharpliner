using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// The builder is what let's us start the definition with the "If."
/// and then forces us to add a condition. The condition then forces us to add
/// an actual definition.
/// </summary>
public class IfConditionBuilder
{
    internal Conditioned? Parent { get; }

    internal IfConditionBuilder(Conditioned? parent = null)
    {
        Parent = parent;
    }

    /// <summary>
    /// Specify any condition
    /// </summary>
    public IfCondition Condition(string condition)
        => Link(new IfCustomCondition(condition));

    public IfCondition Equal(IfCondition condition)
        => Link(condition);

    public IfCondition NotEqual(IfCondition condition)
        => Link(condition);

    public IfCondition Equal(IfStringOrVariableOrParameter expression1, IfStringOrVariableOrParameter expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));

    public IfCondition NotEqual(IfStringOrVariableOrParameter expression1, IfStringOrVariableOrParameter expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));

    public IfCondition And(params IfCondition[] expressions)
        => Link(new IfAndCondition(expressions));

    public IfCondition Or(params IfCondition[] expressions)
        => Link(new IfOrCondition(expressions));

    public IfCondition And(params string[] expressions)
        => Link(new IfAndCondition(expressions));

    public IfCondition Or(params string[] expressions)
        => Link(new IfOrCondition(expressions));

    public IfCondition Xor(IfCondition expression1, IfCondition expression2)
        => Link(new IfXorCondition(expression1, expression2));

    public IfCondition Xor(string expression1, string expression2)
        => Link(new IfXorCondition(expression1, expression2));

    public IfCondition StartsWith(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfStartsWithCondition(needle, haystack);

    public IfCondition EndsWith(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfEndsWithCondition(needle, haystack);

    public IfCondition Contains(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfContainsCondition(needle, haystack);

    public IfCondition ContainsValue(IfStringOrVariableOrParameter needle, params IfStringOrVariableOrParameter[] haystack)
        => new IfContainsValueCondition(needle, haystack);

    public IfCondition In(IfStringOrVariableOrParameter needle, params IfStringOrVariableOrParameter[] haystack)
        => new IfInCondition(needle, haystack);

    public IfCondition NotIn(IfStringOrVariableOrParameter needle, params IfStringOrVariableOrParameter[] haystack)
        => new IfNotInCondition(needle, haystack);

    public IfCondition Greater(IfStringOrVariableOrParameter first, IfStringOrVariableOrParameter second)
        => Link(new IfGreaterCondition(first, second));

    public IfCondition Less(IfStringOrVariableOrParameter first, IfStringOrVariableOrParameter second)
        => Link(new IfLessCondition(first, second));

    public IfCondition IsBranch(IfStringOrVariableOrParameter branchName)
        => Link(new IfBranchCondition(branchName, true));

    public IfCondition IsNotBranch(IfStringOrVariableOrParameter branchName)
        => Link(new IfBranchCondition(branchName, false));

    public IfCondition IsPullRequest
        => Link(new IfBuildReasonCondition(new string("PullRequest"), true));

    public IfCondition IsNotPullRequest
        => Link(new IfBuildReasonCondition(new string("PullRequest"), false));

    private IfCondition Link(IfCondition condition)
    {
        condition.Parent = Parent;
        return condition;
    }
}

public class IfConditionBuilder<T>
{
    internal Conditioned<T>? Parent { get; }

    internal IfConditionBuilder(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }

    public IfCondition<T> Equal(string condition)
        => Link(new IfCustomCondition<T>(condition));

    public IfCondition<T> Equal(IfStringOrVariableOrParameter expression1, IfStringOrVariableOrParameter expression2)
        => Link(new IfEqualityCondition<T>(true, expression1, expression2));

    public IfCondition<T> NotEqual(IfStringOrVariableOrParameter expression1, IfStringOrVariableOrParameter expression2)
    {
        return Link(new IfEqualityCondition<T>(false, expression1, expression2));
    }

    public IfCondition<T> And(params IfCondition[] expressions)
        => Link(new IfAndCondition<T>(expressions));

    public IfCondition<T> Or(params IfCondition[] expressions)
        => Link(new IfOrCondition<T>(expressions));

    public IfCondition<T> Xor(IfCondition expression1, IfCondition expression2)
        => Link(new IfXorCondition<T>(expression1, expression2));

    public IfCondition<T> Xor(string expression1, string expression2)
        => Link(new IfXorCondition<T>(expression1, expression2));

    public IfCondition<T> And(params string[] expressions)
        => Link(new IfAndCondition<T>(expressions));

    public IfCondition<T> Or(params string[] expressions)
        => Link(new IfOrCondition<T>(expressions));

    public IfCondition<T> StartsWith(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfStartsWithCondition<T>(needle, haystack);

    public IfCondition<T> EndsWith(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfEndsWithCondition<T>(needle, haystack);

    public IfCondition<T> Contains(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfContainsCondition<T>(needle, haystack);

    public IfCondition<T> In(IfStringOrVariableOrParameter needle, IfStringOrVariableOrParameter haystack)
        => new IfInCondition<T>(needle, haystack);

    public IfCondition<T> NotIn(IfStringOrVariableOrParameter needle, params IfStringOrVariableOrParameter[] haystack)
        => new IfNotInCondition<T>(needle, haystack);

    public IfCondition<T> ContainsValue(IfStringOrVariableOrParameter needle, params IfStringOrVariableOrParameter[] haystack)
        => new IfContainsValueCondition<T>(needle, haystack);

    public IfCondition<T> IsBranch(IfStringOrVariableOrParameter branchName)
        => Link(new IfBranchCondition<T>(branchName, true));

    public IfCondition<T> IsNotBranch(IfStringOrVariableOrParameter branchName)
        => Link(new IfBranchCondition<T>(branchName, false));

    public IfCondition<T> Greater(IfStringOrVariableOrParameter first, IfStringOrVariableOrParameter second)
        => Link(new IfGreaterCondition<T>(first, second));

    public IfCondition<T> Less(IfStringOrVariableOrParameter first, IfStringOrVariableOrParameter second)
        => Link(new IfLessCondition<T>(first, second));

    public IfCondition<T> IsPullRequest
        => Link(new IfBuildReasonCondition<T>("PullRequest", true));

    public IfCondition<T> IsNotPullRequest
        => Link(new IfBuildReasonCondition<T>("PullRequest", false));

    private IfCondition<T> Link(IfCondition<T> condition)
    {
        condition.Parent = Parent;
        return condition;
    }
}
