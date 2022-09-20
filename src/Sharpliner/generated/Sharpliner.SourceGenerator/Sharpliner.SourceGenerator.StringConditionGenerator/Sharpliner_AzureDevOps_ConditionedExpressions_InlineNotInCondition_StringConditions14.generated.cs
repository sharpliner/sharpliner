using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineNotInCondition : InlineStringCondition
    {
        internal InlineNotInCondition(ParameterReference needle, IEnumerable<object> haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition : InlineStringCondition
    {
        internal InlineNotInCondition(ParameterReference[] needle, string[] haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition : InlineStringCondition
    {
        internal InlineNotInCondition(VariableReference needle, IEnumerable<object> haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition : InlineStringCondition
    {
        internal InlineNotInCondition(VariableReference[] needle, string[] haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class InlineNotInCondition : InlineStringCondition
    {
        internal InlineNotInCondition(string needle, IEnumerable<object> haystack) : base("notIn",needle,haystack)
        {
        }
    }
}
