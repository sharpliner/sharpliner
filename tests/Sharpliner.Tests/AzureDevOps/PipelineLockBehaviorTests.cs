﻿using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineLockBehaviorTests
{
    private class PipelineLockBehaviorTests_Pipeline : SimpleTestPipeline
    {
        public PipelineLockBehaviorTests_Pipeline(LockBehavior? lockBehavior)
        {
            Pipeline =  new()
            {
                Name = "LockBehaviorTest",
                LockBehavior = lockBehavior
            };
        }
        public override SingleStagePipeline Pipeline { get; }
    }

    [Theory]
    [InlineData(LockBehavior.Sequential, "sequential")]
    [InlineData(LockBehavior.RunLatest, "runLatest")]
    public void PipelineLockBehavior_Serialization_TestValues(LockBehavior lockBehaviour, string expectedSerializedValue)
    {
        var yaml = new PipelineLockBehaviorTests_Pipeline(lockBehaviour).Serialize();

        yaml.Trim().Should().Be(
            $"""
            name: LockBehaviorTest

            lockBehavior: {expectedSerializedValue}
            """);
    }

    [Fact]
    public void PipelineLockBehavior_Serialization_Test_Null()
    {
        var yaml = new PipelineLockBehaviorTests_Pipeline(null).Serialize();

        yaml.Trim().Should().Be(
            """
            name: LockBehaviorTest
            """);
    }
}
