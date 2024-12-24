using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DeploymentStrategySerializationTests
{
    [Fact]
    public Task Rolling_Strategy_Test()
    {
        var strategy = new RollingStrategy
        {
            MaxParallel = 4,
            PreDeploy =
            {
                Steps =
                {
                    new CurrentDownloadTask(),
                }
            },
            Deploy =
            {
                Pool = new HostedPool("MacOS-latest"),
                Steps =
                {
                    new CurrentDownloadTask(),
                }
            },
        };

        return Verify(SharplinerSerializer.Serialize(strategy));
    }

    [Fact]
    public Task Canary_Strategy_Test()
    {
        var strategy = new CanaryStrategy
        {
            Increments = { 10, 1000, 25000 },
            RouteTraffic =
            {
                Steps =
                {
                    new CurrentDownloadTask(),
                }
            },
            PostRouteTraffic =
            {
                Pool = new HostedPool("MacOS-latest"),
                Steps =
                {
                    new CurrentDownloadTask(),
                }
            },
            OnFailure =
            {
                Steps =
                {
                    new InlineBashTask("echo 'failure!' && exit 1")
                }
            }
        };

        return Verify(SharplinerSerializer.Serialize(strategy));
    }
}
