using CheengizsRPG.Abstractions;

namespace CheengizsRPG;

public class Enemy : Character
{
    public int BaseDamage { get; }

    public Enemy(string name, int maxHealth, int baseDamage) : base(name, maxHealth)
    {
        BaseDamage = baseDamage;
    }

    public void Bite(Character target)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }
        
        target.TakeDamage(BaseDamage);
    }
}