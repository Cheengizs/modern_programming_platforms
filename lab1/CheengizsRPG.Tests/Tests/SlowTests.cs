using CheengizsTestLib.Attributes;

namespace CheengizsRPG.Tests.Tests;

[TestClass]
public class SlowTests
{
    [TestMethod]
    [TestTimeout(2000)] 
    public void Test_Timeout_Fail()
    {
        Thread.Sleep(3000);
    }

    [TestMethod]
    public void Test_Slow_1()
    {
        Thread.Sleep(1000); 
    }

    [TestMethod]
    public void Test_Slow_2()
    {
        Thread.Sleep(1000);
    }

    [TestMethod]
    public void Test_Slow_3()
    {
        Thread.Sleep(10000);
    }
}