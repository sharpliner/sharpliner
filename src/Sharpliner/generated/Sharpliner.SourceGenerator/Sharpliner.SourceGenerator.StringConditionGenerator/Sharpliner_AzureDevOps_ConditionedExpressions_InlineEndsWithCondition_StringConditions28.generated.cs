using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(ParameterReference[] needle, ParameterReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(ParameterReference[] needle, VariableReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(ParameterReference[] needle, string[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(VariableReference[] needle, ParameterReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(VariableReference[] needle, VariableReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(VariableReference[] needle, string[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(string[] needle, ParameterReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class InlineEndsWithCondition<T> : InlineStringCondition<T>
    {
        internal InlineEndsWithCondition(string[] needle, VariableReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
}
