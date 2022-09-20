using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, ParameterReference[] expression1, ParameterReference[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, ParameterReference[] expression1, VariableReference[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, ParameterReference[] expression1, string[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, VariableReference[] expression1, ParameterReference[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, VariableReference[] expression1, VariableReference[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, VariableReference[] expression1, string[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, string[] expression1, ParameterReference[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
    public partial class InlineEqualityCondition : InlineStringCondition
    {
        internal InlineEqualityCondition(bool equal, string[] expression1, VariableReference[] expression2) : base(equal ? "eq" : "ne",expression1,expression2)
        {
        }
    }
}
