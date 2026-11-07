using System;
using HarmonyLib;

namespace CharmOcarina.Patches
{
	/// <summary>
	/// Song of Power's only hook. <c>HurtCollider.EnemyHurt(Enemy)</c> applies <c>enemyDamage</c> to a monster; the prefix raises it for a
	/// Sword wielded by an Ocarina carrier and the finalizer always restores it (also when the game code threw).
	/// The callers of <c>EnemyHurt</c> only run on the master (or alone), so this runs on the host only.
	/// The monster is read as parameter 0 (<c>__0</c>) so a renamed parameter cannot break the binding.
	/// </summary>
	[HarmonyPatch(typeof(HurtCollider), "EnemyHurt")]
	internal static class HurtCollider_EnemyHurt_Patch
	{
		[HarmonyPrefix]
		private static void Prefix(HurtCollider __instance, Enemy __0, out SongofPower.Boost? __state)
		{
			__state = null;
			try
			{
				__state = SongofPower.Begin(__instance, __0);
			}
			catch (Exception exception)
			{
				CharmOcarinaPlugin.Logger.LogError($"Song of Power could not boost a hit: {exception}");
			}
		}

		[HarmonyFinalizer]
		private static void Finalizer(HurtCollider __instance, SongofPower.Boost? __state, bool __result, Exception? __exception)
		{
			try
			{
				SongofPower.End(__instance, __state, __exception == null && __result);
			}
			catch (Exception exception)
			{
				CharmOcarinaPlugin.Logger.LogError($"Song of Power could not finish a hit: {exception}");
			}
		}
	}
}
