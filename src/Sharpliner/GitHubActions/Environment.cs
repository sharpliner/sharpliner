using System;

namespace Sharpliner.GitHubActions;

// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Environment
{
    public Environment(string name, string? url = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
        Url = url;
    }
    
    public string Name { get; init; }
    public string? Url { get; init; }
}
