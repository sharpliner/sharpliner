using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineContainsValueCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsValueCondition(ParameterReference needle, IEnumerable<object> haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class InlineContainsValueCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsValueCondition(ParameterReference[] needle, string[] haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class InlineContainsValueCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsValueCondition(VariableReference needle, IEnumerable<object> haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class InlineContainsValueCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsValueCondition(VariableReference[] needle, string[] haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class InlineContainsValueCondition<T> : InlineStringCondition<T>
    {
        internal InlineContainsValueCondition(string needle, IEnumerable<object> haystack) : base("containsValue",haystack,needle)
        {
        }
    }
}
