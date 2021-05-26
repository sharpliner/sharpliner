﻿using System.Reflection;

namespace Sharpliner.AzureDevOps.Tasks
{
    public class BashTaskBuilder : ScriptTaskBuilder
    {
        /// <summary>
        /// Creates a bash task where the contents come from an embedded resource.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly where the resource is located</typeparam>
        /// <param name="resourceFileName">Name of the resource file</param>
        public InlineBashTask FromResourceFile(string resourceFileName, string? displayName = null)
            => new InlineBashTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with { DisplayName = displayName! };

        /// <summary>
        /// Creates a bash task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public InlineBashTask FromFile(string path, string? displayName = null) => new InlineBashTask(System.IO.File.ReadAllText(path)) with { DisplayName = displayName! };

        /// <summary>
        /// Creates a bash task referencing a bash file (contents are not inlined in the YAML).
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="filePath">Path to the file</param>
        public BashFileTask File(string filePath, string? displayName = null) => new BashFileTask(filePath) with { DisplayName = displayName! };

        /// <summary>
        /// Creates a bash task with given contents.
        /// </summary>
        /// <param name="scriptLines">Contents of the script</param>
        public InlineBashTask Inline(params string[] scriptLines) => new(scriptLines);

        internal BashTaskBuilder()
        {
        }
    }
}
