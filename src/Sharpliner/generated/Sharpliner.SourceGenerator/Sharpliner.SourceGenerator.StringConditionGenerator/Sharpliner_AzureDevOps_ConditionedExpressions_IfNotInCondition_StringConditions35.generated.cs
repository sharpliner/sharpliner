using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfNotInCondition<T> : IfStringCondition<T>
    {
        internal IfNotInCondition(ParameterReference needle, IEnumerable<object> haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition<T> : IfStringCondition<T>
    {
        internal IfNotInCondition(ParameterReference[] needle, string[] haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition<T> : IfStringCondition<T>
    {
        internal IfNotInCondition(StaticVariableReference needle, IEnumerable<object> haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition<T> : IfStringCondition<T>
    {
        internal IfNotInCondition(StaticVariableReference[] needle, string[] haystack) : base("notin",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition<T> : IfStringCondition<T>
    {
        internal IfNotInCondition(string needle, IEnumerable<object> haystack) : base("notin",needle,haystack)
        {
        }
    }
}
