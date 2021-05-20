using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// This task verifies that you didn't forget to check in your YAML pipeline changes.
    /// </summary>
    public record SharplinerValidateTask : Step, IYamlConvertible
    {
        private readonly string _pipelineProject;
        private readonly bool _isPosix;

        /// <summary>
        /// This task verifies that you didn't forget to check in your YAML pipeline changes.
        /// </summary>
        /// <param name="pipelineProject">Path to the .csproj where pipelines are defined</param>
        /// <param name="isPosix">True for bash, false for Powershell (based on OS)</param>
        public SharplinerValidateTask(string pipelineProject, bool isPosix, string displayName = "Validate YAML is published")
            : base(displayName)
        {
            if (string.IsNullOrEmpty(pipelineProject))
            {
                throw new ArgumentException($"'{nameof(pipelineProject)}' cannot be null or empty.", nameof(pipelineProject));
            }

            _pipelineProject = pipelineProject;
            _isPosix = isPosix;
        }

        private string GetValidationScript() => string.Join(
            "\n",
            _isPosix
            ? new[]
            {
                $"dotnet build \"{_pipelineProject}\"",
                "if [[ `git status --porcelain | grep -i '.ya\\?ml$'` ]]; then",
                $"    echo 'Please rebuild {_pipelineProject} locally and commit the YAML changes' 1>&2",
                "    exit 1",
                "else",
                "    echo 'Check succeeded - no YAML changes needed'",
                "    exit 0",
                "fi",
            }
            : new[]
            {
                $"dotnet build \"{_pipelineProject}\"",
                "$results = Invoke-Expression \"git status --porcelain\" | Select-String -Pattern \"\\.ya?ml$\"",
                "if (!$results) {",
                "    Write-Host 'Check succeeded - no YAML changes needed'",
                "    exit 0",
                "} else {",
                $"    Write-Error 'Please rebuild {_pipelineProject} locally and commit the YAML changes'",
                "    exit 1",
                "}",
            });

        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar(_isPosix ? "bash" : "powershell"));
            emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, GetValidationScript(), ScalarStyle.Literal, true, false));
            emitter.Emit(new Scalar("displayName"));
            emitter.Emit(new Scalar(DisplayName));
            emitter.Emit(new MappingEnd());
        }
    }
}
