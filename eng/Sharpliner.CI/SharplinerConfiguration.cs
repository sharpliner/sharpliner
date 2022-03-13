namespace Sharpliner.CI;

class SharplinerConfiguration : Sharpliner.SharplinerConfiguration
{
    public override void Configure()
    {
        Serialization.UseElseExpression = true;
        Validations.DependsOn = Common.ValidationSeverity.Error;
    }
}
