using System;
using Sharpliner.Common;

namespace Sharpliner.CI;

class Configuration : SharplinerConfiguration
{
    public override void Configure()
    {
        Serialization.UseElseExpression = true;
        Validations.DependsOnFields = ValidationSeverity.Error;
        Hooks.BeforePublish = (definition, path) =>
        {
            switch (definition)
            {
                case PullRequestPipeline:
                    Console.WriteLine(
                        @"     ____  ____            _            _ _            " + Environment.NewLine +
                        @"    |  _ \|  _ \     _ __ (_)_ __   ___| (_)_ __   ___ " + Environment.NewLine +
                        @"    | |_) | |_) |   | '_ \| | '_ \ / _ \ | | '_ \ / _ \" + Environment.NewLine +
                        @"    |  __/|  _ <    | |_) | | |_) |  __/ | | | | |  __/" + Environment.NewLine +
                        @"    |_|   |_| \_\   | .__/|_| .__/ \___|_|_|_| |_|\___|" + Environment.NewLine +
                        @"                    |_|     |_|");
                    break;

                case PublishPipeline:
                    Console.WriteLine(
                        @"     ____        _     _ _     _               _            _ _            " + Environment.NewLine +
                        @"    |  _ \ _   _| |__ | (_)___| |__      _ __ (_)_ __   ___| (_)_ __   ___ " + Environment.NewLine +
                        @"    | |_) | | | | '_ \| | / __| '_ \    | '_ \| | '_ \ / _ \ | | '_ \ / _ \" + Environment.NewLine +
                        @"    |  __/| |_| | |_) | | \__ \ | | |   | |_) | | |_) |  __/ | | | | |  __/" + Environment.NewLine +
                        @"    |_|    \__,_|_.__/|_|_|___/_| |_|   | .__/|_| .__/ \___|_|_|_| |_|\___|" + Environment.NewLine +
                        @"                                        |_|     |_|");
                    break;
            }

            switch (path)
            {
                case string s when s.Contains("install"):
                    Console.WriteLine(
                       @"     _______                   _       _       " + Environment.NewLine +
                       @"    |__   __|_ _ __ ___  _ __ | | __ _| |_ ___ " + Environment.NewLine +
                       @"       | |/ _ \ '_ ` _ \| '_ \| |/ _` | __/ _ \" + Environment.NewLine +
                       @"       | |  __/ | | | | | |_) | | (_| | ||  __/" + Environment.NewLine +
                       @"       |_|\___|_| |_| |_| .__/|_|\__,_|\__\___|" + Environment.NewLine +
                       @"                        |_|" + Environment.NewLine);
                    break;
                    
                case string s when s.Contains("20"):
                    Console.WriteLine(
                       @"     _______        _             _            _ _            " + Environment.NewLine +
                       @"    |__   __|_  ___| |_     _ __ (_)_ __   ___| (_)_ __   ___ " + Environment.NewLine +
                       @"       | |/ _ \/ __| __|   | '_ \| | '_ \ / _ \ | | '_ \ / _ \" + Environment.NewLine +
                       @"       | |  __/\__ \ |_    | |_) | | |_) |  __/ | | | | |  __/" + Environment.NewLine +
                       @"       |_|\___||___/\__|   | .__/|_| .__/ \___|_|_|_| |_|\___|" + Environment.NewLine +
                       @"                           |_|     |_|");
                    break;
            }
        };
    }
}
