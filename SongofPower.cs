using System;
using CharmCore.Api;
using CharmOcarina.Domain;
using UnityEngine;

namespace CharmOcarina
{
	/// <summary>
	/// Song of Power: while a player carries the Ocarina in a pocket, the Sword that player wields deals
	/// <c>DamageMultiplier</c> times its damage to monsters. This is a HOST effect: monster damage from a melee
	/// <c>HurtCollider</c> is applied only on the master (or alone), so everything here runs there and no network
	/// message is needed. <c>Patches.HurtCollider_EnemyHurt_Patch</c> calls <see cref="Begin"/> before the
	/// game applies a hit and <see cref="End"/> afterwards.
	/// </summary>
	internal static class SongofPower
	{
		/// <summary>The in-game item name of the Sword (<c>Item.itemName</c>).</summary>
		internal const string SwordItemName = "Sword";

		/// <summary>The Sword's prefab name; spawned instances carry it with a "(Clone)" suffix.</summary>
		internal const string SwordPrefabName = "Item Melee Sword";

		/// <summary>The temporary change made for one hit, so it can be undone exactly.</summary>
		internal sealed class Boost
		{
			internal Boost(int originalDamage, int boostedDamage, PlayerAvatar wielder, ValuableObject ocarina)
			{
				OriginalDamage = originalDamage;
				BoostedDamage = boostedDamage;
				Wielder = wielder;
				Ocarina = ocarina;
			}

			internal int OriginalDamage { get; }

			internal int BoostedDamage { get; }

			internal PlayerAvatar Wielder { get; }

			internal ValuableObject Ocarina { get; }
		}

		internal static OcarinaCharm? Charm { get; set; }

		/// <summary>
		/// Called before the game applies a melee hit to a monster. When the hit comes from a Sword wielded by a carrier of the
		/// Ocarina it raises the collider's <c>enemyDamage</c> and returns what is needed to undo it; otherwise it changes nothing and returns null.
		/// </summary>
		internal static Boost? Begin(HurtCollider collider, Enemy? enemy)
		{
			OcarinaCharm? charm = Charm;
			if (charm == null || collider == null || enemy == null || !SemiFunc.IsMasterClientOrSingleplayer() || !charm.IsEnabled)
			{
				return null;
			}

			// Hits that do not subtract health (no damage, a monster without health, kill zones) are not Sword hits worth boosting.
			int original = collider.enemyDamage;
			if (original <= 0 || !enemy.HasHealth || collider.deathPit || collider.enemyKill)
			{
				return null;
			}

			ItemMelee? melee = collider.GetComponentInParent<ItemMelee>();
			if (melee == null || !IsSword(melee))
			{
				return null;
			}

			if (!TryFindWielder(melee, collider, charm, out PlayerAvatar? wielder, out ValuableObject? ocarina) || wielder == null || ocarina == null)
			{
				return null;
			}

			int boosted = DamageBoost.Multiply(original, charm.DamageMultiplier.Value);
			if (boosted <= original)
			{
				return null;
			}

			collider.enemyDamage = boosted;
			return new Boost(original, boosted, wielder, ocarina);
		}

		/// <summary>
		/// Called after the game handled the hit, even when it threw: puts the original damage back so the value never drifts, and
		/// wears the Ocarina when the hit really landed (at most once per <c>WearCooldownSeconds</c> per wielder).
		/// </summary>
		internal static void End(HurtCollider collider, Boost? boost, bool hitLanded)
		{
			if (boost == null)
			{
				return;
			}

			if (collider != null)
			{
				collider.enemyDamage = boost.OriginalDamage;
			}

			OcarinaCharm? charm = Charm;
			if (charm == null || !hitLanded)
			{
				return;
			}

			int key = boost.Wielder.GetInstanceID();
			float now = Time.time;
			float last = charm.LastBilled.TryGetValue(key, out float stored) ? stored : float.MinValue;
			bool bill = DamageBoost.CanBill(now, last, charm.WearCooldownSeconds.Value);
			if (bill)
			{
				charm.LastBilled[key] = now;
				charm.WearValuable(boost.Ocarina, charm.WearPercent);
			}

			if (charm.DebugMode)
			{
				charm.LogDebug($"Song of Power: Sword hit boosted {boost.OriginalDamage} -> {boost.BoostedDamage} (wielder {boost.Wielder.name}); wear {(bill ? charm.WearPercent.ToString("0.#") + "%" : "skipped (cooldown)")}");
			}
		}

		/// <summary>True for the Sword: by item name when the item data is there, otherwise by the prefab name of the object.</summary>
		internal static bool IsSword(ItemMelee melee)
		{
			ItemAttributes? attributes = melee.GetComponent<ItemAttributes>();
			if (attributes != null && attributes.item != null && string.Equals(attributes.item.itemName, SwordItemName, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}

			return melee.gameObject.name.StartsWith(SwordPrefabName, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Finds who swings the Sword: the player grabbing it that carries the Ocarina; when nobody holds it (a thrown Sword) the
		/// player the game blames for the hit (<c>playerCausingHurt</c>, else the last player who grabbed it).
		/// </summary>
		private static bool TryFindWielder(ItemMelee melee, HurtCollider collider, OcarinaCharm charm, out PlayerAvatar? wielder, out ValuableObject? ocarina)
		{
			wielder = null;
			ocarina = null;
			PhysGrabObject? grab = melee.physGrabObject;
			bool held = false;
			if (grab != null)
			{
				foreach (PhysGrabber grabber in grab.playerGrabbing.ToArray())
				{
					held = true;
					if (grabber != null && grabber.playerAvatar != null && TryGetOcarina(grabber.playerAvatar, charm, out ocarina))
					{
						wielder = grabber.playerAvatar;
						return true;
					}
				}
			}

			if (held)
			{
				return false;
			}

			PlayerAvatar? blamed = collider.playerCausingHurt != null ? collider.playerCausingHurt : grab != null ? grab.lastPlayerGrabbing : null;
			if (blamed != null && TryGetOcarina(blamed, charm, out ocarina))
			{
				wielder = blamed;
				return true;
			}

			return false;
		}

		/// <summary>The Ocarina this player carries, if any (honours CharmCore's own switch, the charm's switch and dead players).</summary>
		private static bool TryGetOcarina(PlayerAvatar avatar, OcarinaCharm charm, out ValuableObject? ocarina)
		{
			ValuableObject? found = null;
			CharmsApi.ForEachCarrier(charm, (carrier, valuable) =>
			{
				if (carrier == avatar)
				{
					found = valuable;
				}
			});

			ocarina = found;
			return found != null;
		}
	}
}
