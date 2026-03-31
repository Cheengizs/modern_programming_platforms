using CheengizsRPG.Abstractions;

namespace CheengizsRPG.Models;

public class Potion : HealItem
{
    public Potion(string name, int health) : base(name, health)
    {
    }
}
