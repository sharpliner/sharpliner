using System.Linq.Expressions;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public interface IRuntimeExpression
{
    string RuntimeExpression { get; }
}
