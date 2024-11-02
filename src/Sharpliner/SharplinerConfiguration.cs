using Sharpliner.Common;

namespace Sharpliner;

/// <summary>
/// Interface with all required configuration options for the YAML publishing process.
/// </summary>
public interface ISharplinerConfiguration
{
    /// <summary>
    /// Settings around YAML serialization
    /// </summary>
    SharplinerConfiguration.SerializationSettings Serialization { get; }

    /// <summary>
    /// Configuration of which validations run and with what severity
    /// </summary>
    SharplinerConfiguration.ValidationsSettings Validations { get; }

    /// <summary>
    /// Hook into the publishing process
    /// </summary>
    SharplinerConfiguration.SerializationHooks Hooks { get; }
}

/// <summary>
/// Inherit from this class to configure the publishing process.
/// You can customize how YAML is serialized, which validations are run and with what severity and more.
/// </summary>
public abstract class SharplinerConfiguration : ISharplinerConfiguration
{
    /// <summary>
    /// Settings around YAML serialization
    /// </summary>
    public class SerializationSettings
    {
        /// <summary>
        /// When true (default), inserts documentation headers into generated YAML files.
        /// The content of the headers can be customized via the Headers field on your definitions.
        /// </summary>
        public bool IncludeHeaders { get; set; } = true;

        /// <summary>
        /// When true (default), makes the YAML a little bit more human-readable.
        /// For instance, new lines are added.
        /// </summary>
        public bool PrettifyYaml { get; set; } = true;

        /// <summary>
        /// Set to false if you prefer Else branch to contain negated if condition rather than ${{ else }}
        /// </summary>
        public bool UseElseExpression { get; set; } = true;
    }

    /// <summary>
    /// Configuration of which validations run and with what severity
    /// </summary>
    public class ValidationsSettings
    {
        /// <summary>
        /// Validates whether stage and job names are valid.
        /// </summary>
        public ValidationSeverity NameFields { get; set; } = ValidationSeverity.Error;

        /// <summary>
        /// Validates whether stages and jobs do not dependent on each other and similar.
        /// </summary>
        public ValidationSeverity DependsOnFields { get; set; } = ValidationSeverity.Warning;

        /// <summary>
        /// Validates whether checked out repositories are defined in resources.
        /// </summary>
        public ValidationSeverity RepositoryCheckouts { get; set; } = ValidationSeverity.Warning;
    }

    /// <summary>
    /// Hook into the publishing process
    /// </summary>
    public class SerializationHooks
    {
        /// <summary>
        /// Hook that gets called right the YAML is published.
        /// </summary>
        /// <param name="definition">Definition for which is the hook called (i.e. a pipeline)</param>
        /// <param name="destinationPath">Destination path of where the current definition is serialized to</param>
        public delegate void BeforePublishHandler(ISharplinerDefinition definition, string destinationPath);

        /// <summary>
        /// Hook that gets called after the YAML is published.
        /// </summary>
        /// <param name="definition">Definition for which is the hook called (i.e. a pipeline)</param>
        /// <param name="destinationPath">Destination path of where the current definition is serialized to</param>
        /// <param name="yaml">Serialized YAML of the generated definition</param>
        public delegate void AfterPublishHandler(ISharplinerDefinition definition, string destinationPath, string yaml);

        /// <summary>
        /// This hook gets called right before the YAML is published.
        /// Parameters passed are:
        ///   - The definition being published
        ///   - Destination path for the YAML file
        /// </summary>
        public BeforePublishHandler? BeforePublish { get; set; }

        /// <summary>
        /// This hook gets called right after the YAML is published.
        /// Parameters passed are:
        ///   - The definition being published
        ///   - Destination path for the YAML file
        ///   - The serialized YAML
        /// </summary>
        public AfterPublishHandler? AfterPublish { get; set; }
    }

    /// <summary>
    /// Current configuration we can reach from anywhere
    /// </summary>
    internal static ISharplinerConfiguration Current { get; private set; } = new DefaultSharplinerConfiguration();

    /// <summary>
    /// Use this property to customize how YAML is serialized
    /// </summary>
    public SerializationSettings Serialization { get; } = new();

    /// <summary>
    /// Use this property to control which validations are run and with what severity
    /// </summary>
    public ValidationsSettings Validations { get; } = new();

    /// <summary>
    /// Use this property to hook into the publishing process
    /// </summary>
    public SerializationHooks Hooks { get; } = new();

    internal void ConfigureInternal()
    {
        Configure();
        Current = this;
    }

    public abstract void Configure();
}

internal class DefaultSharplinerConfiguration : SharplinerConfiguration
{
    public override void Configure()
    {
    }
}
