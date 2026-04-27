using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = false)]
public class Lab4DemoTests
{
    public static IEnumerable<object[]> GetHeroData()
    {
        yield return ["Arthur", 100, 15];
        yield return ["Lancelot", 120, 20];
        yield return ["Galahad", 90, 10];
        yield return ["Galahad1", 90, 10];
        yield return ["Galahad2", 90, 10];
        yield return ["Galahad3", 90, 10];
        yield return ["Galahad4", 90, 10];
    }

    [TestMethod]
    [Category("Parametric")]
    [DynamicData(nameof(GetHeroData))]
    public void HeroCreation_UsingYieldReturn(string name, int health, int damage)
    {
        var hero = new Paladin(name, health, damage);
        AssertResult.Check(() => hero.Name == name);
        AssertResult.Check(() => hero.Health == health);
    }

    [TestMethod]
    [Category("Parametric")]
    public void ExpressionTree_FailingAssert_ShowsDetailedInfo()
    {
        int actualDamage = 15;
        int expectedDamage = 20;

        AssertResult.Check(() => actualDamage == expectedDamage, "Проверка разбора дерева выражений");
    }

    [TestMethod]
    [Category("Disabled")]
    public void Disabled_Test()
    {
        throw new NotImplementedException("This method is disabled");
    }
}
