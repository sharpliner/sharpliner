using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(ParameterReference[] needle, ParameterReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(ParameterReference[] needle, StaticVariableReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(ParameterReference[] needle, string[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(StaticVariableReference[] needle, ParameterReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(StaticVariableReference[] needle, StaticVariableReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(StaticVariableReference[] needle, string[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(string[] needle, ParameterReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition : IfStringCondition
    {
        internal IfContainsCondition(string[] needle, StaticVariableReference[] haystack) : base("contains",haystack,needle)
        {
        }
    }
}
