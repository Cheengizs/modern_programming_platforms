namespace CheengizsRPG;

public class Weapon
{
    public string Name { get; }
    public int Damage { get; }
    public int MaxDurability { get; }
    public int CurrentDurability { get; private set; }

    public bool IsBroken => CurrentDurability <= 0;

    public Weapon(string name, int damage, int maxDurability)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Weapon name cannot be empty.", nameof(name));
            
        if (damage < 0)
            throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            
        if (maxDurability <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxDurability), "Durability must be positive.");

        Name = name;
        Damage = damage;
        MaxDurability = maxDurability;
        CurrentDurability = maxDurability;
    }

    public void DecreaseDurability()
    {
        if (CurrentDurability > 0)
        {
            CurrentDurability--;
        }
    }

    public void RestoreDurability(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Repair amount must be positive.");
            
        if (CurrentDurability == MaxDurability)
            throw new InvalidOperationException("Weapon is already fully repaired.");

        CurrentDurability += amount;
            
        if (CurrentDurability > MaxDurability)
        {
            CurrentDurability = MaxDurability;
        }
    }
}