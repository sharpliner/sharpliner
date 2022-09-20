using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfNotInCondition : IfStringCondition
    {
        internal IfNotInCondition(ParameterReference needle, IEnumerable<object> haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition : IfStringCondition
    {
        internal IfNotInCondition(ParameterReference[] needle, string[] haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition : IfStringCondition
    {
        internal IfNotInCondition(StaticVariableReference needle, IEnumerable<object> haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition : IfStringCondition
    {
        internal IfNotInCondition(StaticVariableReference[] needle, string[] haystack) : base("notIn",needle,haystack)
        {
        }
    }
    public partial class IfNotInCondition : IfStringCondition
    {
        internal IfNotInCondition(string needle, IEnumerable<object> haystack) : base("notIn",needle,haystack)
        {
        }
    }
}
