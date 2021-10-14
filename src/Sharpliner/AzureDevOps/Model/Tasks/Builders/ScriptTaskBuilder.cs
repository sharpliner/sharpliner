﻿using System.Reflection;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks
{
    public class ScriptTaskBuilder : TaskBuilderBase
    {
        /// <summary>
        /// Creates a script task where the contents come from an embedded resource.
        /// </summary>
        /// <param name="resourceFileName">Name of the resource file</param>
        /// <param name="displayName">Display name of the build step</param>
        public ScriptTask FromResourceFile(string resourceFileName, string? displayName = null)
            => new ScriptTask(GetResourceFile(Assembly.GetCallingAssembly()!, resourceFileName)) with
            {
                DisplayName = displayName!,
            };

        /// <summary>
        /// Creates a script task where the contents come from a file.
        /// The contents are inlined in the YAML as contrary to File method where the file name is just referenced.
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="displayName">Display name of the build step</param>
        public ScriptTask FromFile(string path, string? displayName = null)
            => new ScriptTask(System.IO.File.ReadAllText(path)) with
            {
                DisplayName = displayName!,
            };

        /// <summary>
        /// Creates a script task with given contents.
        /// </summary>
        /// <param name="scriptLines">Contents of the script</param>
        public ScriptTask Inline(params string[] scriptLines) => new(scriptLines);

        internal ScriptTaskBuilder()
        {
        }
    }
}
