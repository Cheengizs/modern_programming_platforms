namespace UnitTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MethodDataAttribute : Attribute
{
    private List<object> Data { get; set; }

    public MethodDataAttribute(params object[] data)
    {
        Data = data.ToList();
    }
}
