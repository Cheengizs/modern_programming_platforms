using CheengizsTestLib;
using CheengizsTestLib.Attributes;

namespace CheengizsRPG.Tests.Tests;

[TestClass]
public class HeroTests
{
    private Hero _hero;

    [TestInitial]
    public void Setup()
    {
        _hero = new Hero("Arthur", 100);
    }

    [TestMethod]
    [Data("Lancelot", 150)]
    [Data("Merlin", 80)]
    public void Constructor_ValidData_SetsProperties(string name, int hp)
    {
        var h = new Hero(name, hp);
        Assert.IsEqual(name, h.Name);
        Assert.IsEqual(hp, h.MaxHealth);
        Assert.IsEqual(hp, h.CurrentHealth);
    }

    [TestMethod]
    public void Constructor_EmptyName_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var hero = new Hero("", 100);
        });
    }

    [TestMethod]
    public void Constructor_NegativeHealth_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var hero = new Hero("BadHero", -10);
        });
    }

    [TestMethod]
    [Data(10, 90)]
    [Data(50, 50)]
    [Data(100, 0)]
    [Data(999, 0)]
    public void TakeDamage_ValidDamage_ReducesHealth(int damage, int expectedHealth)
    {
        _hero.TakeDamage(damage);
        Assert.IsEqual(expectedHealth, _hero.CurrentHealth);
    }

    [TestMethod]
    public void TakeDamage_NegativeAmount_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _hero.TakeDamage(-5));
    }

    [TestMethod]
    public void Heal_ValidAmount_IncreasesHealth()
    {
        _hero.TakeDamage(50);
        _hero.Heal(20);
        Assert.IsEqual(70, _hero.CurrentHealth);
    }

    [TestMethod]
    public void Heal_ExceedsMax_CapsAtMaxHealth()
    {
        _hero.TakeDamage(10);
        _hero.Heal(50);
        Assert.IsEqual(100, _hero.CurrentHealth);
    }

    [TestMethod]
    public void IsDead_HealthZero_ReturnsTrue()
    {
        _hero.TakeDamage(100);
        Assert.IsTrue(_hero.IsDead, "Hero should be dead when HP is 0");
    }

    [TestMethod]
    public void IsDead_HealthPositive_ReturnsFalse()
    {
        _hero.TakeDamage(99);
        Assert.IsFalse(_hero.IsDead, "Hero should be alive when HP is 1");
    }

    [TestMethod]
    public void Heal_DeadHero_ThrowsInvalidOperation()
    {
        _hero.TakeDamage(100); 
        Assert.Throws<InvalidOperationException>(() => _hero.Heal(10));
    }

    [TestMethod]
    public void Equipment_NewHero_HasNoWeapon()
    {
        Assert.IsNull(_hero.EquippedWeapon, "New hero should not have weapon");
    }
}