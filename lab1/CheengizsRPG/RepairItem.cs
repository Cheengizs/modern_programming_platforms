namespace CheengizsRPG;

public class RepairItem
{
    public string Name { get; }
    public int RepairPower { get; }

    public RepairItem(string name, int repairPower)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.");
            
        if (repairPower <= 0)
            throw new ArgumentOutOfRangeException(nameof(repairPower), "Repair power must be positive.");

        Name = name;
        RepairPower = repairPower;
    }

    public void UseOn(Weapon weapon)
    {
        if (weapon == null)
            throw new ArgumentNullException(nameof(weapon));

        weapon.RestoreDurability(RepairPower);
    }
}