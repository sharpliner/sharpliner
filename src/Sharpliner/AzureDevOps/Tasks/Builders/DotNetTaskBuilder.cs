namespace Sharpliner.AzureDevOps.Tasks
{
    public class DotNetTaskBuilder
    {
        internal DotNetTaskBuilder()
        {
        }

        // We return Step and not something more specific so that user cannot override Inputs
        public Step Install(DotNetPackageType packageType, string version) => new UseDotNetTask(packageType, version);

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
