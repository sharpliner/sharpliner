using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

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

    public Condition Equal(StringRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition(true, expression1, expression2));

    public Condition Equal(StringRuntimeExpression expression1, IRuntimeExpression expression2)
        => Link(new EqualityCondition(true, expression1, expression2));

    public Condition Equal(IRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition(true, expression1, expression2));

    public Condition Equal(IRuntimeExpression expression1, IRuntimeExpression expression2)
        => Link(new EqualityCondition(true, expression1, expression2));

    public Condition NotEqual(StringRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition(false, expression1, expression2));

    public Condition NotEqual(IRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition(false, expression1, expression2));

    public Condition NotEqual(StringRuntimeExpression expression1, IRuntimeExpression expression2)
        => Link(new EqualityCondition(false, expression1, expression2));

    public Condition NotEqual(IRuntimeExpression expression1, IRuntimeExpression expression2)
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

    public Condition StartsWith(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new StartsWithCondition(needle, haystack);

    public Condition StartsWith(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new StartsWithCondition(needle, haystack);

    public Condition StartsWith(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new StartsWithCondition(needle, haystack);

    public Condition StartsWith(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new StartsWithCondition(needle, haystack);

    public Condition EndsWith(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new EndsWithCondition(needle, haystack);

    public Condition EndsWith(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new EndsWithCondition(needle, haystack);

    public Condition EndsWith(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new EndsWithCondition(needle, haystack);

    public Condition EndsWith(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new EndsWithCondition(needle, haystack);

    public Condition Contains(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new ContainsCondition(needle, haystack);

    public Condition Contains(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new ContainsCondition(needle, haystack);

    public Condition Contains(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new ContainsCondition(needle, haystack);

    public Condition Contains(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new ContainsCondition(needle, haystack);

    public Condition ContainsValue(StringRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new ContainsValueCondition(needle, haystack);

    public Condition ContainsValue(StringRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new ContainsValueCondition(needle, haystack);

    public Condition ContainsValue(IRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new ContainsValueCondition(needle, haystack);

    public Condition ContainsValue(IRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new ContainsValueCondition(needle, haystack);

    public Condition ContainsValue(StringRuntimeExpression needle, params object[] haystack)
        => new ContainsValueCondition(needle, haystack.AsRuntimeExpressions());

    public Condition ContainsValue(IRuntimeExpression needle, params object[] haystack)
        => new ContainsValueCondition(needle, haystack.AsRuntimeExpressions());

    public Condition In(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new InCondition(needle, haystack);

    public Condition In(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new InCondition(needle, haystack);

    public Condition In(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new InCondition(needle, haystack);

    public Condition In(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new InCondition(needle, haystack);

    public Condition NotIn(StringRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new NotInCondition(needle, haystack);

    public Condition NotIn(StringRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new NotInCondition(needle, haystack);

    public Condition NotIn(IRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new NotInCondition(needle, haystack);

    public Condition NotIn(IRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new NotInCondition(needle, haystack);

    public Condition NotIn(StringRuntimeExpression needle, params object[] haystack)
        => new NotInCondition(needle, haystack.AsRuntimeExpressions());

    public Condition NotIn(IRuntimeExpression needle, params object[] haystack)
        => new NotInCondition(needle, haystack.AsRuntimeExpressions());

    public Condition Greater(StringRuntimeExpression first, StringRuntimeExpression second)
        => Link(new GreaterCondition(first, second));

    public Condition Greater(StringRuntimeExpression first, IRuntimeExpression second)
        => Link(new GreaterCondition(first, second));

    public Condition Greater(IRuntimeExpression first, StringRuntimeExpression second)
        => Link(new GreaterCondition(first, second));

    public Condition Greater(IRuntimeExpression first, IRuntimeExpression second)
        => Link(new GreaterCondition(first, second));

    public Condition Less(StringRuntimeExpression first, StringRuntimeExpression second)
        => Link(new LessCondition(first, second));

    public Condition Less(StringRuntimeExpression first, IRuntimeExpression second)
        => Link(new LessCondition(first, second));

    public Condition Less(IRuntimeExpression first, StringRuntimeExpression second)
        => Link(new LessCondition(first, second));

    public Condition Less(IRuntimeExpression first, IRuntimeExpression second)
        => Link(new LessCondition(first, second));

    public Condition IsBranch(StringRuntimeExpression branchName)
        => Link(new BranchCondition(branchName, true));

    public Condition IsBranch(IRuntimeExpression branchName)
        => Link(new BranchCondition(branchName, true));

    public Condition IsNotBranch(StringRuntimeExpression branchName)
        => Link(new BranchCondition(branchName, false));

    public Condition IsNotBranch(IRuntimeExpression branchName)
        => Link(new BranchCondition(branchName, false));

    public Condition IsPullRequest
        => Link(new BuildReasonCondition(new StringRuntimeExpression("PullRequest"), true));

    public Condition IsNotPullRequest
        => Link(new BuildReasonCondition(new StringRuntimeExpression("PullRequest"), false));

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

    public Condition<T> Equal(StringRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(true, expression1, expression2));

    public Condition<T> Equal(IRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(true, expression1, expression2));

    public Condition<T> Equal(StringRuntimeExpression expression1, IRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(true, expression1, expression2));

    public Condition<T> Equal(IRuntimeExpression expression1, IRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(true, expression1, expression2));

    public Condition<T> NotEqual(StringRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(false, expression1, expression2));

    public Condition<T> NotEqual(IRuntimeExpression expression1, StringRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(false, expression1, expression2));

    public Condition<T> NotEqual(StringRuntimeExpression expression1, IRuntimeExpression expression2)
        => Link(new EqualityCondition<T>(false, expression1, expression2));

    public Condition<T> NotEqual(IRuntimeExpression expression1, IRuntimeExpression expression2)
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

    public Condition<T> StartsWith(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new StartsWithCondition<T>(needle, haystack);

    public Condition<T> StartsWith(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new StartsWithCondition<T>(needle, haystack);

    public Condition<T> StartsWith(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new StartsWithCondition<T>(needle, haystack);

    public Condition<T> StartsWith(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new StartsWithCondition<T>(needle, haystack);

    public Condition<T> EndsWith(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new EndsWithCondition<T>(needle, haystack);

    public Condition<T> EndsWith(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new EndsWithCondition<T>(needle, haystack);

    public Condition<T> EndsWith(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new EndsWithCondition<T>(needle, haystack);

    public Condition<T> EndsWith(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new EndsWithCondition<T>(needle, haystack);

    public Condition<T> Contains(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new ContainsCondition<T>(needle, haystack);

    public Condition<T> Contains(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new ContainsCondition<T>(needle, haystack);

    public Condition<T> Contains(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new ContainsCondition<T>(needle, haystack);

    public Condition<T> Contains(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new ContainsCondition<T>(needle, haystack);

    public Condition In(StringRuntimeExpression needle, StringRuntimeExpression haystack)
        => new InCondition<T>(needle, haystack);

    public Condition In(StringRuntimeExpression needle, IRuntimeExpression haystack)
        => new InCondition<T>(needle, haystack);

    public Condition In(IRuntimeExpression needle, StringRuntimeExpression haystack)
        => new InCondition<T>(needle, haystack);

    public Condition In(IRuntimeExpression needle, IRuntimeExpression haystack)
        => new InCondition<T>(needle, haystack);

    public Condition NotIn(StringRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new NotInCondition<T>(needle, haystack);

    public Condition NotIn(StringRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new NotInCondition<T>(needle, haystack);

    public Condition NotIn(IRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new NotInCondition<T>(needle, haystack);

    public Condition NotIn(IRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new NotInCondition<T>(needle, haystack);

    public Condition NotIn(StringRuntimeExpression needle, params object[] haystack)
        => new NotInCondition<T>(needle, haystack.AsRuntimeExpressions());

    public Condition NotIn(IRuntimeExpression needle, params object[] haystack)
        => new NotInCondition<T>(needle, haystack.AsRuntimeExpressions());

    public Condition<T> ContainsValue(StringRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new ContainsValueCondition<T>(needle, haystack);

    public Condition<T> ContainsValue(StringRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new ContainsValueCondition<T>(needle, haystack);

    public Condition<T> ContainsValue(IRuntimeExpression needle, params StringRuntimeExpression[] haystack)
        => new ContainsValueCondition<T>(needle, haystack);

    public Condition<T> ContainsValue(IRuntimeExpression needle, params IRuntimeExpression[] haystack)
        => new ContainsValueCondition<T>(needle, haystack);

    public Condition<T> ContainsValue(StringRuntimeExpression needle, params object[] haystack)
        => new ContainsValueCondition<T>(needle, haystack.AsRuntimeExpressions());

    public Condition<T> ContainsValue(IRuntimeExpression needle, params object[] haystack)
        => new ContainsValueCondition<T>(needle, haystack.AsRuntimeExpressions());

    public Condition<T> IsBranch(StringRuntimeExpression branchName)
        => Link(new BranchCondition<T>(branchName, true));

    public Condition<T> IsBranch(IRuntimeExpression branchName)
        => Link(new BranchCondition<T>(branchName, true));

    public Condition<T> IsNotBranch(StringRuntimeExpression branchName)
        => Link(new BranchCondition<T>(branchName, false));

    public Condition<T> IsNotBranch(IRuntimeExpression branchName)
        => Link(new BranchCondition<T>(branchName, false));

    public Condition<T> Greater(StringRuntimeExpression first, StringRuntimeExpression second)
        => Link(new GreaterCondition<T>(first, second));

    public Condition<T> Greater(StringRuntimeExpression first, IRuntimeExpression second)
        => Link(new GreaterCondition<T>(first, second));

    public Condition<T> Greater(IRuntimeExpression first, StringRuntimeExpression second)
        => Link(new GreaterCondition<T>(first, second));

    public Condition<T> Greater(IRuntimeExpression first, IRuntimeExpression second)
        => Link(new GreaterCondition<T>(first, second));

    public Condition<T> Less(StringRuntimeExpression first, StringRuntimeExpression second)
        => Link(new LessCondition<T>(first, second));

    public Condition<T> Less(StringRuntimeExpression first, IRuntimeExpression second)
        => Link(new LessCondition<T>(first, second));

    public Condition<T> Less(IRuntimeExpression first, StringRuntimeExpression second)
        => Link(new LessCondition<T>(first, second));

    public Condition<T> Less(IRuntimeExpression first, IRuntimeExpression second)
        => Link(new LessCondition<T>(first, second));

    public Condition<T> IsPullRequest
        => Link(new BuildReasonCondition<T>(new StringRuntimeExpression("PullRequest"), true));

    public Condition<T> IsNotPullRequest
        => Link(new BuildReasonCondition<T>(new StringRuntimeExpression("PullRequest"), false));

    private Condition<T> Link(Condition<T> condition)
    {
        condition.Parent = Parent;
        return condition;
    }
}
