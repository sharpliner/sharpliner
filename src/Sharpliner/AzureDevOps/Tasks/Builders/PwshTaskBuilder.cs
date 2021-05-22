using System.Reflection;

namespace Sharpliner.AzureDevOps.Tasks
{
    public class PwshTaskBuilder : ScriptTaskBuilder
    {
        /// <summary>
        /// Creates a Powershell task where the contents come from an embedded resource.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly where the resource is located</typeparam>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="resourceFileName">Name of the resource file</param>
        public InlinePowershellTask FromResourceFile(string displayName, string resourceFileName)
            => new InlinePowershellTask(displayName, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task where the contents come from an embedded resource.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly where the resource is located</typeparam>
        /// <param name="resourceFileName">Name of the resource file</param>
        public InlinePowershellTask FromResourceFile(string resourceFileName)
            => new InlinePowershellTask(null, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="path">Path to the file</param>
        public InlinePowershellTask FromFile(string displayName, string path)
            => new InlinePowershellTask(displayName, System.IO.File.ReadAllText(path)) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public InlinePowershellTask FromFile(string path)
            => new InlinePowershellTask(null, System.IO.File.ReadAllText(path)) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task referencing a Powershell file (contents are not inlined in the YAML).
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="filePath">Path to the file</param>
        public PowershellFileTask File(string displayName, string filePath)
            => new PowershellFileTask(displayName, filePath) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task referencing a Powershell file (contents are not inlined in the YAML).
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public PowershellFileTask File(string filePath)
            => new PowershellFileTask(null!, filePath) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task with given contents.
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="scriptLines">Contents of the script</param>
        public InlinePowershellTask Inline(string displayName, params string[] scriptLines)
            => new InlinePowershellTask(displayName, scriptLines) with
            {
                Pwsh = true
            };

        /// <summary>
        /// Creates a Powershell task with given contents.
        /// </summary>
        /// <param name="scriptLines">Contents of the script</param>
        public InlinePowershellTask Inline(params string[] scriptLines)
            => new InlinePowershellTask(null!, scriptLines) with
            {
                Pwsh = true
            };

        internal PwshTaskBuilder()
        {
        }
    }
}
