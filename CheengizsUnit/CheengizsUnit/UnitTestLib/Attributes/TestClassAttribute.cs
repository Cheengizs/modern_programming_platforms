namespace UnitTestLib.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TestClassAttribute : Attribute
{
    public bool Ignore { get; set; } = false;
    public Type? ContextType { get; set; }
}
