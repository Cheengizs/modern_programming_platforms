using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)] 
public class ParameterizedHeroTests
{
    [TestMethod(Timeout = 100)]
    [MethodData(10, 25, 50, 100, 1,2,3,4,5,6,7,8,9,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,3738,39,40)] 
    
    public void Heal_WithVariousPotionAmounts_IncreasesHealthCorrectly(int healAmount)
    {
        Thread.Sleep(200);
        Hero paladin = new Paladin("Arthur", 50, 15) { MaxHealth = 150 };
        HealItem potion = new Potion("Custom Potion", healAmount);
        paladin.AddHealItem(potion);

        paladin.Heal();

        int expectedHealth = Math.Min(50 + healAmount, 150);
        AssertResult.IsEqual(expectedHealth, paladin.Health, $"Failed for heal amount: {healAmount}");
    }

    [TestMethod]
    [MethodData("Goblin", "Orc", "Troll")] 
    public void Enemy_Creation_SetsNameCorrectly(string enemyName)
    {
        Enemy enemy = new Goblin(enemyName, 100, 10);
        AssertResult.IsEqual(enemyName, enemy.Name);
    }
}
