using CheengizsRPG.Abstractions;

namespace CheengizsRPG;

public class Hero : Character
{
    public Weapon? EquippedWeapon { get; private set; }
    private readonly List<Buff> _activeBuffs = new List<Buff>();
    public IReadOnlyCollection<Buff> Buffs => _activeBuffs;

    public Hero(string name, int maxHealth) : base(name, maxHealth)
    {
    }

    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon ?? throw new ArgumentNullException(nameof(weapon));
    }

    public void UseItem(RepairItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        if (EquippedWeapon == null) throw new InvalidOperationException("No weapon equipped to repair.");

        item.UseOn(EquippedWeapon);
    }

    public void Attack(Character target)
    {
        if (IsDead) throw new InvalidOperationException("Hero cannot attack while dead.");
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (target == this) throw new InvalidOperationException("Cannot attack yourself.");

        int baseDmg = 5;
        int weaponDmg = 0;

        if (EquippedWeapon != null)
        {
            if (!EquippedWeapon.IsBroken)
            {
                weaponDmg = EquippedWeapon.Damage;
                EquippedWeapon.DecreaseDurability();
            }
            else
            {
                weaponDmg = 1;
            }
        }

        double multiplier = 1.0;
        foreach (var buff in _activeBuffs.ToList())
        {
            if (buff.DurationInTurns > 0)
            {
                multiplier *= buff.DamageMultiplier;
            }

            buff.DurationInTurns--;
            if (buff.DurationInTurns <= 0)
            {
                _activeBuffs.Remove(buff);
            }
        }

        int totalDamage = (int)((baseDmg + weaponDmg) * multiplier);
        target.TakeDamage(totalDamage);
    }

    public void AddBuff(Buff buff)
    {
        if (buff.DurationInTurns > 0)
            _activeBuffs.Add(buff);
    }
}