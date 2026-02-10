using CheengizsRPG.Abstractions;
using CheengizsTestLib;
using CheengizsTestLib.Attributes;

namespace CheengizsRPG.Tests.Tests;

[TestClass]
public class InventoryTests
{
    private Hero _hero;

    [TestInitial]
    public void Setup()
    {
        _hero = new Hero("Traveler", 100);
    }

    [TestMethod]
    public async Task RestAsync_IncreasesHealth()
    {
        _hero.TakeDamage(50);
        await _hero.RestAsync(10);
        Assert.IsEqual(60, _hero.CurrentHealth);
    }

    [TestMethod]
    public async Task RestAsync_DeadHero_DoesNothing()
    {
        _hero.TakeDamage(100);
        await _hero.RestAsync(10);
        Assert.IsEqual(0, _hero.CurrentHealth);
    }

    [TestMethod]
    public async Task RestAsync_FullHealth_DoesNotExceedMax()
    {
        await _hero.RestAsync(10);
        Assert.IsEqual(100, _hero.CurrentHealth);
    }

    [TestMethod]
    public void Collection_EmptyList_IsEmpty()
    {
        var list = new List<Weapon>();
        Assert.IsEmpty(list);
    }

    [TestMethod]
    public void Collection_OneItem_IsSingle()
    {
        var list = new List<Weapon> { new Weapon("Dagger", 5, 10) };
        Assert.Single(list);
    }

    [TestMethod]
    public void Collection_MultipleItems_IsNotSingle()
    {
        var list = new List<int> { 1, 2 };
        Assert.NotSingle(list);
    }

    [TestMethod]
    public void Type_HeroIsCharacter()
    {
        Assert.IsInstanceOf<Character>(_hero);
    }
    
    [TestMethod]
    public void Type_HeroIsNotWeapon()
    {
        try 
        {
            Assert.IsInstanceOf<Weapon>(_hero);
        }
        catch (Exception)
        {
            return;
        }
        throw new Exception("Should have failed");
    }
    
    [TestMethod]
    public void Collection_NotEmpty_ThrowsException()
    {
        var list = new List<int> { 1 };
        try
        {
            Assert.IsEmpty(list);
        }
        catch
        {
            return;
        }
        throw new Exception("Should have failed");
    }
}
