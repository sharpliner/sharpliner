using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(ParameterReference[] first, ParameterReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(ParameterReference[] first, VariableReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(ParameterReference[] first, string[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(VariableReference[] first, ParameterReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(VariableReference[] first, VariableReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(VariableReference[] first, string[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(string[] first, ParameterReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class InlineLessCondition<T> : InlineStringCondition<T>
    {
        internal InlineLessCondition(string[] first, VariableReference[] second) : base("lt",first,second)
        {
        }
    }
}
