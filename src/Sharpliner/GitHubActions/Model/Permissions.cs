using System;
using System.Collections.Generic;

namespace Sharpliner.GitHubActions;

// TODO (GitHub Actions): Made internal until we get to a more complete API
internal enum GitHubPermissionScope
{
    Actions,
    Checks,
    Contents,
    Deployments,
    Issues,
    Packages,
    PullRequests,
    RepositoryProjects,
    SecurityEvents,
    Statuses,
}

// TODO (GitHub Actions): Made internal until we get to a more complete API
internal enum GitHubPermission
{
    None,
    Read,
    Write
}

// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Permissions
{
    public HashSet<GitHubPermissionScope> Read { get; } = [];
    public HashSet<GitHubPermissionScope> Write { get; } = [];

    public Permissions All(GitHubPermission permission)
    {
        // loop and add, we are not worried because it is a hasset, we add
        // to the correct hashset or remove from both in the case of None
        foreach (GitHubPermissionScope scope in Enum.GetValues(typeof(GitHubPermissionScope)))
        {
            switch (permission)
            {
                case GitHubPermission.Read:
                    Read.Add(scope);
                    break;
                case GitHubPermission.Write:
                    Write.Add(scope);
                    break;
                default:
                    Read.Remove(scope);
                    Write.Remove(scope);
                    break;
            }
        }
        return this;
    }
}
