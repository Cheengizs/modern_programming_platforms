using CheengizsTestLib;
using CheengizsTestLib.Attributes;

namespace CheengizsRPG.Tests.Tests;

[TestClass]
public class WeaponTests
{
    private Weapon _sword;

    [TestInitial]
    public void Setup()
    {
        _sword = new Weapon("Excalibur", 10, 20);
    }

    [TestMethod]
    public void Constructor_ValidParams_CreatesInstance()
    {
        Assert.IsNotNull(_sword);
        Assert.IsEqual<string>("Excalibur", _sword.Name);
    }

    [TestMethod]
    public void Constructor_ZeroDurability_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Weapon("Stick", 1, 0));
    }

    [TestMethod]
    public void DecreaseDurability_ReducesByOne()
    {
        _sword.DecreaseDurability();
        Assert.IsEqual(19, _sword.CurrentDurability);
    }

    [TestMethod]
    public void IsBroken_DurabilityZero_ReturnsTrue()
    {
        for(int i=0; i<20; i++) _sword.DecreaseDurability();
        
        Assert.IsEqual(0, _sword.CurrentDurability);
        Assert.IsTrue(_sword.IsBroken, "Weapon should be broken");
    }

    [TestMethod]
    public void IsBroken_DurabilityPositive_ReturnsFalse()
    {
        _sword.DecreaseDurability();
        Assert.IsFalse(_sword.IsBroken);
    }

    [TestMethod]
    public void RestoreDurability_ValidAmount_Restores()
    {
        _sword.DecreaseDurability(); 
        _sword.DecreaseDurability(); 
        
        _sword.RestoreDurability(1);
        Assert.IsEqual(19, _sword.CurrentDurability);
    }

    [TestMethod]
    public void RestoreDurability_ExceedsMax_CapsAtMax()
    {
        _sword.DecreaseDurability(); 
        _sword.RestoreDurability(100); 
        
        Assert.IsEqual(20, _sword.CurrentDurability);
    }

    [TestMethod]
    public void RestoreDurability_FullWeapon_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _sword.RestoreDurability(5));
    }

    [TestMethod]
    public void Weapons_DifferentInstances_AreNotEqual()
    {
        var sword1 = new Weapon("A", 10, 10);
        var sword2 = new Weapon("A", 10, 10);
        
        Assert.IsNotEqual(sword1, sword2);
    }
    
    [TestMethod]
    public void Weapon_IsObject_CheckType()
    {
        Assert.IsInstanceOf<object>(_sword); 
        Assert.IsInstanceOf<Weapon>(_sword); 
    }
}