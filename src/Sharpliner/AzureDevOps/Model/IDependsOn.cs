﻿using System.Collections.Generic;

namespace Sharpliner.AzureDevOps
{
    public interface IDependsOn
    {
        string Name { get; }
        ConditionedList<string> DependsOn { get; }
    }
}
