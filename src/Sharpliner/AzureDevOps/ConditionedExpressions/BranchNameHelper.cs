namespace Sharpliner.AzureDevOps.ConditionedExpressions;

internal class BranchNameHelper
{
    public static string FormatBranchName(string branchName)
    {
        if (branchName.StartsWith("refs/heads/"))
        {
            return branchName;
        }

        return $"refs/heads/{branchName}";
    }
}
