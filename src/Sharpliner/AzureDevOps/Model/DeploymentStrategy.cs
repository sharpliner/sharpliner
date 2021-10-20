﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Deployment strategy for the deployment job.
/// 
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/deployment-jobs?view=azure-devops">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record DeploymentStrategy : IYamlConvertible
{
    private readonly string _type;

    /// <summary>
    /// Used to run steps that initialize resources before application deployment starts.
    /// </summary>
    [YamlMember(Order = 100)]
    public LifeCycleHook PreDeploy { get; init; } = new();

    /// <summary>
    /// Used to run steps that deploy your application.
    ///
    /// Download artifact task will be auto injected only in the deploy hook for deployment jobs.
    /// To stop downloading artifacts, use - download: none or choose specific artifacts to download by specifying the download task.
    /// </summary>
    [YamlMember(Order = 200)]
    public LifeCycleHook Deploy { get; init; } = new();

    /// <summary>
    /// Used to run steps that serve the traffic to the updated version.
    /// </summary>
    [YamlMember(Order = 300)]
    public LifeCycleHook RouteTraffic { get; init; } = new();

    /// <summary>
    /// Used to run the steps after the traffic is routed. Typically, these tasks monitor the health of the updated version for defined interval.
    /// </summary>
    [YamlMember(Order = 400)]
    public LifeCycleHook PostRouteTraffic { get; init; } = new();

    /// <summary>
    /// Used to run steps for rollback actions or clean-up.
    /// </summary>
    [YamlMember(Order = 500)]
    public LifeCycleHook OnSuccess { get; init; } = new();

    /// <summary>
    /// Used to run steps for rollback actions or clean-up.
    /// </summary>
    [YamlMember(Order = 600)]
    public LifeCycleHook OnFailure { get; init; } = new();

    protected DeploymentStrategy(string type)
    {
        _type = type;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new MappingStart());
        emitter.Emit(new Scalar(_type));
        emitter.Emit(new MappingStart());

        WriteCustomFields(emitter, nestedObjectSerializer);

        var properties = new (string, Func<DeploymentStrategy, LifeCycleHook>)[]
        {
                ("preDeploy", x => x.PreDeploy),
                ("deploy", x => x.Deploy),
                ("routeTraffic", x => x.RouteTraffic),
                ("postRouteTraffic", x => x.PostRouteTraffic),
        };

        foreach (var pair in properties)
        {
            string name = pair.Item1;
            LifeCycleHook value = pair.Item2(this);

            if (value.Steps.Any())
            {
                emitter.Emit(new Scalar(name));
                nestedObjectSerializer(value);
            }
        }

        if (OnFailure.Steps.Any() || OnSuccess.Steps.Any())
        {
            emitter.Emit(new Scalar("on"));
            emitter.Emit(new MappingStart());

            if (OnSuccess.Steps.Any())
            {
                emitter.Emit(new Scalar("success"));
                nestedObjectSerializer(OnSuccess);
            }

            if (OnFailure.Steps.Any())
            {
                emitter.Emit(new Scalar("failure"));
                nestedObjectSerializer(OnFailure);
            }

            emitter.Emit(new MappingEnd());
        }

        emitter.Emit(new MappingEnd());
        emitter.Emit(new MappingEnd());
    }

    protected abstract void WriteCustomFields(IEmitter emitter, ObjectSerializer nestedObjectSerializer);
}

/// <summary>
/// runOnce is the simplest deployment strategy wherein all the lifecycle hooks, namely preDeploy deploy, routeTraffic, and postRouteTraffic, are executed once.
/// Then, either on: success or on: failure is executed.
/// </summary>
public record RunOnceStrategy : DeploymentStrategy
{
    public RunOnceStrategy() : base("runOnce")
    {
    }

    protected override void WriteCustomFields(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
    }
}

/// <summary>
/// A rolling deployment replaces instances of the previous version of an application with instances of the new version of the application on a fixed set of virtual machines (rolling set) in each iteration.
/// We currently only support the rolling strategy to VM resources.
/// For example, a rolling deployment typically waits for deployments on each set of virtual machines to complete before proceeding to the next set of deployments.
/// You could do a health check after each iteration and if a significant issue occurs, the rolling deployment can be stopped.
/// Rolling deployments can be configured by specifying the keyword rolling: under the strategy: node.
/// The strategy.name variable is available in this strategy block, which takes the name of the strategy. In this case, rolling.
/// </summary>
public record RollingStrategy : DeploymentStrategy
{
    /// <summary>
    /// With maxParallel: # or % of VMs - you can control the number/percentage of virtual machine targets to deploy to in parallel.
    /// This ensures that the app is running on these machines and is capable of handling requests while the deployment is taking place on the rest of the machines, which reduces overall downtime.
    /// </summary>
    [YamlMember(Order = 50, DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
    [DefaultValue(0)]
    public int MaxParallel { get; init; } = 0;

    public RollingStrategy() : base("rolling")
    {
    }

    protected override void WriteCustomFields(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {

        if (MaxParallel != 0)
        {
            emitter.Emit(new Scalar("maxParallel"));
            emitter.Emit(new Scalar(MaxParallel.ToString()));
        }
    }
}

/// <summary>
/// Canary deployment strategy is an advanced deployment strategy that helps mitigate the risk involved in rolling out new versions of applications.
/// By using this strategy, you can roll out the changes to a small subset of servers first.
/// As you gain more confidence in the new version, you can release it to more servers in your infrastructure and route more traffic to it.
/// </summary>
public record CanaryStrategy : DeploymentStrategy
{
    /// <summary>
    /// The increment value used in the current interaction.
    /// This variable is available only in deploy, routeTraffic, and postRouteTraffic lifecycle hooks.
    /// </summary>
    [YamlMember(Order = 50)]
    public List<int> Increments { get; init; } = new();

    public CanaryStrategy() : base("canary")
    {
    }

    protected override void WriteCustomFields(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Increments.Any())
        {
            emitter.Emit(new Scalar("increments"));
            nestedObjectSerializer(Increments);
        }
    }
}
