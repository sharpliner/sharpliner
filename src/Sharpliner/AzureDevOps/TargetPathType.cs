namespace Sharpliner.AzureDevOps
{
    public enum TargetPathType
    {
        /// <summary>
        /// Relative to parent the parent directory that contains a .git directory
        /// </summary>
        RelativeToGitRoot,

        /// <summary>
        /// Relative to where you are invoking the compilation from
        /// </summary>
        RelativeToCurrentDir,

        /// <summary>
        /// Relative to the assembly where the pipeline is defined
        /// </summary>
        RelativeToAssembly,

        /// <summary>
        /// Absolute file system path
        /// </summary>
        Absolute,
    }
}
