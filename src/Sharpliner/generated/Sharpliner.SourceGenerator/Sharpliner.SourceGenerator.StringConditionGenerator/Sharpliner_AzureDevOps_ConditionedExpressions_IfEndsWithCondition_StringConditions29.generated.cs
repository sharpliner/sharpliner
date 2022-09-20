using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(ParameterReference[] needle, ParameterReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(ParameterReference[] needle, StaticVariableReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(ParameterReference[] needle, string[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(StaticVariableReference[] needle, ParameterReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(StaticVariableReference[] needle, StaticVariableReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(StaticVariableReference[] needle, string[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(string[] needle, ParameterReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
    public partial class IfEndsWithCondition<T> : IfStringCondition<T>
    {
        internal IfEndsWithCondition(string[] needle, StaticVariableReference[] haystack) : base("endsWith",haystack,needle)
        {
        }
    }
}
