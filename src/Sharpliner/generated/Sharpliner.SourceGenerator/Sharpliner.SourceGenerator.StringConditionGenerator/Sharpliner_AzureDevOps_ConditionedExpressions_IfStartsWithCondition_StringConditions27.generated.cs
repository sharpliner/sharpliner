using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(ParameterReference[] needle, ParameterReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(ParameterReference[] needle, StaticVariableReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(ParameterReference[] needle, string[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(StaticVariableReference[] needle, ParameterReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(StaticVariableReference[] needle, StaticVariableReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(StaticVariableReference[] needle, string[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(string[] needle, ParameterReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
    public partial class IfStartsWithCondition<T> : IfStringCondition<T>
    {
        internal IfStartsWithCondition(string[] needle, StaticVariableReference[] haystack) : base("startsWith",haystack,needle)
        {
        }
    }
}
