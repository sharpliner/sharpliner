using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class InlineBranchCondition : InlineEqualityCondition
    {
        internal InlineBranchCondition(ParameterReference[] branchName, bool equal) : base(equal,new StaticVariableReference("Build.SourceBranch"),branchName)
        {
        }
    }
    public partial class InlineBranchCondition : InlineEqualityCondition
    {
        internal InlineBranchCondition(VariableReference[] branchName, bool equal) : base(equal,new StaticVariableReference("Build.SourceBranch"),branchName)
        {
        }
    }
}
