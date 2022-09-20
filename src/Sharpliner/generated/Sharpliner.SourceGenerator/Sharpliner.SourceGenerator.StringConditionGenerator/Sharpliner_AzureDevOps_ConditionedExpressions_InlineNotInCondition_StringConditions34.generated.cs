using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineNotInCondition<T> : InlineStringCondition<T>
    {
        internal InlineNotInCondition(ParameterReference needle, IEnumerable<object> haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition<T> : InlineStringCondition<T>
    {
        internal InlineNotInCondition(ParameterReference[] needle, string[] haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition<T> : InlineStringCondition<T>
    {
        internal InlineNotInCondition(VariableReference needle, IEnumerable<object> haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition<T> : InlineStringCondition<T>
    {
        internal InlineNotInCondition(VariableReference[] needle, string[] haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition<T> : InlineStringCondition<T>
    {
        internal InlineNotInCondition(string needle, IEnumerable<object> haystack) : base("notin",needle,haystack)
        {
        }
    }
}
