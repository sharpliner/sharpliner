﻿using System.Reflection;

namespace Sharpliner.AzureDevOps.Tasks
{
    public class BashTaskBuilder : ScriptTaskBuilder
    {
        /// <summary>
        /// Creates a bash task where the contents come from an embedded resource.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly where the resource is located</typeparam>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="resourceFileName">Name of the resource file</param>
        public InlineBashTask FromResourceFile(string displayName, string resourceFileName)
            => new(displayName, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName));

        /// <summary>
        /// Creates a bash task where the contents come from an embedded resource.
        /// </summary>
        /// <typeparam name="TAssembly">A type located in the assembly where the resource is located</typeparam>
        /// <param name="resourceFileName">Name of the resource file</param>
        public InlineBashTask FromResourceFile(string resourceFileName)
            => new(null, GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName));

        /// <summary>
        /// Creates a bash task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="path">Path to the file</param>
        public InlineBashTask FromFile(string displayName, string path) => new(displayName, System.IO.File.ReadAllText(path));

        /// <summary>
        /// Creates a bash task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="path">Path to the file</param>
        public InlineBashTask FromFile(string path) => FromFile(null!, System.IO.File.ReadAllText(path));

        /// <summary>
        /// Creates a bash task referencing a bash file (contents are not inlined in the YAML).
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="filePath">Path to the file</param>
        public BashFileTask File(string displayName, string filePath) => new(displayName, filePath);

        /// <summary>
        /// Creates a bash task referencing a bash file (contents are not inlined in the YAML).
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        public BashFileTask File(string filePath) => File(null!, filePath);

        /// <summary>
        /// Creates a bash task with given contents.
        /// </summary>
        /// <param name="displayName">Name of the build step</param>
        /// <param name="scriptLines">Contents of the script</param>
        public InlineBashTask Inline(string displayName, params string[] scriptLines) => new(displayName, scriptLines);

        /// <summary>
        /// Creates a bash task with given contents.
        /// </summary>
        /// <param name="scriptLines">Contents of the script</param>
        public InlineBashTask Inline(params string[] scriptLines) => Inline(null!, scriptLines);

        internal BashTaskBuilder()
        {
        }
    }
}
