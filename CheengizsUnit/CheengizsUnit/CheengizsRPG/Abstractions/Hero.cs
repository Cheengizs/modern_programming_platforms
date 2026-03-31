namespace CheengizsRPG.Abstractions;

public abstract class Hero
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Damage { get; set; }
    public Weapon? Weapon { get; set; }
    public Queue<RepairItem> RepairItems { get; set; } = new Queue<RepairItem>();
    public Queue<HealItem> HealItems { get; set; } = new Queue<HealItem>();

    public Hero(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }

    public void Attack(Enemy target)
    {
        if (Weapon?.Damage > this.Damage)
        {
            target.Health -= Weapon.Damage;
        }
        else
        {
            target.Health -= this.Damage;
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        Weapon = weapon;
    }

    public void RepairItem()
    {
        if (RepairItems.Count == 0)
        {
            return;
        }

        if (Weapon is null)
        {
            return;
        }

        Weapon.AddDurability(RepairItems.Dequeue().RepairAmount);
    }

    public void Heal()
    {
        if (HealItems.Count == 0)
        {
            return;
        }
        
        Health = Math.Min(Health + HealItems.Dequeue().Health, MaxHealth);
    }

    public void AddRepairItem(RepairItem repairItem)
    {
        RepairItems.Enqueue(repairItem);
    }

    public void AddHealItem(HealItem healItem)
    {
        HealItems.Enqueue(healItem);
    }
}
