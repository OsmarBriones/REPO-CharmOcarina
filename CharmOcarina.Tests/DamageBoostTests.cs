using CharmOcarina.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CharmOcarina.Tests;

[TestClass]
public class DamageBoostTests
{
	[DataTestMethod]
	[DataRow(50, 3f, 150)]
	[DataRow(50, 1f, 50)]
	[DataRow(50, 10f, 500)]
	[DataRow(7, 2.5f, 18)]
	[DataRow(3, 1.5f, 5)]
	[DataRow(1, 3f, 3)]
	public void Multiply_RoundsToNearestWholePoint(int damage, float multiplier, int expected)
	{
		Assert.AreEqual(expected, DamageBoost.Multiply(damage, multiplier));
	}

	[DataTestMethod]
	[DataRow(50, 0.5f)]
	[DataRow(50, 0f)]
	[DataRow(50, -4f)]
	[DataRow(50, float.NegativeInfinity)]
	[DataRow(50, float.NaN)]
	public void Multiply_NeverGoesBelowTheOriginal(int damage, float multiplier)
	{
		Assert.AreEqual(damage, DamageBoost.Multiply(damage, multiplier));
	}

	[TestMethod]
	public void Multiply_ClampsHugeMultipliers()
	{
		Assert.AreEqual(500, DamageBoost.Multiply(50, 1000f));
		Assert.AreEqual(500, DamageBoost.Multiply(50, float.PositiveInfinity));
	}

	[TestMethod]
	public void Multiply_DoesNotOverflow()
	{
		Assert.AreEqual(int.MaxValue, DamageBoost.Multiply(int.MaxValue, 10f));
		Assert.AreEqual(int.MaxValue, DamageBoost.Multiply(int.MaxValue / 2, 10f));
		Assert.AreEqual(int.MaxValue, DamageBoost.Multiply(int.MaxValue, 1f));
	}

	[TestMethod]
	public void Multiply_LeavesNonPositiveDamageAlone()
	{
		Assert.AreEqual(0, DamageBoost.Multiply(0, 3f));
		Assert.AreEqual(-5, DamageBoost.Multiply(-5, 3f));
		Assert.AreEqual(int.MinValue, DamageBoost.Multiply(int.MinValue, 3f));
	}

	[TestMethod]
	public void CanBill_FirstHitAlwaysBills()
	{
		Assert.IsTrue(DamageBoost.CanBill(0.1f, -1000f, 0.4f));
	}

	[TestMethod]
	public void CanBill_WaitsForTheCooldown()
	{
		Assert.IsFalse(DamageBoost.CanBill(10.25f, 10f, 0.5f));
		Assert.IsTrue(DamageBoost.CanBill(10.5f, 10f, 0.5f));
		Assert.IsTrue(DamageBoost.CanBill(11f, 10f, 0.5f));
	}

	[TestMethod]
	public void CanBill_ZeroCooldownBillsEveryHit()
	{
		Assert.IsTrue(DamageBoost.CanBill(10f, 10f, 0f));
		Assert.IsTrue(DamageBoost.CanBill(10f, 10f, -1f));
	}

	[TestMethod]
	public void CanBill_ClockGoingBackwardsBills()
	{
		Assert.IsTrue(DamageBoost.CanBill(1f, 500f, 5f));
	}
}
