using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfConditionBuilder 
    {
        public IfCondition Equal(ParameterReference expression1, ParameterReference expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(ParameterReference expression1, StaticVariableReference expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(ParameterReference expression1, string expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(StaticVariableReference expression1, ParameterReference expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(StaticVariableReference expression1, StaticVariableReference expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(StaticVariableReference expression1, string expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(string expression1, ParameterReference expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition Equal(string expression1, StaticVariableReference expression2)
        => Link(new IfEqualityCondition(true, expression1, expression2));
        
        public IfCondition NotEqual(ParameterReference expression1, ParameterReference expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(ParameterReference expression1, StaticVariableReference expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(ParameterReference expression1, string expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(StaticVariableReference expression1, ParameterReference expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(StaticVariableReference expression1, StaticVariableReference expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(StaticVariableReference expression1, string expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(string expression1, ParameterReference expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition NotEqual(string expression1, StaticVariableReference expression2)
        => Link(new IfEqualityCondition(false, expression1, expression2));
        
        public IfCondition StartsWith(ParameterReference needle, ParameterReference haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(ParameterReference needle, StaticVariableReference haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(ParameterReference needle, string haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(StaticVariableReference needle, ParameterReference haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(StaticVariableReference needle, StaticVariableReference haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(StaticVariableReference needle, string haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(string needle, ParameterReference haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition StartsWith(string needle, StaticVariableReference haystack)
        => new IfStartsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(ParameterReference needle, ParameterReference haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(ParameterReference needle, StaticVariableReference haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(ParameterReference needle, string haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(StaticVariableReference needle, ParameterReference haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(StaticVariableReference needle, StaticVariableReference haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(StaticVariableReference needle, string haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(string needle, ParameterReference haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition EndsWith(string needle, StaticVariableReference haystack)
        => new IfEndsWithCondition(needle, haystack);
        
        public IfCondition Contains(ParameterReference needle, ParameterReference haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(ParameterReference needle, StaticVariableReference haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(ParameterReference needle, string haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(StaticVariableReference needle, ParameterReference haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(StaticVariableReference needle, StaticVariableReference haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(StaticVariableReference needle, string haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(string needle, ParameterReference haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition Contains(string needle, StaticVariableReference haystack)
        => new IfContainsCondition(needle, haystack);
        
        public IfCondition ContainsValue(ParameterReference needle, ParameterReference[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(ParameterReference needle, StaticVariableReference[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(ParameterReference needle, string[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(ParameterReference needle, IEnumerable<object> haystack)
        => new IfContainsValueCondition(needle, haystack);
        
        public IfCondition ContainsValue(StaticVariableReference needle, ParameterReference[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(StaticVariableReference needle, StaticVariableReference[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(StaticVariableReference needle, string[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(StaticVariableReference needle, IEnumerable<object> haystack)
        => new IfContainsValueCondition(needle, haystack);
        
        public IfCondition ContainsValue(string needle, ParameterReference[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(string needle, StaticVariableReference[] haystack)
        => new IfContainsValueCondition(needle, haystack.Cast<object>());
        
        public IfCondition ContainsValue(string needle, IEnumerable<object> haystack)
        => new IfContainsValueCondition(needle, haystack);
        
        public IfCondition In(ParameterReference needle, ParameterReference[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(ParameterReference needle, StaticVariableReference[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(ParameterReference needle, string[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(ParameterReference needle, IEnumerable<object> haystack)
        => new IfInCondition(needle, haystack);
        
        public IfCondition In(StaticVariableReference needle, ParameterReference[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(StaticVariableReference needle, StaticVariableReference[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(StaticVariableReference needle, string[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(StaticVariableReference needle, IEnumerable<object> haystack)
        => new IfInCondition(needle, haystack);
        
        public IfCondition In(string needle, ParameterReference[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(string needle, StaticVariableReference[] haystack)
        => new IfInCondition(needle, haystack.Cast<object>());
        
        public IfCondition In(string needle, IEnumerable<object> haystack)
        => new IfInCondition(needle, haystack);
        
        public IfCondition NotIn(ParameterReference needle, ParameterReference[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(ParameterReference needle, StaticVariableReference[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(ParameterReference needle, string[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(ParameterReference needle, IEnumerable<object> haystack)
        => new IfNotInCondition(needle, haystack);
        
        public IfCondition NotIn(StaticVariableReference needle, ParameterReference[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(StaticVariableReference needle, StaticVariableReference[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(StaticVariableReference needle, string[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(StaticVariableReference needle, IEnumerable<object> haystack)
        => new IfNotInCondition(needle, haystack);
        
        public IfCondition NotIn(string needle, ParameterReference[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(string needle, StaticVariableReference[] haystack)
        => new IfNotInCondition(needle, haystack.Cast<object>());
        
        public IfCondition NotIn(string needle, IEnumerable<object> haystack)
        => new IfNotInCondition(needle, haystack);
        
        public IfCondition Greater(ParameterReference first, ParameterReference second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(ParameterReference first, StaticVariableReference second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(ParameterReference first, string second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(StaticVariableReference first, ParameterReference second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(StaticVariableReference first, StaticVariableReference second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(StaticVariableReference first, string second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(string first, ParameterReference second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Greater(string first, StaticVariableReference second)
        => Link(new IfGreaterCondition(first, second));
        
        public IfCondition Less(ParameterReference first, ParameterReference second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(ParameterReference first, StaticVariableReference second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(ParameterReference first, string second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(StaticVariableReference first, ParameterReference second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(StaticVariableReference first, StaticVariableReference second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(StaticVariableReference first, string second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(string first, ParameterReference second)
        => Link(new IfLessCondition(first, second));
        
        public IfCondition Less(string first, StaticVariableReference second)
        => Link(new IfLessCondition(first, second));
        
        public Condition IsBranch(ParameterReference branchName)
        => Link(new IfBranchCondition(branchName, true));
        
        public Condition IsBranch(StaticVariableReference branchName)
        => Link(new IfBranchCondition(branchName, true));
        
        public Condition IsNotBranch(ParameterReference branchName)
        => Link(new IfBranchCondition(branchName, false));
        
        public Condition IsNotBranch(StaticVariableReference branchName)
        => Link(new IfBranchCondition(branchName, false));
        
    }
}
