namespace CheengizsRPG.Abstractions;

public class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Damage { get; set; }
    
    public Weapon? Weapon { get; set; }
    
    public Enemy(string name, int health, int damage)
    {
        Name = name;
        Health = health;
        Damage = damage;
    }

    public void Attack(Hero target)
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
}
