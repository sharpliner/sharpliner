using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(ParameterReference[] haystack, ParameterReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(ParameterReference[] haystack, VariableReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(ParameterReference[] haystack, string[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(VariableReference[] haystack, ParameterReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(VariableReference[] haystack, VariableReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(VariableReference[] haystack, string[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(string[] haystack, ParameterReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsCondition(string[] haystack, VariableReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
}
