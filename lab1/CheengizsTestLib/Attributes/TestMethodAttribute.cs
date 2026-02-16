namespace CheengizsTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute 
{
    public int Priority { get; }
    public TestMethodAttribute(int priority = 0)
    {
        Priority = priority;
    }
}