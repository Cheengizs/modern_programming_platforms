using System.ComponentModel;

namespace UnitTestLib.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestMethodAttribute : Attribute
{
    private string Name { get; set; }
    public int Timeout { get; set; } = 0;
}
