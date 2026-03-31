using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)] 
public class HeroTests
{
    [TestMethod]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        AssertResult.IsEqual("Arthur", paladin.Name);
        AssertResult.IsEqual(100, paladin.Health);
        AssertResult.IsEqual(15, paladin.Damage);
    }

    [TestMethod]
    public void Hero_IsInstanceOfPaladin()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        AssertResult.IsInstanceOf<Paladin>(paladin);
    }

    [TestMethod]
    public void SetWeapon_ShouldAssignWeaponAndNotBeNull()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        AssertResult.IsNull(paladin.Weapon);
        
        paladin.SetWeapon(new Axe(20, 100, 100));
        AssertResult.IsNotNull(paladin.Weapon);
    }

    [TestMethod]
    public void Attack_NullTarget_ShouldThrowNullReferenceException()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        Enemy? nullEnemy = null;
        
        AssertResult.Throws<NullReferenceException>(() => paladin.Attack(nullEnemy!));
    }

    [TestMethod]
    public void Attack_WithoutWeapon_ShouldDecreaseEnemyHealthByHeroDamage()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        Enemy goblin = new Goblin("Uruk", 50, 5);
        
        paladin.Attack(goblin);
        AssertResult.IsEqual(35, goblin.Health);
    }

    [TestMethod]
    public void HealItemsQueue_Initially_IsEmpty()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        AssertResult.IsEmpty(paladin.HealItems);
    }

    [TestMethod]
    public void AddHealItem_ShouldMakeQueueSingle()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        HealItem potion = new Potion("Small Potion", 20);
        
        paladin.AddHealItem(potion);
        AssertResult.Single(paladin.HealItems);
    }

    [TestMethod]
    public void AddMultipleHealItems_ShouldMakeQueueNotSingle()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        paladin.AddHealItem(new Potion("Potion 1", 20));
        paladin.AddHealItem(new Potion("Potion 2", 20));
        
        AssertResult.NotSingle(paladin.HealItems);
    }

    [TestMethod]
    public void Heal_ShouldDequeueItem_QueueNotContainsItAnymore()
    {
        Hero paladin = new Paladin("Arthur", 80, 15) { MaxHealth = 100 };
        HealItem potion = new Potion("Health Potion", 20);
        
        paladin.AddHealItem(potion);
        AssertResult.Contains(paladin.HealItems, potion);
        
        paladin.Heal();
        AssertResult.NotContains(paladin.HealItems, potion);
    }

    [TestMethod]
    public void RepairItem_WithoutWeapon_LogsButDoesNotThrow()
    {
        Hero paladin = new Paladin("Arthur", 100, 15);
        paladin.AddRepairItem(new RepairKit("Kit", 10));
        
        try
        {
            paladin.RepairItem(); 
            AssertResult.IsTrue(true);
        }
        catch
        {
            AssertResult.Fail("RepairItem threw an exception when it shouldn't have.");
        }
    }
}
