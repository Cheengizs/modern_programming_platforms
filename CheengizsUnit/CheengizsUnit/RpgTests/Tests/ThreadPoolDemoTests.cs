using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = false )]
public class ThreadPoolDemoTests
{
    [TestMethod(Timeout = 5000)] 
    [MethodData(
        1,2,3,4,5,6,7,8,9,10,
        11,12,13,14,15,16,17,18,19,20,
        21,22,23,24,25,26,27,28,29,30,
        31,32,33,34,35,36,37,38,39,40)] 
    public void HeavyLoad_SimulatedWork(int id)
    {
        Thread.Sleep(500); 
        
        Hero paladin = new Paladin("Arthur", 50, 15) { MaxHealth = 150 };
        paladin.AddHealItem(new Potion("Potion", 10));
        paladin.Heal();
        
        AssertResult.IsEqual(60, paladin.Health);
    }

    [TestMethod]
    [MethodData("Goblin", "Orc", "POISONOUS_SLIME", "Troll")] 
    public void Enemy_Creation_WithFaultTolerance(string enemyName)
    {
        if (enemyName == "POISONOUS_SLIME")
        {
            throw new InvalidOperationException("CRITICAL SYSTEM FAILURE in working thread!");
        }

        Enemy enemy = new Goblin(enemyName, 100, 10);
        AssertResult.IsEqual(enemyName, enemy.Name);
    }

    [TestMethod(Timeout = 300)]
    public void EndlessLoop_ShouldBeKilledByTimeout()
    {
        
        Thread.Sleep(5000); 
    }
}
