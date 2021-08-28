namespace Sharpliner.AzureDevOps.Tasks
{
    public class DotNetTaskBuilder
    {
        internal DotNetTaskBuilder()
        {
        }

        public DotNetInstallBuilder Install => new();

        /// <summary>
        /// Creates the build command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="projects">Projects to build</param>
        /// <param name="includeNuGetOrg">Include nuget.org in package sources?</param>
        /// <param name="arguments">Additional arguments</param>
        // We return Step and not something more specific so that user cannot override Inputs
        public Step Build(string projects, bool includeNuGetOrg = false, string arguments = null!)
        {
            var inputs = new TaskInputs();

            if (includeNuGetOrg)
            {
                inputs.Add("includeNuGetOrg", "true");
            }

            return Command(DotNetCommand.Build, projects, arguments, inputs);
        }

        /// <summary>
        /// Creates the DotNetCoreCLI task.
        /// </summary>
        /// <param name="command">.NET command to call</param>
        /// <param name="projects">Projects to build</param>
        /// <param name="arguments">Additional arguments for the call</param>
        /// <param name="inputs">Additional arguments defined by the DotNetCoreCLI task</param>
        // We return Step and not something more specific so that user cannot override Inputs
        public Step Command(DotNetCommand command, string projects = null!, string arguments = null!, TaskInputs inputs = null!)
        {
            var orderedInputs = new TaskInputs()
            {
                { "command", command.ToString().ToLowerInvariant() }
            };

            if (projects != null)
            {
                orderedInputs.Add("projects", projects);
            }

            if (arguments != null)
            {
                orderedInputs.Add("arguments", arguments);
            }

            if (inputs != null)
            {
                foreach (var pair in inputs)
                {
                    orderedInputs[pair.Key] = pair.Value;
                }
            }

            return new DotNetCoreCliTask()
            {
                Inputs = orderedInputs
            };
        }

        /// <summary>
        /// Creates a custom command version of the DotNetCoreCLI task.
        /// </summary>
        /// <param name="command">.NET command to call</param>
        /// <param name="arguments">Additional arguments for the call</param>
        /// <param name="inputs">Additional arguments defined by the DotNetCoreCLI task</param>
        // We return Step and not something more specific so that user cannot override Inputs
        public Step Custom(string command, string arguments = null!, TaskInputs inputs = null!)
        {
            var orderedInputs = new TaskInputs()
            {
                { "command", "custom" },
                { "custom", command },
            };

            if (arguments != null)
            {
                orderedInputs.Add("arguments", arguments);
            }

            if (inputs != null)
            {
                foreach (var pair in inputs)
                {
                    orderedInputs[pair.Key] = pair.Value;
                }
            }

            return new DotNetCoreCliTask()
            {
                Inputs = orderedInputs
            };
        }

        public class DotNetInstallBuilder
        {
            internal DotNetInstallBuilder()
            {
            }

            /// <summary>
            /// Creates the `dotnet install` task for full .NET SDK.
            /// </summary>
            public UseDotNetTask Sdk(string version) => new(DotNetPackageType.Sdk, version);

            /// <summary>
            /// Creates the `dotnet install` task for .NET runtime only.
            /// </summary>
            public UseDotNetTask Runtime(string version) => new(DotNetPackageType.Runtime, version);
        }
    }

    public enum DotNetCommand
    {
        Build,
        Push,
        Pack,
        Publish,
        Restore,
        Run,
        Test,
    }
}
