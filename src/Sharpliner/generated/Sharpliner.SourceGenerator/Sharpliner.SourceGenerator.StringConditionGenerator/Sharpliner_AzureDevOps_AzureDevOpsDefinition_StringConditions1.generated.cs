using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps
{
    public abstract partial class AzureDevOpsDefinition 
    {
        public InlineCondition Equal<T>(ParameterReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(ParameterReference expression1, VariableReference expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(ParameterReference expression1, string expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(VariableReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(VariableReference expression1, VariableReference expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(VariableReference expression1, string expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(string expression1, ParameterReference expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition Equal<T>(string expression1, VariableReference expression2)
        => new InlineEqualityCondition<T>(true, expression1, expression2);
        
        public InlineCondition NotEqual<T>(ParameterReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(ParameterReference expression1, VariableReference expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(ParameterReference expression1, string expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(VariableReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(VariableReference expression1, VariableReference expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(VariableReference expression1, string expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(string expression1, ParameterReference expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition NotEqual<T>(string expression1, VariableReference expression2)
        => new InlineEqualityCondition<T>(false, expression1, expression2);
        
        public InlineCondition Contains<T>(ParameterReference needle, ParameterReference haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(ParameterReference needle, VariableReference haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(ParameterReference needle, string haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(VariableReference needle, ParameterReference haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(VariableReference needle, VariableReference haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(VariableReference needle, string haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(string needle, ParameterReference haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition Contains<T>(string needle, VariableReference haystack)
        => new InlineContainsCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(ParameterReference needle, ParameterReference haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(ParameterReference needle, VariableReference haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(ParameterReference needle, string haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(VariableReference needle, ParameterReference haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(VariableReference needle, VariableReference haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(VariableReference needle, string haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(string needle, ParameterReference haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition StartsWith<T>(string needle, VariableReference haystack)
        => new InlineStartsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(ParameterReference needle, ParameterReference haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(ParameterReference needle, VariableReference haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(ParameterReference needle, string haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(VariableReference needle, ParameterReference haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(VariableReference needle, VariableReference haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(VariableReference needle, string haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(string needle, ParameterReference haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition EndsWith<T>(string needle, VariableReference haystack)
        => new InlineEndsWithCondition<T>(needle, haystack);
        
        public InlineCondition ContainsValue<T>(ParameterReference needle, ParameterReference[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(ParameterReference needle, VariableReference[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(ParameterReference needle, string[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(ParameterReference needle, IEnumerable<object> haystack)
        => new InlineContainsValueCondition<T>(needle, haystack);
        
        public InlineCondition ContainsValue<T>(VariableReference needle, ParameterReference[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(VariableReference needle, VariableReference[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(VariableReference needle, string[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(VariableReference needle, IEnumerable<object> haystack)
        => new InlineContainsValueCondition<T>(needle, haystack);
        
        public InlineCondition ContainsValue<T>(string needle, ParameterReference[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(string needle, VariableReference[] haystack)
        => new InlineContainsValueCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue<T>(string needle, IEnumerable<object> haystack)
        => new InlineContainsValueCondition<T>(needle, haystack);
        
        public InlineCondition In<T>(ParameterReference needle, ParameterReference[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(ParameterReference needle, VariableReference[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(ParameterReference needle, string[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(ParameterReference needle, IEnumerable<object> haystack)
        => new InlineInCondition<T>(needle, haystack);
        
        public InlineCondition In<T>(VariableReference needle, ParameterReference[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(VariableReference needle, VariableReference[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(VariableReference needle, string[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(VariableReference needle, IEnumerable<object> haystack)
        => new InlineInCondition<T>(needle, haystack);
        
        public InlineCondition In<T>(string needle, ParameterReference[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(string needle, VariableReference[] haystack)
        => new InlineInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition In<T>(string needle, IEnumerable<object> haystack)
        => new InlineInCondition<T>(needle, haystack);
        
        public InlineCondition NotIn<T>(ParameterReference needle, ParameterReference[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(ParameterReference needle, VariableReference[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(ParameterReference needle, string[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(ParameterReference needle, IEnumerable<object> haystack)
        => new InlineNotInCondition<T>(needle, haystack);
        
        public InlineCondition NotIn<T>(VariableReference needle, ParameterReference[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(VariableReference needle, VariableReference[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(VariableReference needle, string[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(VariableReference needle, IEnumerable<object> haystack)
        => new InlineNotInCondition<T>(needle, haystack);
        
        public InlineCondition NotIn<T>(string needle, ParameterReference[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(string needle, VariableReference[] haystack)
        => new InlineNotInCondition<T>(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn<T>(string needle, IEnumerable<object> haystack)
        => new InlineNotInCondition<T>(needle, haystack);
        
        public InlineCondition Greater<T>(ParameterReference expression1, ParameterReference expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(ParameterReference expression1, VariableReference expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(ParameterReference expression1, string expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(VariableReference expression1, ParameterReference expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(VariableReference expression1, VariableReference expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(VariableReference expression1, string expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(string expression1, ParameterReference expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Greater<T>(string expression1, VariableReference expression2)
        => new InlineGreaterCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(ParameterReference expression1, ParameterReference expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(ParameterReference expression1, VariableReference expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(ParameterReference expression1, string expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(VariableReference expression1, ParameterReference expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(VariableReference expression1, VariableReference expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(VariableReference expression1, string expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(string expression1, ParameterReference expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Less<T>(string expression1, VariableReference expression2)
        => new InlineLessCondition<T>(expression1, expression2);
        
        public InlineCondition Contains(ParameterReference needle, ParameterReference haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(ParameterReference needle, VariableReference haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(ParameterReference needle, string haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(VariableReference needle, ParameterReference haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(VariableReference needle, VariableReference haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(VariableReference needle, string haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(string needle, ParameterReference haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition Contains(string needle, VariableReference haystack)
        => new InlineContainsCondition(needle, haystack);
        
        public InlineCondition StartsWith(ParameterReference needle, ParameterReference haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(ParameterReference needle, VariableReference haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(ParameterReference needle, string haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(VariableReference needle, ParameterReference haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(VariableReference needle, VariableReference haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(VariableReference needle, string haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(string needle, ParameterReference haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition StartsWith(string needle, VariableReference haystack)
        => new InlineStartsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(ParameterReference needle, ParameterReference haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(ParameterReference needle, VariableReference haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(ParameterReference needle, string haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(VariableReference needle, ParameterReference haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(VariableReference needle, VariableReference haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(VariableReference needle, string haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(string needle, ParameterReference haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition EndsWith(string needle, VariableReference haystack)
        => new InlineEndsWithCondition(needle, haystack);
        
        public InlineCondition In(ParameterReference needle, ParameterReference[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(ParameterReference needle, VariableReference[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(ParameterReference needle, string[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(ParameterReference needle, IEnumerable<object> haystack)
        => new InlineInCondition(needle, haystack);
        
        public InlineCondition In(VariableReference needle, ParameterReference[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(VariableReference needle, VariableReference[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(VariableReference needle, string[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(VariableReference needle, IEnumerable<object> haystack)
        => new InlineInCondition(needle, haystack);
        
        public InlineCondition In(string needle, ParameterReference[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(string needle, VariableReference[] haystack)
        => new InlineInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition In(string needle, IEnumerable<object> haystack)
        => new InlineInCondition(needle, haystack);
        
        public InlineCondition NotIn(ParameterReference needle, ParameterReference[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(ParameterReference needle, VariableReference[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(ParameterReference needle, string[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(ParameterReference needle, IEnumerable<object> haystack)
        => new InlineNotInCondition(needle, haystack);
        
        public InlineCondition NotIn(VariableReference needle, ParameterReference[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(VariableReference needle, VariableReference[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(VariableReference needle, string[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(VariableReference needle, IEnumerable<object> haystack)
        => new InlineNotInCondition(needle, haystack);
        
        public InlineCondition NotIn(string needle, ParameterReference[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(string needle, VariableReference[] haystack)
        => new InlineNotInCondition(needle, haystack.Cast<object>());
        
        public InlineCondition NotIn(string needle, IEnumerable<object> haystack)
        => new InlineNotInCondition(needle, haystack);
        
        public InlineCondition ContainsValue(ParameterReference needle, ParameterReference[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(ParameterReference needle, VariableReference[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(ParameterReference needle, string[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(ParameterReference needle, IEnumerable<object> haystack)
        => new InlineContainsValueCondition(needle, haystack);
        
        public InlineCondition ContainsValue(VariableReference needle, ParameterReference[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(VariableReference needle, VariableReference[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(VariableReference needle, string[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(VariableReference needle, IEnumerable<object> haystack)
        => new InlineContainsValueCondition(needle, haystack);
        
        public InlineCondition ContainsValue(string needle, ParameterReference[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(string needle, VariableReference[] haystack)
        => new InlineContainsValueCondition(needle, haystack.Cast<object>());
        
        public InlineCondition ContainsValue(string needle, IEnumerable<object> haystack)
        => new InlineContainsValueCondition(needle, haystack);
        
        public InlineCondition Equal(ParameterReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(ParameterReference expression1, VariableReference expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(ParameterReference expression1, string expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(VariableReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(VariableReference expression1, VariableReference expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(VariableReference expression1, string expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(string expression1, ParameterReference expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition Equal(string expression1, VariableReference expression2)
        => new InlineEqualityCondition(true, expression1, expression2);
        
        public InlineCondition NotEqual(ParameterReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(ParameterReference expression1, VariableReference expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(ParameterReference expression1, string expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(VariableReference expression1, ParameterReference expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(VariableReference expression1, VariableReference expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(VariableReference expression1, string expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(string expression1, ParameterReference expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition NotEqual(string expression1, VariableReference expression2)
        => new InlineEqualityCondition(false, expression1, expression2);
        
        public InlineCondition Greater(ParameterReference expression1, ParameterReference expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(ParameterReference expression1, VariableReference expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(ParameterReference expression1, string expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(VariableReference expression1, ParameterReference expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(VariableReference expression1, VariableReference expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(VariableReference expression1, string expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(string expression1, ParameterReference expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Greater(string expression1, VariableReference expression2)
        => new InlineGreaterCondition(expression1, expression2);
        
        public InlineCondition Less(ParameterReference expression1, ParameterReference expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(ParameterReference expression1, VariableReference expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(ParameterReference expression1, string expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(VariableReference expression1, ParameterReference expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(VariableReference expression1, VariableReference expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(VariableReference expression1, string expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(string expression1, ParameterReference expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public InlineCondition Less(string expression1, VariableReference expression2)
        => new InlineLessCondition(expression1, expression2);
        
        public Condition IsBranch(ParameterReference branchName)
        => new InlineBranchCondition(branchName, true);
        
        public Condition IsBranch(VariableReference branchName)
        => new InlineBranchCondition(branchName, true);
        
        public Condition IsNotBranch(ParameterReference branchName)
        => new InlineBranchCondition(branchName, false);
        
        public Condition IsNotBranch(VariableReference branchName)
        => new InlineBranchCondition(branchName, false);
        
    }
}
