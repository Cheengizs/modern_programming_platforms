using CheengizsRPG.Abstractions;
using CheengizsRPG.Models;
using UnitTestLib.Asserts;
using UnitTestLib.Attributes;

namespace RpgTests.Tests;

[TestClass(Ignore = true)] 
public class HealItemTests
{
    [TestMethod]
    public void Constructor_SetsNameAndHealth()
    {
        HealItem potion = new Potion("Minor Potion", 25);
        AssertResult.IsEqual("Minor Potion", potion.Name);
        AssertResult.IsEqual(25, potion.Health);
    }

    [TestMethod]
    public void HealItem_IsInstanceOfPotion()
    {
        HealItem potion = new Potion("Minor Potion", 25);
        AssertResult.IsInstanceOf<Potion>(potion);
    }

    [TestMethod]
    public void PotionName_IsNotNull()
    {
        HealItem potion = new Potion("Minor Potion", 25);
        AssertResult.IsNotNull(potion.Name);
    }

    [TestMethod]
    public void NullPotionReference_IsNull()
    {
        HealItem? nullPotion = null;
        AssertResult.IsNull(nullPotion);
    }

    [TestMethod]
    public void PotionHealth_IsNotEqualZero()
    {
        HealItem potion = new Potion("Minor Potion", 25);
        AssertResult.IsNotEqual(0, potion.Health);
    }

    [TestMethod]
    public void PotionHealth_IsPositive()
    {
        HealItem potion = new Potion("Minor Potion", 25);
        AssertResult.IsTrue(potion.Health > 0);
    }

    [TestMethod]
    public void PotionHealth_IsNegative_ReturnsFalse()
    {
        HealItem potion = new Potion("Minor Potion", 25);
        AssertResult.IsFalse(potion.Health < 0);
    }

    [TestMethod]
    public void PotionCollection_IsEmpty()
    {
        List<HealItem> inventory = new List<HealItem>();
        AssertResult.IsEmpty(inventory);
    }

    [TestMethod]
    public void PotionCollection_AddTwoItems_IsNotSingle()
    {
        List<HealItem> inventory = new List<HealItem> 
        { 
            new Potion("P1", 10), 
            new Potion("P2", 20) 
        };
        AssertResult.NotSingle(inventory);
    }

    [TestMethod]
    public void FailedLogic_CatchCustomFailMessage()
    {
        try
        {
            HealItem potion = new Potion("P1", 10);
            if (potion.Health == 10)
            {
                AssertResult.Fail("Testing the Fail method manually");
            }
        }
        catch (UnitTestLib.Exceptions.AssertFailedException ex)
        {
            AssertResult.IsNotNull(ex);
            AssertResult.IsTrue(ex.Message.Contains("Testing the Fail method"));
        }
    }
}
