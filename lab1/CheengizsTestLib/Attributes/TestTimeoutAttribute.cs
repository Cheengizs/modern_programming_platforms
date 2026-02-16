namespace CheengizsTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestTimeoutAttribute : Attribute
{
    public int TimeoutMilliseconds { get; }

    public TestTimeoutAttribute(int milliseconds)
    {
        TimeoutMilliseconds = milliseconds;
    }
}