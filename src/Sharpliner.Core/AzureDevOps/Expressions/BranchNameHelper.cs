namespace Sharpliner.AzureDevOps.Expressions;

internal class BranchNameHelper
{
    private const string RefsPrefix = "refs/heads/";

    public static string FormatBranchName(string branchName)
    {
        if (branchName.StartsWith(RefsPrefix))
        {
            return branchName;
        }

        return RefsPrefix + branchName;
    }
}
