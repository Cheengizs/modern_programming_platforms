namespace UnitTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class DynamicDataAttribute : Attribute
{
    public string MethodName { get; }
    public DynamicDataAttribute(string methodName) => MethodName = methodName;
}
