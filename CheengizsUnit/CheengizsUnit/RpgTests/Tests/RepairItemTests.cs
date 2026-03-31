using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)]
public class RepairItemTests
{
    [TestMethod]
    public void Constructor_SetsNameAndRepairAmount()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        AssertResult.IsEqual("Iron Kit", kit.Name);
        AssertResult.IsEqual(50, kit.RepairAmount);
    }

    [TestMethod]
    public void RepairItem_IsInstanceOfRepairKit()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        AssertResult.IsInstanceOf<RepairKit>(kit);
    }

    [TestMethod]
    public void RepairAmount_IsPositive_IsTrue()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        AssertResult.IsTrue(kit.RepairAmount > 0);
    }

    [TestMethod]
    public void RepairAmount_IsZero_IsFalse()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        AssertResult.IsFalse(kit.RepairAmount == 0);
    }

    [TestMethod]
    public void Name_IsNotNull()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        AssertResult.IsNotNull(kit.Name);
    }

    [TestMethod]
    public void Name_IsNotEqual_WrongString()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        AssertResult.IsNotEqual("Wood Kit", kit.Name);
    }

    [TestMethod]
    public void NullRepairItem_IsNull()
    {
        RepairItem? kit = null;
        AssertResult.IsNull(kit);
    }

    [TestMethod]
    public void RepairKitCollection_ContainsItem()
    {
        RepairItem kit = new RepairKit("Iron Kit", 50);
        List<RepairItem> stash = new List<RepairItem> { kit };
        
        AssertResult.Contains(stash, kit);
    }

    [TestMethod]
    public void RepairKitCollection_Empty_IsEmpty()
    {
        List<RepairItem> stash = new List<RepairItem>();
        AssertResult.IsEmpty(stash);
    }

    [TestMethod]
    public async Task AsyncSimulation_ThrowsAsync_WhenTargetNull()
    {
        await AssertResult.ThrowsAsync<NullReferenceException>(async () =>
        {
            await Task.Run(() => 
            {
                RepairItem? nullKit = null;
                int amount = nullKit!.RepairAmount;
            });
        });
    }
}
