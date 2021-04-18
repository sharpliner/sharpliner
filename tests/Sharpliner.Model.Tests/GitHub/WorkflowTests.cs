using Sharpliner.Model.GHActions;
using Xunit;

namespace Sharpliner.Model.Tests.GitHub
{
    public class WorkflowTests
    {

        [Fact]
        public void Workflow_Single_Manual_Workflow()
        {
            var w = new Workflow
            {
                On =
                {
                    Manuals =
                    {
                        WorkflowDispatch = new () {
                            Inputs =
                            {
                               new ("name") {
                                    Description = "Person to greet",
                                    Default = "Mona the Octocat",
                                    IsRequired = true,
                               }
                            }
                        }
                    }
                }
            };

            Assert.NotNull(w);
            Assert.NotNull(w.On.Manuals.WorkflowDispatch);
            Assert.Null(w.On.Manuals.RepositoryDispatch);
            Assert.Empty(w.On.Schedules);
            Assert.Empty(w.On.Webhooks);
        }

        [Fact]
        public void Workflow_Manual_RepositoryDispatch()
        {
            var w = new Workflow
            {
                On =
                {
                   Manuals =
                   {
                       RepositoryDispatch = new () {
                           Activities = { "Opened", "MyActivity"}
                       }
                   }
                }
            };

            Assert.NotNull(w);
            Assert.Null(w.On.Manuals.WorkflowDispatch);
            Assert.NotNull(w.On.Manuals.RepositoryDispatch);
            Assert.Empty(w.On.Schedules);
            Assert.Empty(w.On.Webhooks);
        }

        [Fact]
        public void Workflow_Single_Schedule()
        {
            var w = new Workflow
            {
                On =
                {
                    Schedules =
                    {
                        new ("'*/30 5,17 * * *'")
                    }
                }
            };
            Assert.NotNull(w);
            Assert.Null(w.On.Manuals.WorkflowDispatch);
            Assert.Null(w.On.Manuals.RepositoryDispatch);
            Assert.Single(w.On.Schedules);
            Assert.Empty(w.On.Webhooks);
        }

        [Fact]
        public void Workflow_Several_Sechedule()
        {
            var w = new Workflow
            {
                On =
                {
                    Schedules =
                    {
                        new ("'*/30 5,17 * * *'"),
                        new ("'*/30 1,17 * * *'")
                    }
                }
            };
            Assert.NotNull(w);
            Assert.Null(w.On.Manuals.WorkflowDispatch);
            Assert.Null(w.On.Manuals.RepositoryDispatch);
            Assert.Equal(2, w.On.Schedules.Count);
            Assert.Empty(w.On.Webhooks);
        }

        [Fact]
        public void Workflow_Single_Webhook()
        {
            var w = new Workflow()
            {
                On =
                {
                    Webhooks =
                    {
                        new CheckRun
                        {
                            Activities =  { CheckRun.Activity.Completed, CheckRun.Activity.RequestedAction }
                        }
                    }
                }
            };
            Assert.NotNull(w);
            Assert.Null(w.On.Manuals.WorkflowDispatch);
            Assert.Null(w.On.Manuals.RepositoryDispatch);
            Assert.Empty(w.On.Schedules);
            Assert.Single(w.On.Webhooks);
        }

        [Fact]
        public void Workflow_Several_webhook()
        {
            var w = new Workflow
            {
                On =
                {
                    Webhooks =
                    {
                        new CheckRun
                        {
                            Activities =  { CheckRun.Activity.Completed, CheckRun.Activity.RequestedAction }
                        },
                        new PullRequest
                        {
                            Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                        }
                    }
                }
            };

            Assert.NotNull(w);
            Assert.Null(w.On.Manuals.WorkflowDispatch);
            Assert.Null(w.On.Manuals.RepositoryDispatch);
            Assert.Empty(w.On.Schedules);
            Assert.Equal(2, w.On.Webhooks.Count);
        }
    }
}
