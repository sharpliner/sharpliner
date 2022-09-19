namespace Sharpliner.SourceGenerator;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class StringConditionAttribute : Attribute
{
    public bool HasArrayParameter1 { get; set; }
    public bool HasArrayParameter2 { get; set; }
}
