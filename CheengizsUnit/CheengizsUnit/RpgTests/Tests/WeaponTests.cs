using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)]
public class WeaponTests
{
    [TestMethod]
    public void Constructor_SetsDamageAndDurability()
    {
        Weapon axe = new Axe(20, 50, 100);
        AssertResult.IsEqual(20, axe.Damage);
        AssertResult.IsEqual(50, axe.Durability);
        AssertResult.IsEqual(100, axe.MaxDurability);
    }

    [TestMethod]
    public void Weapon_IsInstanceOfAxe()
    {
        Weapon axe = new Axe(20, 50, 100);
        AssertResult.IsInstanceOf<Axe>(axe);
    }

    [TestMethod]
    public void Damage_IsNotZero()
    {
        Weapon axe = new Axe(20, 50, 100);
        AssertResult.IsNotEqual(0, axe.Damage);
    }

    [TestMethod]
    public void AddDurability_IncreasesValue()
    {
        Weapon axe = new Axe(20, 50, 100);
        axe.AddDurability(20);
        AssertResult.IsEqual(70, axe.Durability);
    }

    [TestMethod]
    public void AddDurability_DoesNotExceedMaxDurability()
    {
        Weapon axe = new Axe(20, 90, 100);
        axe.AddDurability(30);
        AssertResult.IsEqual(100, axe.Durability);
    }

    [TestMethod]
    public void Durability_IsPositiveCondition_ReturnsTrue()
    {
        Weapon axe = new Axe(20, 50, 100);
        AssertResult.IsTrue(axe.Durability > 0);
    }

    [TestMethod]
    public void MaxDurability_IsZero_ReturnsFalse()
    {
        Weapon axe = new Axe(20, 50, 100);
        AssertResult.IsFalse(axe.MaxDurability == 0);
    }

    [TestMethod]
    public void WeaponCollection_ContainsCreatedAxe()
    {
        Weapon axe1 = new Axe(10, 10, 10);
        Weapon axe2 = new Axe(20, 20, 20);
        List<Weapon> armory = new List<Weapon> { axe1, axe2 };
        
        AssertResult.Contains(armory, axe1);
    }

    [TestMethod]
    public void WeaponCollection_NotContainsExternalAxe()
    {
        Weapon axe1 = new Axe(10, 10, 10);
        Weapon axeExternal = new Axe(30, 30, 30);
        List<Weapon> armory = new List<Weapon> { axe1 };
        
        AssertResult.NotContains(armory, axeExternal);
    }

    [TestMethod]
    public void WeaponCollection_SingleWeapon_IsSingle()
    {
        List<Weapon> armory = new List<Weapon> { new Axe(10, 10, 10) };
        AssertResult.Single(armory);
    }
}
