using System;

namespace CharmOcarina.Domain
{
	/// <summary>Game-free rules of Song of Power: how a sword hit is multiplied and when it is billed. Kept free of Unity and game types so it is unit tested.</summary>
	internal static class DamageBoost
	{
		/// <summary>Largest multiplier the maths accepts (matches the upper limit of the setting).</summary>
		internal const float MaxMultiplier = 10f;

		/// <summary>
		/// The boosted damage. The multiplier is clamped to 1..<see cref="MaxMultiplier"/> (a non-number counts as 1), the
		/// product is rounded to the nearest whole point (halves go up), is never below the original damage and never
		/// overflows an int. Damage of 0 or less is returned unchanged.
		/// </summary>
		internal static int Multiply(int damage, float multiplier)
		{
			if (damage <= 0)
			{
				return damage;
			}

			double factor = float.IsNaN(multiplier) ? 1d : multiplier < 1f ? 1d : multiplier > MaxMultiplier ? MaxMultiplier : multiplier;
			double boosted = Math.Round(damage * factor, MidpointRounding.AwayFromZero);
			if (boosted >= int.MaxValue)
			{
				return int.MaxValue;
			}

			int result = (int)boosted;
			return result < damage ? damage : result;
		}

		/// <summary>
		/// True when a boosted hit may be billed to the valuable now: the pause since the last billed hit (<paramref name="lastBilledAt"/>,
		/// use a very negative number for "never") has reached <paramref name="cooldownSeconds"/>. A cooldown of 0 or less bills every hit.
		/// If the clock went backwards (a new level restarts it) the hit is billed.
		/// </summary>
		internal static bool CanBill(float now, float lastBilledAt, float cooldownSeconds)
		{
			if (cooldownSeconds <= 0f || now < lastBilledAt)
			{
				return true;
			}

			return now - lastBilledAt >= cooldownSeconds;
		}
	}
}
