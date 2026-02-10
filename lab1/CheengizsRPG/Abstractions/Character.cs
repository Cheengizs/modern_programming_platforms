namespace CheengizsRPG.Abstractions;

public abstract class Character
{
    public string Name { get; }
    public int MaxHealth { get; }
    public int CurrentHealth { get; protected set; }
    public bool IsDead => CurrentHealth <= 0;

    protected Character(string name, int maxHealth)
    {
        if(string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
            
        if (maxHealth <= 0)
            throw new ArgumentException("Max health must be positive.");

        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        if (damage < 0) throw new ArgumentException("Damage cannot be negative.");

        CurrentHealth -= damage;
        if (CurrentHealth < 0) CurrentHealth = 0;
    }

    public void Heal(int amount)
    {
        if (IsDead) throw new InvalidOperationException("Cannot heal a dead character.");
        if (amount <= 0) throw new ArgumentException("Heal amount must be positive.");

        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
    }

    public async Task RestAsync(int durationMs)
    {
        if (IsDead) return;

        await Task.Delay(durationMs);

        int healAmount = (int)(MaxHealth * 0.1);
        Heal(Math.Max(1, healAmount));
    }
}