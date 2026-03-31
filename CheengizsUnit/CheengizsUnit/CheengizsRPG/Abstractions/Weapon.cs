namespace CheengizsRPG.Abstractions;

public abstract class Weapon
{
    public int Damage { get; set; }
    public int Durability { get; set; }
    public int MaxDurability { get; set; }

    public Weapon(int damage, int durability, int maxDurability)
    {
        Damage = damage;
        Durability = durability;
        MaxDurability = maxDurability;
    }
    
    public void AddDurability(int durability)
    {
        Durability = Math.Min(Durability + durability, MaxDurability);
    }
}
