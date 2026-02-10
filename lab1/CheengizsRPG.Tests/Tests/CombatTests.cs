using CheengizsTestLib;
using CheengizsTestLib.Attributes;

namespace CheengizsRPG.Tests.Tests;

[TestClass]
public class CombatTests
{
    private Hero _hero;
    private Enemy _enemy;
    private Weapon _sword;

    [TestInitial]
    public void Setup()
    {
        _hero = new Hero("Arthur", 100);
        _enemy = new Enemy("Orc", 100, 10);
        _sword = new Weapon("Excalibur", 10, 20);
    }

    [TestMethod]
    public void Attack_NoWeapon_DealsBaseDamage()
    {
        _hero.Attack(_enemy); 
        Assert.IsEqual(95, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Attack_WithWeapon_DealsCombinedDamage()
    {
        _hero.EquipWeapon(_sword);
        _hero.Attack(_enemy);
        Assert.IsEqual(85, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Attack_WithWeapon_DecreasesDurability()
    {
        _hero.EquipWeapon(_sword);
        _hero.Attack(_enemy);
        Assert.IsEqual(19, _sword.CurrentDurability);
    }

    [TestMethod]
    public void Attack_WithBrokenWeapon_DealsReducedDamage()
    {
        for (int i = 0; i < 20; i++) _sword.DecreaseDurability();
        
        _hero.EquipWeapon(_sword);
        _hero.Attack(_enemy);
        
        Assert.IsEqual(94, _enemy.CurrentHealth);
    }

    [TestMethod]
    public void Attack_Self_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => _hero.Attack(_hero));
    }

    [TestMethod]
    public void Attack_NullTarget_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _hero.Attack(null));
    }

    [TestMethod]
    public void EquipWeapon_Null_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _hero.EquipWeapon(null));
    }

    [TestMethod]
    public void UseItem_Repair_RestoresDurability()
    {
        _hero.EquipWeapon(_sword);
        _sword.DecreaseDurability(); 
        _sword.DecreaseDurability(); 
        
        var repairKit = new RepairItem("Kit", 1);
        _hero.UseItem(repairKit);
        
        Assert.IsEqual(19, _sword.CurrentDurability);
    }

    [TestMethod]
    public void UseItem_Null_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _hero.UseItem(null));
    }

    [TestMethod]
    public void UseItem_NoWeapon_ThrowsException()
    {
        var repairKit = new RepairItem("Kit", 10);
        Assert.Throws<InvalidOperationException>(() => _hero.UseItem(repairKit));
    }

    [TestMethod]
    public void Enemy_Bite_DealsDamage()
    {
        _enemy.Bite(_hero);
        Assert.IsEqual(90, _hero.CurrentHealth);
    }

    [TestMethod]
    public void Enemy_Bite_Null_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _enemy.Bite(null));
    }

    [TestMethod]
    public void Hero_KillEnemy_EnemyIsDead()
    {
        var weakEnemy = new Enemy("Rat", 5, 1);
        _hero.Attack(weakEnemy);
        Assert.IsTrue(weakEnemy.IsDead);
    }

    [TestMethod]
    public void Attack_DeadHero_ThrowsException()
    {
        _hero.TakeDamage(100);
        Assert.Throws<InvalidOperationException>(() => _hero.Attack(_enemy));
    }
}