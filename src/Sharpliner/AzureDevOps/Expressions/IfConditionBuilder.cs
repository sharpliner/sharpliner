using Sharpliner.AzureDevOps.Expressions.Arguments;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// The builder is what let's us start the definition with the "If."
/// and then forces us to add a condition. The condition then forces us to add
/// an actual definition.
/// </summary>
public class IfConditionBuilder
{
    internal bool IsElseIf { get; } = false;
    internal AdoExpression? Parent { get; }

    internal IfConditionBuilder(AdoExpression? parent = null) : this(parent, false) { }

    internal IfConditionBuilder(AdoExpression? parent = null, bool isElseIf = false)
    {
        Parent = parent;
        IsElseIf = isElseIf;
    }

    /// <summary>
    /// Use this to specify any custom condition (in case you miss some operator or expression).
    /// </summary>
    /// <param name="condition">The condition to specify.</param>
    /// <returns>The new condition.</returns>
    public IfCondition Condition(string condition)
        => Link(new IfCustomCondition(condition));

    /// <summary>
    /// <para>
    /// Use this to specify a boolean parameter reference as a condition.
    /// </para>
    /// For example:
    /// <code language="csharp">
    /// If.Condition(parameters["IsInternal"])
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if parameters.IsInternal }}:
    /// </code>
    /// </summary>
    /// <param name="parameter">The boolean parameter reference to use as a condition.</param>
    /// <returns>The new condition.</returns>
    public IfCondition Condition(ParameterReference parameter) => new IfParameterCondition(parameter);

    /// <summary>
    /// <para>
    /// Use this to specify a boolean parameter reference as a condition.
    /// </para>
    /// For example:
    /// <code language="csharp">
    /// Parameter&lt;bool&gt; isInternal = new BooleanParameter("IsInternal");
    /// </code>
    /// And then use the parameter:
    /// <code language="csharp">
    /// If.Condition(isInternal)
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if parameters.IsInternal }}:
    /// </code>
    /// </summary>
    /// <param name="parameter">The boolean parameter reference to use as a condition.</param>
    /// <returns>The new condition.</returns>
    public IfCondition Condition(Parameter<bool?> parameter) => new IfParameterCondition(new ParameterReference(parameter.Name));

    /// <summary>
    /// Utility that appends an <c>eq(expression1, expression2)</c> condition to an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Equal(variables.Build.Reason, "PullRequest")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
    /// </code>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>eq</c> condition with the specified expressions.</returns>
    public IfCondition Equal(IfExpression expression1, IfExpression expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>ne(expression1, expression2)</c> condition to an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.NotEqual(variables.Build.Reason, "PullRequest")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if ne(variables['Build.Reason'], 'PullRequest') }}:
    /// </code>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>ne</c> condition with the specified expressions.</returns>
    public IfCondition NotEqual(IfExpression expression1, IfExpression expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>and(expressions)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.And(
    ///    Equal(variables["_RunAsInternal"], "true"),
    ///    Equal(variables["_RunAsExternal"], "true")
    /// )
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if and(eq(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    /// </code>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    public IfCondition And(params IfCondition[] expressions)
        => Link(new IfAndCondition(expressions));

    /// <summary>
    /// Utility that appends an <c>or(expressions)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Or(
    ///    Equal(variables["_RunAsInternal"], "true"),
    ///    Equal(variables["_RunAsExternal"], "true")
    /// )
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if or(eq(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    /// </code>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    public IfCondition Or(params IfCondition[] expressions)
        => Link(new IfOrCondition(expressions));

    /// <summary>
    /// Utility that appends an <c>and(expressions)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.And(
    ///    Equal("variables['_RunAsInternal']", "true"),
    ///    Equal("variables['_RunAsExternal']", "true")
    /// )
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if and(eq(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    /// </code>
    /// <remarks>
    /// This method is useful when you want to specify the expressions as strings.
    /// In most cases, you should use the <see cref="And(IfCondition[])"/> method instead.
    /// </remarks>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    public IfCondition And(params string[] expressions)
        => Link(new IfAndCondition(expressions));

    /// <summary>
    /// Utility that appends an <c>or(expressions)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Or(
    ///    Equal("variables['_RunAsInternal']", "true"),
    ///    Equal("variables['_RunAsExternal']", "true")
    /// )
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if or(eq(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    /// </code>
    /// <remarks>
    /// This method is useful when you want to specify the expressions as strings.
    /// In most cases, you should use the <see cref="Or(IfCondition[])"/> method instead.
    /// </remarks>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    public IfCondition Or(params string[] expressions)
        => Link(new IfOrCondition(expressions));

    /// <summary>
    /// Utility that appends an <c>xor(expression1, expression2)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Xor(
    ///   Equal(variables["IsInternal"], "true"),
    ///   Equal(variables["IsExternal"], "true")
    /// )
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if xor(eq(variables['IsInternal'], true), eq(variables['IsExternal'], true)) }}:
    /// </code>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    public IfCondition Xor(IfCondition expression1, IfCondition expression2)
        => Link(new IfXorCondition(expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>xor(expression1, expression2)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Xor(
    ///   Equal("variables['IsInternal']", "true"),
    ///   Equal("variables['IsExternal']", "true")
    /// )
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if xor(eq(variables['IsInternal'], true), eq(variables['IsExternal'], true)) }}:
    /// </code>
    /// <remarks>
    /// This method is useful when you want to specify the expressions as strings.
    /// In most cases, you should use the <see cref="Xor(IfCondition, IfCondition)"/> method instead.
    /// </remarks>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    public IfCondition Xor(string expression1, string expression2)
        => Link(new IfXorCondition(expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>startswith(needle, haystack)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.StartsWith("refs/heads/feature/", variables.Build.SourceBranch)
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if startsWith(variables['Build.SourceBranch'], 'refs/heads/feature/') }}:
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for at start</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>startsWith</c> condition with the specified expressions.</returns>
    public IfCondition StartsWith(IfExpression needle, IfExpression haystack)
        => new IfStartsWithCondition(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>endswith(needle, haystack)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.EndsWith("/feature/ui-mode", variables.Build.SourceBranch)
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if endsWith(variables['Build.SourceBranch'], '/feature/ui-mode') }}:
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for at end</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>endsWith</c> condition with the specified expressions.</returns>
    public IfCondition EndsWith(IfExpression needle, IfExpression haystack)
        => new IfEndsWithCondition(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>contains(needle, haystack)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Contains("feature", variables.Build.SourceBranch)
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if contains(variables['Build.SourceBranch'], 'feature') }}:
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>contains</c> condition with the specified expressions.</returns>
    public IfCondition Contains(IfExpression needle, IfExpression haystack)
        => new IfContainsCondition(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>containsValue(needle, haystack)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.ContainsValue(variables.Build.SourceBranch, parameters["branchOptions"])
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ containsValue(parameters.branchOptions, variables['Build.SourceBranch']) }}:
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>containsValue</c> condition with the specified expressions.</returns>
    public IfCondition ContainsValue(IfExpression needle, params IfExpression[] haystack)
        => new IfContainsValueCondition(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>in(needle, haystack)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.In(variables.Build.SourceBranch, "refs/heads/feature/", "refs/heads/main")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if in(variables['Build.SourceBranch'], 'refs/heads/feature/', 'refs/heads/main') }}:
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>An <c>in</c> condition with the specified expressions.</returns>
    public IfCondition In(IfExpression needle, params IfExpression[] haystack)
        => new IfInCondition(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>notIn(needle, haystack)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.NotIn(variables.Build.SourceBranch, "refs/heads/feature/", "refs/heads/main")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if notIn(variables['Build.SourceBranch'], 'refs/heads/feature/', 'refs/heads/main') }}:
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>An <c>notIn</c> condition with the specified expressions.</returns>
    public IfCondition NotIn(IfExpression needle, params IfExpression[] haystack)
        => new IfNotInCondition(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>gt(expression1, expression2)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Greater(variables.Build.BuildNumber, "100")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if gt(variables['Build.BuildNumber'], 100) }}:
    /// </code>
    /// </summary>
    /// <param name="first">First expression</param>
    /// <param name="second">Second expression</param>
    /// <returns>An <c>gt</c> condition with the specified expressions.</returns>
    public IfCondition Greater(IfExpression first, IfExpression second)
        => Link(new IfGreaterCondition(first, second));

    /// <summary>
    /// Utility that appends an <c>lt(expression1, expression2)</c> condition an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.Less(variables.Build.BuildNumber, "100")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if lt(variables['Build.BuildNumber'], 100) }}:
    /// </code>
    /// </summary>
    /// <param name="first">First expression</param>
    /// <param name="second">Second expression</param>
    /// <returns>An <c>lt</c> condition with the specified expressions.</returns>
    public IfCondition Less(IfExpression first, IfExpression second)
        => Link(new IfLessCondition(first, second));

    /// <summary>
    /// Utility that appends an <c>eq(variables['Build.SourceBranch'], 'refs/heads/{branchName}')</c> condition to an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.IsBranch("main")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    /// </code>
    /// </summary>
    /// <param name="branchName">The name of the branch to check for.</param>
    /// <returns>An <c>eq</c> condition with the specified expressions.</returns>
    public IfCondition IsBranch(IfExpression branchName)
        => Link(new IfBranchCondition(branchName, true));

    /// <summary>
    /// Utility that appends an <c>ne(variables['Build.SourceBranch'], 'refs/heads/{branchName}')</c> condition to an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.IsNotBranch("main")
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if ne(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    /// </code>
    /// </summary>
    /// <param name="branchName">The name of the branch to check for.</param>
    /// <returns>An <c>ne</c> condition with the specified expressions.</returns>
    public IfCondition IsNotBranch(IfExpression branchName)
        => Link(new IfBranchCondition(branchName, false));

    /// <summary>
    /// Utility that appends an <c>eq(variables['Build.Reason'], 'PullRequest')</c> condition to an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.IsPullRequest
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
    /// </code>
    /// </summary>
    public IfCondition IsPullRequest
        => Link(new IfBuildReasonCondition(new string("PullRequest"), true));

    /// <summary>
    /// Utility that appends an <c>ne(variables['Build.Reason'], 'PullRequest')</c> condition to an <c>${{ if() }}</c> section.
    /// For example:
    /// <code language="csharp">
    /// If.IsNotPullRequest
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if ne(variables['Build.Reason'], 'PullRequest') }}:
    /// </code>
    /// </summary>
    public IfCondition IsNotPullRequest
        => Link(new IfBuildReasonCondition(new string("PullRequest"), false));

    private IfCondition Link(IfCondition condition)
    {
        condition.Parent = Parent;
        condition.IsElseIf = IsElseIf;
        return condition;
    }
}

/// <summary>
/// The builder is what let's us continue appending conditions after specifying the definition type
/// and then forces us to add a specific definition type.
/// </summary>
public class IfConditionBuilder<T>
{
    internal bool IsElseIf { get; } = false;
    internal AdoExpression<T>? Parent { get; }

    internal IfConditionBuilder(AdoExpression<T>? parent = null) : this(parent, false) { }

    internal IfConditionBuilder(AdoExpression<T>? parent = null, bool isElseIf = false)
    {
        Parent = parent;
        IsElseIf = isElseIf;
    }

    /// <summary>
    /// Utility that appends an <c>eq(expression1, expression2)</c> condition to an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Equal("C", "D")
    ///         .Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if eq('C', 'D') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>eq</c> condition with the specified expressions.</returns>
    public IfCondition<T> Equal(IfExpression expression1, IfExpression expression2)
        => Link(new IfEqualityCondition<T>(true, expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>eq(expression1, expression2)</c> condition to an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.NotEqual("C", "D")
    ///         .Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if ne('C', 'D') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>ne</c> condition with the specified expressions.</returns>
    public IfCondition<T> NotEqual(IfExpression expression1, IfExpression expression2)
    {
        return Link(new IfEqualityCondition<T>(false, expression1, expression2));
    }

    /// <summary>
    /// Utility that appends an <c>and(expressions)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.And(
    ///         Equal(variables["_RunAsInternal"], "true"),
    ///         Equal(variables["_RunAsExternal"], "true")
    ///     ).Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if and(eq(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    public IfCondition<T> And(params IfCondition[] expressions)
        => Link(new IfAndCondition<T>(expressions));

    /// <summary>
    /// Utility that appends an <c>or(expressions)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Or(
    ///         Equal(variables["_RunAsInternal"], "true"),
    ///         Equal(variables["_RunAsExternal"], "true")
    ///     ).Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if and(or(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    public IfCondition<T> Or(params IfCondition[] expressions)
        => Link(new IfOrCondition<T>(expressions));

    /// <summary>
    /// Utility that appends an <c>xor(expressions)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Xor(
    ///         Equal(variables["IsInternal"], "true"),
    ///         Equal(variables["IsExternal"], "true")
    ///     ).Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if and(xor(variables['IsInternal'], true), eq(variables['IsExternal'], true)) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    public IfCondition<T> Xor(IfCondition expression1, IfCondition expression2)
        => Link(new IfXorCondition<T>(expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>xor(expressions)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Xor(
    ///         Equal("variables['IsInternal']", "true"),
    ///         Equal("variables['IsExternal']", "true")
    ///     ).Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if and(xor(variables['IsInternal'], true), eq(variables['IsExternal'], true)) }}:
    ///     name: pool-B
    /// </code>
    /// <remarks>
    /// This method is useful when you want to specify the expressions as strings.
    /// In most cases, you should use the <see cref="Xor(IfCondition, IfCondition)"/> method instead.
    /// </remarks>
    /// </summary>
    /// <param name="expression1">First expression</param>
    /// <param name="expression2">Second expression</param>
    /// <returns>An <c>xor</c> condition with the specified expressions.</returns>
    public IfCondition<T> Xor(string expression1, string expression2)
        => Link(new IfXorCondition<T>(expression1, expression2));

    /// <summary>
    /// Utility that appends an <c>and(expressions)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.And(
    ///         Equal("variables['_RunAsInternal']", "true"),
    ///         Equal("variables['_RunAsExternal']", "true")
    ///     ).Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if and(eq(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    ///     name: pool-B
    /// </code>
    /// <remarks>
    /// This method is useful when you want to specify the expressions as strings.
    /// In most cases, you should use the <see cref="And(IfCondition[])"/> method instead.
    /// </remarks>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>and</c> condition with the specified expressions.</returns>
    public IfCondition<T> And(params string[] expressions)
        => Link(new IfAndCondition<T>(expressions));

    /// <summary>
    /// Utility that appends an <c>or(expressions)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Or(
    ///         Equal("variables['_RunAsInternal']", "true"),
    ///         Equal("variables['_RunAsExternal']", "true")
    ///     ).Pool(new HostedPool("pool-B")),
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if and(or(variables['_RunAsInternal'], true), eq(variables['_RunAsExternal'], true)) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="expressions">Expressions to be combined with OR</param>
    /// <returns>An <c>or</c> condition with the specified expressions.</returns>
    public IfCondition<T> Or(params string[] expressions)
        => Link(new IfOrCondition<T>(expressions));

    /// <summary>
    /// Utility that appends an <c>startswith(needle, haystack)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.StartsWith("refs/heads/feature/", variables.Build.SourceBranch)
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if startsWith(variables['Build.SourceBranch'], 'refs/heads/feature/') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for at start</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>startsWith</c> condition with the specified expressions.</returns>
    public IfCondition<T> StartsWith(IfExpression needle, IfExpression haystack)
        => new IfStartsWithCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>endswith(needle, haystack)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.EndsWith("/feature/ui-mode", variables.Build.SourceBranch)
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if endsWith(variables['Build.SourceBranch'], '/feature/ui-mode') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for at end</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>endsWith</c> condition with the specified expressions.</returns>
    public IfCondition<T> EndsWith(IfExpression needle, IfExpression haystack)
        => new IfEndsWithCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>contains(needle, haystack)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Contains("feature", variables.Build.SourceBranch)
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if contains(variables['Build.SourceBranch'], 'feature') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>contains</c> condition with the specified expressions.</returns>
    public IfCondition<T> Contains(IfExpression needle, IfExpression haystack)
        => new IfContainsCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>in(needle, haystack)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.In(variables.Build.SourceBranch, "refs/heads/feature/", "refs/heads/main")
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if in(variables['Build.SourceBranch'], 'refs/heads/feature/', 'refs/heads/main') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>An <c>in</c> condition with the specified expressions.</returns>
    public IfCondition<T> In(IfExpression needle, IfExpression haystack)
        => new IfInCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>notIn(needle, haystack)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.NotIn(variables.Build.SourceBranch, "refs/heads/feature/", "refs/heads/main")
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if notIn(variables['Build.SourceBranch'], 'refs/heads/feature/', 'refs/heads/main') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>An <c>notIn</c> condition with the specified expressions.</returns>
    public IfCondition<T> NotIn(IfExpression needle, params IfExpression[] haystack)
        => new IfNotInCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>containsValue(needle, haystack)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.ContainsValue(variables.Build.SourceBranch, parameters["branchOptions"])
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ containsValue(parameters.branchOptions, variables['Build.SourceBranch']) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="needle">Needle to search for</param>
    /// <param name="haystack">Haystack to search in</param>
    /// <returns>A <c>containsValue</c> condition with the specified expressions.</returns>
    public IfCondition<T> ContainsValue(IfExpression needle, params IfExpression[] haystack)
        => new IfContainsValueCondition<T>(needle, haystack);

    /// <summary>
    /// Utility that appends an <c>eq(variables['Build.SourceBranch'], 'refs/heads/{branchName}')</c> condition to an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.IsBranch("main")
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="branchName">The name of the branch to check for.</param>
    /// <returns>An <c>eq</c> condition with the specified expressions.</returns>
    public IfCondition<T> IsBranch(IfExpression branchName)
        => Link(new IfBranchCondition<T>(branchName, true));

    /// <summary>
    /// Utility that appends an <c>ne(variables['Build.SourceBranch'], 'refs/heads/{branchName}')</c> condition to an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.IsNotBranch("main")
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if ne(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="branchName">The name of the branch to check for.</param>
    /// <returns>An <c>ne</c> condition with the specified expressions.</returns>
    public IfCondition<T> IsNotBranch(IfExpression branchName)
        => Link(new IfBranchCondition<T>(branchName, false));

    /// <summary>
    /// Utility that appends an <c>gt(expression1, expression2)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Greater(variables.Build.BuildNumber, "100")
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if gt(variables['Build.BuildNumber'], 100) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="first">First expression</param>
    /// <param name="second">Second expression</param>
    /// <returns>An <c>gt</c> condition with the specified expressions.</returns>
    public IfCondition<T> Greater(IfExpression first, IfExpression second)
        => Link(new IfGreaterCondition<T>(first, second));

    /// <summary>
    /// Utility that appends an <c>lt(expression1, expression2)</c> condition an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.Less(variables.Build.BuildNumber, "100")
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if lt(variables['Build.BuildNumber'], 100) }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    /// <param name="first">First expression</param>
    /// <param name="second">Second expression</param>
    /// <returns>An <c>lt</c> condition with the specified expressions.</returns>
    public IfCondition<T> Less(IfExpression first, IfExpression second)
        => Link(new IfLessCondition<T>(first, second));

    /// <summary>
    /// Utility that appends an <c>eq(variables['Build.Reason'], 'PullRequest')</c> condition to an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.IsPullRequest
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
    ///     name: pool-B
    /// </code>
    ///
    /// <code language="csharp">
    /// If.IsPullRequest
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
    /// </code>
    /// </summary>
    public IfCondition<T> IsPullRequest
        => Link(new IfBuildReasonCondition<T>("PullRequest", true));

    /// <summary>
    /// Utility that appends an <c>ne(variables['Build.Reason'], 'PullRequest')</c> condition to an <c>${{ if() }}</c> section that's connected to a specific definition type.
    /// For example:
    /// <code language="csharp">
    /// Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///     .EndIf
    ///     .If.IsNotPullRequest
    ///         .Pool(new HostedPool("pool-B"))
    /// </code>
    /// will generate:
    /// <code language="yaml">
    /// pool:
    ///   ${{ if eq('A', 'B') }}:
    ///     name: pool-A
    ///   ${{ if ne(variables['Build.Reason'], 'PullRequest') }}:
    ///     name: pool-B
    /// </code>
    /// </summary>
    public IfCondition<T> IsNotPullRequest
        => Link(new IfBuildReasonCondition<T>("PullRequest", false));

    private IfCondition<T> Link(IfCondition<T> condition)
    {
        condition.Parent = Parent;
        condition.IsElseIf = IsElseIf;
        
        // Eager materialization: Create conditional block and attach to parent immediately
        if (Parent != null)
        {
            // Check if we should merge conditions (when chaining If().If() with no items)
            if (Parent.Condition != null && Parent.Definitions.Count == 0 && !IsElseIf)
            {
                // Merge conditions with 'and()' - create new condition that combines both
                var existingCondition = Parent.Condition;
                var mergedCondition = new IfAndCondition<T>(existingCondition, condition);
                mergedCondition.Parent = Parent.Parent;
                mergedCondition.IsElseIf = IsElseIf;
                
                // Update the parent's condition to the merged one
                Parent.Condition = mergedCondition;
                
                // Return the merged condition but attach it to the same parent for chaining
                mergedCondition.Parent = Parent;
                return mergedCondition;
            }
            else
            {
                // For eager materialization, we'll create the block structure immediately
                // but we still return the condition to maintain API compatibility
                // The chaining will be enabled by adding an If property to IfCondition<T>
                return condition;
            }
        }
        else
        {
            return condition;
        }
    }
}
