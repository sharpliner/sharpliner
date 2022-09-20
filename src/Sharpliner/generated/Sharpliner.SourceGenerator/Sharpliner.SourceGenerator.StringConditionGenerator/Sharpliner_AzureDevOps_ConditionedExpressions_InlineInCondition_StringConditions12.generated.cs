using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineInCondition : InlineStringCondition
    {
        internal InlineInCondition(ParameterReference needle, IEnumerable<object> haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class InlineInCondition : InlineStringCondition
    {
        internal InlineInCondition(ParameterReference[] needle, string[] haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class InlineInCondition : InlineStringCondition
    {
        internal InlineInCondition(VariableReference needle, IEnumerable<object> haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class InlineInCondition : InlineStringCondition
    {
        internal InlineInCondition(VariableReference[] needle, string[] haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class InlineInCondition : InlineStringCondition
    {
        internal InlineInCondition(string needle, IEnumerable<object> haystack) : base("in",needle,haystack)
        {
        }
    }
}
