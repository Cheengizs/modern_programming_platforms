namespace CheengizsRPG.Abstractions;

public abstract class RepairItem
{
    public string Name { get; set; }
    public int RepairAmount { get; set; }

    public RepairItem(string name, int repairAmount)
    {
        Name = name;
        RepairAmount = repairAmount;
    }
}
