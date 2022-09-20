using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(ParameterReference[] first, ParameterReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(ParameterReference[] first, StaticVariableReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(ParameterReference[] first, string[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(StaticVariableReference[] first, ParameterReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(StaticVariableReference[] first, StaticVariableReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(StaticVariableReference[] first, string[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(string[] first, ParameterReference[] second) : base("lt",first,second)
        {
        }
    }
    public partial class IfLessCondition : IfStringCondition
    {
        internal IfLessCondition(string[] first, StaticVariableReference[] second) : base("lt",first,second)
        {
        }
    }
}
