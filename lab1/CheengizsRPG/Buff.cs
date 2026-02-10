namespace CheengizsRPG;

public class Buff
{
    public string Name { get; }
    public double DamageMultiplier { get; }
    public int DurationInTurns { get; set; }

    public Buff(string name, double damageMultiplier, int durationInTurns)
    {
        Name = name;
        DamageMultiplier = damageMultiplier;
        DurationInTurns = durationInTurns;
    }
}