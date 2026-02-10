namespace CheengizsTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute 
{
    public string Description { get; }
    public TestMethodAttribute(string description = "")
    {
        Description = description;
    }
}