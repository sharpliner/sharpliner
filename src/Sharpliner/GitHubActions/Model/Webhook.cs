using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sharpliner.GitHubActions;

/// <summary>
/// Base abstract class for all webhook related triggers.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal abstract record Webhook : Event;

// All the following enum types look verbose.. but we are
// looking into the future with https://github.com/dotnet/csharplang/issues/2926
// ideally, all these will look as nice as:
//
// var cr = new CheckRun {
//     Activities = { Created, Requested }
// }
//
// we will have to wait until c# does implement the feature, so for the time
// being we will do:
//
// var cr = new CheckRun {
//     Activities = { CheckRun.Activity.Created, CheckRun.Activity.Requested }
// }
//
// verbose, but type safe and autocomplete will help. If not, get a better editor!

/// <summary>
/// Triggers the workflow for the 'check_run' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record CheckRun : Webhook
{
    public enum Activity
    {
        Created,
        Rerequested,
        Completed,
        RequestedAction,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'check_suite' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record CheckSuite : Webhook
{
    public enum Activity
    {
        Completed,
        Requested,
        Rerequested,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'create' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Create : Webhook;

/// <summary>
/// Triggers the workflow for the 'delete' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Delete : Webhook;

/// <summary>
/// Triggers the workflow for the 'deployment' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Deployment : Webhook;

/// <summary>
/// Triggers the workflow for the 'deployment_status' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record DeploymentStatus : Webhook;

/// <summary>
/// Triggers the workflow for the 'fork' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Fork : Webhook;

/// <summary>
/// Triggers the workflow for the 'gollum' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Gollum : Webhook;

/// <summary>
/// Triggers the workflow for the 'issue_comment' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record IssueComment : Webhook
{
    public enum Activity
    {
        Created,
        Edited,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'issues' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Issues : Webhook
{
    public enum Activity
    {
        Opened,
        Edited,
        Deleted,
        Transferred,
        Pinned,
        Unpinned,
        Closed,
        Reopened,
        Assigned,
        Unassigned,
        Labeled,
        Unlabeled,
        Locked,
        Unlocked,
        Milestoned,
        Demilestoned,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'label' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Label : Webhook
{
    public enum Activity
    {
        Created,
        Edited,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'milestone' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Milestone : Webhook
{
    public enum Activity
    {
        Created,
        Closed,
        Opened,
        Edited,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'page_build' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record PageBuild : Webhook { }

/// <summary>
/// Triggers the workflow for the 'project' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Project : Webhook
{
    public enum Activity
    {
        Created,
        Updated,
        Closed,
        Reopened,
        Edited,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'project_card' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record ProjectCard : Webhook
{
    public enum Activity
    {
        Created,
        Moved,
        ConvertedToAnIssue,
        Edited,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'project_column' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record ProjectColumn : Webhook
{
    public enum Activity
    {
        Created,
        Updated,
        Moved,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Public : Webhook { }

/// <summary>
/// Triggers the workflow for the 'pull_request' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record PullRequest : Webhook
{
    public enum Activity
    {
        Assigned,
        Unassigned,
        Labeled,
        Unlabeled,
        Opened,
        Edited,
        Closed,
        Reopened,
        Synchronize,
        ReadyForReview,
        Locked,
        Unlocked,
        ReviewRequested,
        ReviewRequestRemoved,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'pull_request_review' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record PullRequestReview : Webhook
{
    public enum Activity
    {
        Submitted,
        Edited,
        Dismissed,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'pull_request_review_comment' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record PullRequestReviewComment : Webhook
{
    public enum Activity
    {
        Created,
        Edited,
        Deleted,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'pull_request_target' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record PullRequestTarget : Webhook
{

    public enum Activity
    {
        Assigned,
        Unassigned,
        Labeled,
        Unlabeled,
        Opened,
        Edited,
        Closed,
        Reopened,
        Synchronize,
        ReadyForReview,
        Locked,
        Unlocked,
        ReviewRequested,
        ReviewRequestRemoved,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'push' event.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Push : Webhook;

/// <summary>
/// Triggers the workflow for the 'registry_package' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record RegistryPackage : Webhook
{

    public enum Activity
    {
        Published,
        Updated
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'release' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Release : Webhook
{
    public enum Activity
    {
        Published,
        Unpublished,
        Created,
        Edited,
        Deleted,
        Prereleased,
        Released,
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Status : Webhook { }

/// <summary>
/// Triggers the workflow for the 'watch' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Watch : Webhook
{
    public enum Activity
    {
        Started
    }

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}

/// <summary>
/// Triggers the workflow for the 'workflow_run' event. For than one activity can trigger the
/// event. By default all activities trigger the workflow. Use the Activities property to filter them.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record WorkflowRun : Webhook
{
    public enum Activity
    {
        Completed,
        Requested,
    }

    /// <summary>
    /// List with the workflows that will trigger the event.
    /// </summary>
    [Required]
    public List<string> Workflows { get; } = [];

    /// <summary>
    /// List with the branches that will trigger the event.
    /// </summary>
    [Required]
    public List<string> Branches { get; } = [];

    /// <summary>
    /// List of activities that will be considered when the event is triggered.
    /// </summary>
    public List<Activity> Activities { get; } = [];
}
