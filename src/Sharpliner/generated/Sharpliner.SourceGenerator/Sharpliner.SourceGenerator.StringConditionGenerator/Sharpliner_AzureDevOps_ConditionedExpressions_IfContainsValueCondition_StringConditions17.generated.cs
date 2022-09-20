using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfContainsValueCondition : IfStringCondition
    {
        internal IfContainsValueCondition(ParameterReference needle, IEnumerable<object> haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class IfContainsValueCondition : IfStringCondition
    {
        internal IfContainsValueCondition(ParameterReference[] needle, string[] haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class IfContainsValueCondition : IfStringCondition
    {
        internal IfContainsValueCondition(StaticVariableReference needle, IEnumerable<object> haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class IfContainsValueCondition : IfStringCondition
    {
        internal IfContainsValueCondition(StaticVariableReference[] needle, string[] haystack) : base("containsValue",haystack,needle)
        {
        }
    }
    public partial class IfContainsValueCondition : IfStringCondition
    {
        internal IfContainsValueCondition(string needle, IEnumerable<object> haystack) : base("containsValue",haystack,needle)
        {
        }
    }
}
