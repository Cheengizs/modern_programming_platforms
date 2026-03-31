using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)] 
public class EnemyTests
{
    [TestMethod]
    public void Constructor_ShouldSetValues()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        AssertResult.IsEqual("Grob", goblin.Name);
        AssertResult.IsNotEqual(0, goblin.Health);
    }

    [TestMethod]
    public void Enemy_IsInstanceOfGoblin()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        AssertResult.IsInstanceOf<Goblin>(goblin);
    }

    [TestMethod]
    public void Attack_HeroNullTarget_ThrowsException()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        Hero? nullHero = null;
        
        AssertResult.Throws<NullReferenceException>(() => goblin.Attack(nullHero!));
    }

    [TestMethod]
    public async Task AttackAsync_NullTarget_ThrowsAsyncException()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        
        await AssertResult.ThrowsAsync<NullReferenceException>(async () => 
        {
            await Task.Delay(1); 
            Hero? nullHero = null;
            goblin.Attack(nullHero!);
        });
    }

    [TestMethod]
    public void Attack_DecreasesHeroHealth()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        Hero paladin = new Paladin("Arthur", 100, 15);
        
        goblin.Attack(paladin);
        AssertResult.IsEqual(90, paladin.Health);
    }

    [TestMethod]
    public void EnemyWeapon_InitiallyNull()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        AssertResult.IsNull(goblin.Weapon);
    }

    [TestMethod]
    public void Damage_ShouldBePositive()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        AssertResult.IsTrue(goblin.Damage > 0);
    }

    [TestMethod]
    public void Health_ShouldNotBeNegativeInitially()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        AssertResult.IsFalse(goblin.Health < 0);
    }

    [TestMethod]
    public void AttackWithWeapon_DealsWeaponDamageIfHigher()
    {
        Enemy goblin = new Goblin("Grob", 50, 10);
        goblin.Weapon = new Axe(25, 100, 100);
        Hero paladin = new Paladin("Arthur", 100, 15);
        
        goblin.Attack(paladin);
        AssertResult.IsEqual(75, paladin.Health); 
    }

    [TestMethod]
    public void ForceFail_WithAssertFailedException_CaughtByThrows()
    {

        AssertResult.Throws<UnitTestLib.Exceptions.AssertFailedException>(() => 
        {
            AssertResult.Fail("This is a forced failure for testing purposes.");
        });
    }
}
