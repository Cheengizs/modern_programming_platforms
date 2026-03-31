namespace CheengizsRPG.Abstractions;

public abstract class HealItem
{
    public string Name { get; set; }
    public int Health { get; set; }

    public HealItem(string name, int health)
    {
        Name = name;
        Health = health;
    }
}
