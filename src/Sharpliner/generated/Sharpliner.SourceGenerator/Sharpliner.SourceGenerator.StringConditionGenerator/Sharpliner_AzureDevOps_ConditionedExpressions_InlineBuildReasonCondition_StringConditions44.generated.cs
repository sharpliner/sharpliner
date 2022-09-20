using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineBuildReasonCondition : InlineEqualityCondition
    {
        internal InlineBuildReasonCondition(ParameterReference[] reason, bool equal) : base(equal,new StaticVariableReference("Build.Reason"),reason)
        {
        }
    }
    public partial class InlineBuildReasonCondition : InlineEqualityCondition
    {
        internal InlineBuildReasonCondition(VariableReference[] reason, bool equal) : base(equal,new StaticVariableReference("Build.Reason"),reason)
        {
        }
    }
}
