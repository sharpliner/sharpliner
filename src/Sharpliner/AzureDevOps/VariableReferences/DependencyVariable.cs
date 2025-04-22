using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpliner.AzureDevOps.VariableReferences;

/// <summary>
/// <para>
/// Container for all possible dependency yaml combinations.  This can be used to reach all variable references from other dependencies.  Specify whether you're attempting to use
/// the varaible within a <c>stage</c> or <c>job</c> entry, then specify if the dependency comes from a Deployment job or not using <c>deploy</c>.
/// </para>
/// </summary>
public class DependencyVariable
{
    /// <summary>
    /// <para>
    /// Allows the <c>;${{ stageDependencies.&lt;stage-name&gt;.outputs['&lt;job-name&gt;.&lt;step-name&gt;.&lt;variable-name&gt;'] }}</c> notation for dependency output variables.
    /// This is meant to be used in `conditions` section of a `stage` and only for variables that did not come from deployment jobs.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// stageDependencies["stage", "job", "step", "variable"]
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// ${{ stageDependencies.stage.outputs['job.step.variable'] }}
    /// </code>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Should not be capitalized to follow YAML syntax")]
    public readonly StageLevelDependencyVariable stage = new();

    /// <summary>
    /// <para>
    /// Allows either the <c>;${{ dependencies.&lt;job-name&gt;.outputs['&lt;step-name&gt;.&lt;variable-name&gt;'] }}</c> or <c>;${{ stageDependencies.&lt;stage-name&gt;.&lt;job-name&gt;.outputs['&lt;step-name&gt;.&lt;variable-name&gt;'] }}</c> notation for dependency output variables.
    /// This is meant for use within the `jobs` section of a pipeline.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// dependencies["stage", "job", "step", "variable"]
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// ${{ stageDependencies.stage.job.outputs['step.variable'] }}
    /// </code>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Should not be capitalized to follow YAML syntax")]
    public readonly JobLevelDependencyVariable job = new();
}
