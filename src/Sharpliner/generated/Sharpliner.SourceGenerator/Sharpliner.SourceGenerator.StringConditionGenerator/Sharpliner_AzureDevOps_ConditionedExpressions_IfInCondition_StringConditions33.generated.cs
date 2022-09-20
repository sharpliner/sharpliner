using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfInCondition<T> : IfStringCondition<T>
    {
        internal IfInCondition(ParameterReference needle, IEnumerable<object> haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class IfInCondition<T> : IfStringCondition<T>
    {
        internal IfInCondition(ParameterReference[] needle, string[] haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class IfInCondition<T> : IfStringCondition<T>
    {
        internal IfInCondition(StaticVariableReference needle, IEnumerable<object> haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class IfInCondition<T> : IfStringCondition<T>
    {
        internal IfInCondition(StaticVariableReference[] needle, string[] haystack) : base("in",needle,haystack)
        {
        }
    }
    public partial class IfInCondition<T> : IfStringCondition<T>
    {
        internal IfInCondition(string needle, IEnumerable<object> haystack) : base("in",needle,haystack)
        {
        }
    }
}
