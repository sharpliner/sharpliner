using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DeploymentStrategySerializationTests
{
    [Fact]
    public void Rolling_Strategy_Test()
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

        string yaml = SharplinerSerializer.Serialize(strategy);
        yaml.Trim().Should().Be(
            """
            rolling:
              maxParallel: 4
              preDeploy:
                steps:
                - download: current
              deploy:
                pool:
                  name: MacOS-latest
                steps:
                - download: current
            """);
    }

    [Fact]
    public void Canary_Strategy_Test()
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

        string yaml = SharplinerSerializer.Serialize(strategy);
        yaml.Trim().Should().Be(
            """
            canary:
              increments:
              - 10
              - 1000
              - 25000
              routeTraffic:
                steps:
                - download: current
              postRouteTraffic:
                pool:
                  name: MacOS-latest
                steps:
                - download: current
              on:
                failure:
                  steps:
                  - bash: |-
                      echo 'failure!' && exit 1
            """);
    }
}
