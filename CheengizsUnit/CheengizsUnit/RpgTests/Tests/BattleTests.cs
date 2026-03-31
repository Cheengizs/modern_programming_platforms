using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)]
public class BattleTests
{
    private Paladin _hero;
    private Goblin _enemy;

    [TestInitialize]
    public void Setup()
    {
        _hero = new Paladin("Sir Lancelot", 100, 10);
        _hero.MaxHealth = 100; 
        _enemy = new Goblin("Gobby", 50, 5);
    }

    [TestMethod]
    public void HeroAttacksEnemy_EnemyHealthShouldDecrease()
    {
        var initialEnemyHealth = _enemy.Health;

        _hero.Attack(_enemy);

        AssertResult.IsEqual(initialEnemyHealth - _hero.Damage, _enemy.Health,
            "Enemy health should decrease by hero's base damage.");
    }

    [TestMethod]
    public void HeroAttacksEnemyWithWeapon_EnemyHealthShouldDecreaseByWeaponDamage()
    {
        var weapon = new Axe(20, 100, 100);
        _hero.SetWeapon(weapon);
        var initialEnemyHealth = _enemy.Health;

        _hero.Attack(_enemy);

        AssertResult.IsEqual(initialEnemyHealth - weapon.Damage, _enemy.Health,
            "Enemy health should decrease by weapon damage when weapon is equipped.");
    }

    [TestMethod]
    public void HeroHeals_HealthShouldIncrease()
    {
        _hero.Health = 50;
        var healItem = new Potion("Small Potion", 20);
        _hero.AddHealItem(healItem);
        var initialHeroHealth = _hero.Health;

        _hero.Heal();

        AssertResult.IsEqual(initialHeroHealth + healItem.Health, _hero.Health,
            "Hero health should increase after healing.");
    }

    [TestMethod]
    public void HeroHeals_HealthShouldNotExceedMaxHealth()
    {
        _hero.Health = 90;
        var healItem = new Potion("Large Potion", 50);
        _hero.AddHealItem(healItem);

        _hero.Heal();

        AssertResult.IsEqual(_hero.MaxHealth, _hero.Health, "Hero health should not exceed MaxHealth after healing.");
    }

    [TestMethod]
    public void HeroHealsWithoutHealItems_HealthShouldNotChange()
    {
        _hero.HealItems.Clear();
        var initialHeroHealth = _hero.Health;

        _hero.Heal();

        AssertResult.IsEqual(initialHeroHealth, _hero.Health,
            "Hero health should not change if no heal items are available.");
        AssertResult.IsEmpty(_hero.HealItems, "Heal items queue should remain empty.");
    }

    [TestMethod]
    public void HeroRepairsWeapon_DurabilityShouldIncrease()
    {
        var weapon = new Axe(10, 50, 100);
        _hero.SetWeapon(weapon);
        var repairItem = new RepairKit("Basic Kit", 30);
        _hero.AddRepairItem(repairItem);
        var initialDurability = weapon.Durability;

        _hero.RepairItem();

        AssertResult.IsEqual(initialDurability + repairItem.RepairAmount, weapon.Durability,
            "Weapon durability should increase after repair.");
        AssertResult.IsEmpty(_hero.RepairItems, "Repair items queue should be empty after use.");
    }

    [TestMethod]
    public void HeroRepairsWeapon_DurabilityShouldNotExceedMaxDurability()
    {
        var weapon = new Axe(10, 90, 100);
        _hero.SetWeapon(weapon);
        var repairItem = new RepairKit("Super Kit", 50);
        _hero.AddRepairItem(repairItem);

        _hero.RepairItem();

        AssertResult.IsEqual(weapon.MaxDurability, weapon.Durability,
            "Weapon durability should not exceed MaxDurability after repair.");
    }

    [TestMethod]
    public void HeroRepairsWeaponWithoutRepairItems_DurabilityShouldNotChange()
    {
        var weapon = new Axe(10, 50, 100);
        _hero.SetWeapon(weapon);
        _hero.RepairItems.Clear();
        var initialDurability = weapon.Durability;

        _hero.RepairItem();

        AssertResult.IsEqual(initialDurability, weapon.Durability,
            "Weapon durability should not change if no repair items are available.");
        AssertResult.IsEmpty(_hero.RepairItems, "Repair items queue should remain empty.");
    }

    [TestMethod]
    public void HeroRepairsWeaponWithoutWeapon_RepairItemShouldNotBeConsumed()
    {
        _hero.Weapon = null;
        var repairItem = new RepairKit("Kit", 10);
        _hero.AddRepairItem(repairItem);

        _hero.RepairItem();

        AssertResult.IsEqual(1, _hero.RepairItems.Count, "Repair item should not be consumed if hero has no weapon.");
        AssertResult.Contains(_hero.RepairItems, repairItem, "The repair item should still be in the queue.");
    }

