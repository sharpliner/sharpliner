using Sharpliner.Common;

namespace Sharpliner;

/// <summary>
/// Inherit from this class to configure the publishing process.
/// You can customize how YAML is serialized, which validations are run and with what severity and more.
/// </summary>
public abstract class SharplinerConfiguration
{
    public class SerializationSettings
    {
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

    public class ValidationsSettings
    {
        /// <summary>
        /// Validates whether stage and job names are valid.
        /// </summary>
        public ValidationSeverity Name { get; set; } = ValidationSeverity.Error;

        /// <summary>
        /// Validates whether stages and jobs do not dependent on each other and similar.
        /// </summary>
        public ValidationSeverity DependsOn { get; set; } = ValidationSeverity.Error;
    }

    /// <summary>
    /// Settings we can reach from within model classes.
    /// </summary>
    internal static SharplinerConfiguration Current { get; private set; } = new DefaultSharplinerConfiguration();

    /// <summary>
    /// Use this property to customize how YAML is serialized.
    /// </summary>
    public virtual SerializationSettings Serialization { get; } = new();

    /// <summary>
    /// Use this property to control which validations are run and with what severity.
    /// </summary>
    public virtual ValidationsSettings Validations { get; } = new();

    internal void ConfigureInternal()
    {
        Configure();
        Current = this;
    }

    public abstract void Configure();
}

internal class DefaultSharplinerConfiguration : SharplinerConfiguration
{
    public override void Configure() => throw new System.NotImplementedException();
}
