using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(ParameterReference[] needle, ParameterReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(ParameterReference[] needle, VariableReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(ParameterReference[] needle, string[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(VariableReference[] needle, ParameterReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(VariableReference[] needle, VariableReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(VariableReference[] needle, string[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(string[] needle, ParameterReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class InlineStartsWithCondition : InlineStringCondition
    {
        internal InlineStartsWithCondition(string[] needle, VariableReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
}
