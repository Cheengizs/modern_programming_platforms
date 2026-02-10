namespace CheengizsTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DataAttribute : Attribute
{
    public object[] Data { get; }

    public DataAttribute(params object[] data)
    {
        Data = data;
    }
}