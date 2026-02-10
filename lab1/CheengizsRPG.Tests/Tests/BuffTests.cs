using CheengizsTestLib;
using CheengizsTestLib.Attributes;

namespace CheengizsRPG.Tests.Tests;

[TestClass]
public class BuffTests
{
    private Hero _hero;
    private Enemy _enemy;

    [TestInitial]
    public void Setup()
    {
        _hero = new Hero("Mage", 100);
        _enemy = new Enemy("Dummy", 200, 0);
    }

    [TestMethod]
    public void Buff_DoubleDamage_IncreasesAttack()
    {
        var rage = new Buff("Rage", 2.0, 1);
        _hero.AddBuff(rage);
        
        _hero.Attack(_enemy);
        Assert.IsEqual(190, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Buff_HalfDamage_DecreasesAttack()
    {
        var weak = new Buff("Weakness", 0.5, 1);
        _hero.AddBuff(weak);
        
        _hero.Attack(_enemy); 
        Assert.IsEqual(198, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Buff_Expires_AfterDuration()
    {
        var power = new Buff("Power", 2.0, 1);
        _hero.AddBuff(power);
        
        _hero.Attack(_enemy); 
        Assert.IsEqual(190, _enemy.CurrentHealth);
        
        _hero.Attack(_enemy); 
        Assert.IsEqual(185, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Buff_Stacking_MultipliesDamage()
    {
        var b1 = new Buff("B1", 2.0, 1);
        var b2 = new Buff("B2", 2.0, 1);
        
        _hero.AddBuff(b1);
        _hero.AddBuff(b2);
        
        _hero.Attack(_enemy); 
        Assert.IsEqual(180, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Buff_ZeroDuration_RemovesImmediately()
    {
        var b1 = new Buff("Flash", 10.0, 0); 
        _hero.AddBuff(b1);
        
        _hero.Attack(_enemy);
        Assert.IsEqual(195, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Buff_NegativeDuration_DoesNotCrash()
    {
        var b1 = new Buff("Bug", 2.0, -1);
        _hero.AddBuff(b1);
        _hero.Attack(_enemy);
        Assert.IsEqual(195, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Buff_IsExistsAfterZeroDuration_NotExists()
    {
        var buff = new Buff("NotABuff", 2.0, 0);
        _hero.AddBuff(buff);
        Assert.NotContains(_hero.Buffs, buff);
    }
}