using Sharpliner.Common;

namespace Sharpliner.CI;

class SharplinerConfiguration : Sharpliner.SharplinerConfiguration
{
    public override void Configure()
    {
        Serialization.UseElseExpression = true;
        Validations.DependsOn = ValidationSeverity.Error;
    }
}
