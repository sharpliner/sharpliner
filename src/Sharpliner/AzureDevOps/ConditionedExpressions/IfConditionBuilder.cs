using Sharpliner.SourceGenerator;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// The builder is what let's us start the definition with the "If."
/// and then forces us to add a condition. The condition then forces us to add
/// an actual definition.
/// </summary>
public partial class IfConditionBuilder
{
    internal Conditioned? Parent { get; }

    internal IfConditionBuilder(Conditioned? parent = null)
    {
        Parent = parent;
    }

    /// <summary>
    /// Specify any condition
    /// </summary>
    public IfCondition IfCondition(string condition)
        => Link(new IfCustomCondition(condition));

    public IfCondition Equal(IfCondition condition)
        => Link(condition);

    public IfCondition NotEqual(IfCondition condition)
        => Link(condition);

    [StringCondition]
    public IfCondition Equal(string expression1, string expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));

    [StringCondition]
    public IfCondition NotEqual(string expression1, string expression2)
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

    [StringCondition]
    public IfCondition StartsWith(string needle, string haystack)
        => new IfStartsWithCondition(needle, haystack);

    [StringCondition]
    public IfCondition EndsWith(string needle, string haystack)
        => new IfEndsWithCondition(needle, haystack);

    [StringCondition]
    public IfCondition Contains(string needle, string haystack)
        => new IfContainsCondition(needle, haystack);

    [StringCondition]
    public IfCondition ContainsValue(string needle, params string[] haystack)
        => new IfContainsValueCondition(needle, haystack);

    [StringCondition]
    public IfCondition In(string needle, params string[] haystack)
        => new IfInCondition(needle, haystack);

    [StringCondition]
    public IfCondition NotIn(string needle, params string[] haystack)
        => new IfNotInCondition(needle, haystack);

    [StringCondition]
    public IfCondition Greater(string first, string second)
        => Link(new IfGreaterCondition(first, second));

    [StringCondition]
    public IfCondition Less(string first, string second)
        => Link(new IfLessCondition(first, second));

    [StringCondition]
    public IfCondition IsBranch(string branchName)
        => Link(new IfBranchCondition(branchName, true));

    [StringCondition]
    public IfCondition IsNotBranch(string branchName)
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

public partial class IfConditionBuilder<T>
{
    internal Conditioned<T>? Parent { get; }

    internal IfConditionBuilder(Conditioned<T>? parent = null)
    {
        Parent = parent;
    }

    public IfCondition<T> Equal(string condition)
        => Link(new IfCustomCondition<T>(condition));

    [StringCondition]
    public IfCondition<T> Equal(string expression1, string expression2)
        => Link(new IfEqualityCondition<T>(true, expression1, expression2));

    [StringCondition]
    public IfCondition<T> NotEqual(string expression1, string expression2)
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

    [StringCondition]
    public IfCondition<T> StartsWith(string needle, string haystack)
        => new IfStartsWithCondition<T>(needle, haystack);

    [StringCondition]
    public IfCondition<T> EndsWith(string needle, string haystack)
        => new IfEndsWithCondition<T>(needle, haystack);

    [StringCondition]
    public IfCondition<T> Contains(string needle, string haystack)
        => new IfContainsCondition<T>(needle, haystack);

    [StringCondition]
    public IfCondition<T> In(string needle, string haystack)
        => new IfInCondition<T>(needle, haystack);

    [StringCondition]
    public IfCondition<T> NotIn(string needle, params string[] haystack)
        => new IfNotInCondition<T>(needle, haystack);

    [StringCondition]
    public IfCondition<T> ContainsValue(string needle, params string[] haystack)
        => new IfContainsValueCondition<T>(needle, haystack);

    [StringCondition]
    public IfCondition<T> IsBranch(string branchName)
        => Link(new IfBranchCondition<T>(branchName, true));

    [StringCondition]
    public IfCondition<T> IsNotBranch(string branchName)
        => Link(new IfBranchCondition<T>(branchName, false));

    [StringCondition]
    public IfCondition<T> Greater(string first, string second)
        => Link(new IfGreaterCondition<T>(first, second));

    [StringCondition]
    public IfCondition<T> Less(string first, string second)
        => Link(new IfLessCondition<T>(first, second));

    public IfCondition<T> IsPullRequest
        => Link(new IfBuildReasonCondition<T>(new StaticVariableReference("PullRequest"), true));

    public IfCondition<T> IsNotPullRequest
        => Link(new IfBuildReasonCondition<T>(new StaticVariableReference("PullRequest"), false));

    private IfCondition<T> Link(IfCondition<T> condition)
    {
        condition.Parent = Parent;
        return condition;
    }
}
