namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// This task verifies that you didn't forget to check in your YAML pipeline changes.
    /// </summary>
    public record SharplinerValidateTask : InlineBashTask
    {
        /// <summary>
        /// This task verifies that you didn't forget to check in your YAML pipeline changes.
        /// </summary>
        /// <param name="pipelineProject">Path to the .csproj where pipelines are defined</param>
        /// <param name="isPosix">True for bash, false for PowerShell (based on OS)</param>
        public SharplinerValidateTask(string pipelineProject, bool isPosix, string displayName = "Validate YAML is published")
            : base(displayName, ValidationScript(pipelineProject, isPosix))
        {
        }

        private static string[] ValidationScript(string pipelineProject, bool isPosix) => isPosix
            ? new[]
            {
                $"dotnet build {pipelineProject}",
                "$results = git status --porcelain | Select-String -Pattern \"\\.ya?ml$\"",
                "if ($null -eq $results) {",
                "    Write-Host 'No YAML changes needed'",
                "    exit 0",
                "} else {",
                $"    Write-Host 'Please rebuild {pipelineProject} and commit the YAML changes'",
                "    exit 1",
                "}",
            }
            : new[]
            {
                $"dotnet build {pipelineProject}",
                "if [[ `git status --porcelain | grep -i '.ya\\?ml$'` ]]; then",
                $"    echo 'Please rebuild {pipelineProject} and commit the YAML changes'",
                "    exit 1",
                "else",
                "    echo 'No YAML changes needed'",
                "    exit 0",
                "fi",
            };
    }
}