    [TestMethod]
    public void HeroHasCorrectInitialProperties()
    {
        AssertResult.IsEqual("Sir Lancelot", _hero.Name, "Hero's name should be 'Sir Lancelot'.");
        AssertResult.IsEqual(100, _hero.Health, "Hero's initial health should be 100.");
        AssertResult.IsEqual(10, _hero.Damage, "Hero's initial damage should be 10.");
        AssertResult.IsNotNull(_hero.HealItems, "HealItems queue should not be null.");
        AssertResult.IsNotNull(_hero.RepairItems, "RepairItems queue should not be null.");
        AssertResult.IsNull(_hero.Weapon, "Hero should not have a weapon initially.");
        AssertResult.IsInstanceOf<Paladin>(_hero, "Hero should be an instance of Paladin.");
    }

    [TestMethod]
    public void EnemyAttacksHero_HeroHealthShouldDecrease()
    {
        var initialHeroHealth = _hero.Health;

        _enemy.Attack(_hero);

        AssertResult.IsEqual(initialHeroHealth - _enemy.Damage, _hero.Health,
            "Hero health should decrease by enemy's base damage.");
    }

    [TestMethod]
    public void EnemyAttacksHeroWithWeapon_HeroHealthShouldDecreaseByWeaponDamage()
    {
        var weapon = new Axe(15, 100, 100);
        _enemy.Weapon = weapon;
        var initialHeroHealth = _hero.Health;

        _enemy.Attack(_hero);

        AssertResult.IsEqual(initialHeroHealth - weapon.Damage, _hero.Health,
            "Hero health should decrease by enemy's weapon damage when weapon is equipped.");
    }

    [TestMethod]
    public void EnemyHasCorrectInitialProperties()
    {
        AssertResult.IsEqual("Gobby", _enemy.Name, "Enemy's name should be 'Gobby'.");
        AssertResult.IsEqual(50, _enemy.Health, "Enemy's initial health should be 50.");
        AssertResult.IsEqual(5, _enemy.Damage, "Enemy's initial damage should be 5.");
        AssertResult.IsNull(_enemy.Weapon, "Enemy should not have a weapon initially.");
        AssertResult.IsInstanceOf<Goblin>(_enemy, "Enemy should be an instance of Goblin.");
    }

    [TestMethod]
    public void EnemyWeaponDamageIsUsedIfHigherThanBaseDamage()
    {
        _enemy.Damage = 5;
        var weapon = new Axe(10, 100, 100);
        _enemy.Weapon = weapon;
        var initialHeroHealth = _hero.Health;

        _enemy.Attack(_hero);

        AssertResult.IsEqual(initialHeroHealth - weapon.Damage, _hero.Health,
            "Enemy should use weapon damage if it's higher than base damage.");
    }

    [TestMethod]
    public void EnemyBaseDamageIsUsedIfHigherThanWeaponDamage()
    {
        _enemy.Damage = 10;
        var weapon = new Axe(5, 100, 100);
        _enemy.Weapon = weapon;
        var initialHeroHealth = _hero.Health;

        _enemy.Attack(_hero);

        AssertResult.IsEqual(initialHeroHealth - _enemy.Damage, _hero.Health,
            "Enemy should use base damage if it's higher than weapon damage.");
    }

    [TestMethod]
    public void EnemyAttackWithNoWeaponUsesBaseDamage()
    {
        _enemy.Weapon = null;
        var initialHeroHealth = _hero.Health;

        _enemy.Attack(_hero);

        AssertResult.IsEqual(initialHeroHealth - _enemy.Damage, _hero.Health,
            "Enemy should use base damage if no weapon is equipped.");
    }

    [TestMethod]
    public void EnemyAttackWithZeroDamageWeaponUsesBaseDamage()
    {
        _enemy.Damage = 10;
        var weapon = new Axe(0, 100, 100);
        _enemy.Weapon = weapon;
        var initialHeroHealth = _hero.Health;

        _enemy.Attack(_hero);

        AssertResult.IsEqual(initialHeroHealth - _enemy.Damage, _hero.Health,
            "Enemy should use base damage if weapon damage is zero and base damage is higher.");
    }

    [TestMethod]
    public void EnemyIsInstanceOfGoblin()
    {
        AssertResult.IsInstanceOf<Goblin>(_enemy, "Enemy object should be an instance of Goblin.");
    }

    [TestMethod]
    public void EnemyIsNotInstanceOfPaladin()
    {
        AssertResult.IsFalse(_enemy is Paladin, "Enemy object should not be an instance of Paladin.");
        AssertResult.IsNotEqual(typeof(Paladin), _enemy.GetType(), "Enemy object type should not be Paladin.");
    }
}
