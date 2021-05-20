namespace Sharpliner.AzureDevOps.Tasks
{
    public class PowershellTaskBuilder : ScriptTaskBuilder
    {
        /// <summary>
        /// Creates a Powershell task where the contents come from an embedded resource.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly where the resource is located</typeparam>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="resourceFileName">Name of the resource file</param>
        public InlinePowershellTask FromResourceFile<TAssembly>(string displayName, string resourceFileName)
            => new(displayName, GetResourceFile<TAssembly>(resourceFileName));

        /// <summary>
        /// Creates a Powershell task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="path">Path to the file</param>
        public InlinePowershellTask FromFile(string displayName, string path) => new(displayName, System.IO.File.ReadAllText(path));

        /// <summary>
        /// Creates a Powershell task referencing a Powershell file (contents are not inlined in the YAML).
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="filePath">Path to the file</param>
        public PowershellFileTask File(string displayName, string filePath) => new(displayName, filePath);

        /// <summary>
        /// Creates a Powershell task with given contents.
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="scriptLines">Contents of the script</param>
        public InlinePowershellTask Inline(string displayName, params string[] scriptLines) => new(displayName, scriptLines);
    }
}
