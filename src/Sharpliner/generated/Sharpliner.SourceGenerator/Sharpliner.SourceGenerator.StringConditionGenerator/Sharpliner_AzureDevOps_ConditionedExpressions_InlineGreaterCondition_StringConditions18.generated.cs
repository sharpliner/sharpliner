using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(ParameterReference[] first, ParameterReference[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(ParameterReference[] first, VariableReference[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(ParameterReference[] first, string[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(VariableReference[] first, ParameterReference[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(VariableReference[] first, VariableReference[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(VariableReference[] first, string[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(string[] first, ParameterReference[] second) : base("gt",first,second)
        {
        }
    }
    public partial class InlineGreaterCondition : InlineStringCondition
    {
        internal InlineGreaterCondition(string[] first, VariableReference[] second) : base("gt",first,second)
        {
        }
    }
}
