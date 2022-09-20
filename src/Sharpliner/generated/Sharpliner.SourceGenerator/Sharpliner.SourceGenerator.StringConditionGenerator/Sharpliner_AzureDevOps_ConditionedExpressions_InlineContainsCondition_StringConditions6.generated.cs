using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(ParameterReference[] needle, ParameterReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(ParameterReference[] needle, VariableReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(ParameterReference[] needle, string[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(VariableReference[] needle, ParameterReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(VariableReference[] needle, VariableReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(VariableReference[] needle, string[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(string[] needle, ParameterReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class InlineContainsCondition : InlineStringCondition
    {
        internal InlineContainsCondition(string[] needle, VariableReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
}
