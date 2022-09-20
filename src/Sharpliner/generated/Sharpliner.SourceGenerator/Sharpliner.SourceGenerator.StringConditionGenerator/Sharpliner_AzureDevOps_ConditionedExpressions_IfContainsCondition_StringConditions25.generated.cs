using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(ParameterReference[] haystack, ParameterReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(ParameterReference[] haystack, StaticVariableReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(ParameterReference[] haystack, string[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(StaticVariableReference[] haystack, ParameterReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(StaticVariableReference[] haystack, StaticVariableReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(StaticVariableReference[] haystack, string[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(string[] haystack, ParameterReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
    public partial class IfContainsCondition<T> : IfStringCondition<T>
    {
        internal IfContainsCondition(string[] haystack, StaticVariableReference[] needle) : base("contains",haystack,needle)
        {
        }
    }
}
