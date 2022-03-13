using Sharpliner.Common;

namespace Sharpliner.CI;

class Configuration : SharplinerConfiguration
{
    public override void Configure()
    {
        Serialization.UseElseExpression = true;
        Validations.DependsOnFields = ValidationSeverity.Error;
    }
}
