using System;
using Sharpliner.SourceGenerator;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions
{
    public partial class IfBranchCondition<T> : IfEqualityCondition<T>
    {
        internal IfBranchCondition(ParameterReference[] branchName, bool equal) : base(equal,new StaticVariableReference("Build.SourceBranch"),branchName)
        {
        }
    }
    public partial class IfBranchCondition<T> : IfEqualityCondition<T>
    {
        internal IfBranchCondition(StaticVariableReference[] branchName, bool equal) : base(equal,new StaticVariableReference("Build.SourceBranch"),branchName)
        {
        }
    }
}
