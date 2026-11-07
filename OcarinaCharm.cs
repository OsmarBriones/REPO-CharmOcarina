using System.Collections.Generic;
using CharmCore.Api;

namespace CharmOcarina
{
	/// <summary>Song of Power: the Sword deals triple damage to monsters while the Ocarina is in a pocket.</summary>
	internal sealed class OcarinaCharm : Charm
	{
		internal const string PrefabName = "Valuable Manor Ocarina";

		internal OcarinaCharm()
			: base("Ocarina", "Ocarina - Song of Power", defaultWearPercent: 8f, defaultCooldownSeconds: null, PrefabName)
		{
			DamageMultiplier = AddSetting("DamageMultiplier", 3f, 1f, 10f, "How many times more damage the Sword deals to monsters while its wielder carries the Ocarina (3 = triple damage, 1 = no boost).");
			WearCooldownSeconds = AddSetting("WearCooldownSeconds", 0.4f, 0f, 5f, "Pause, in seconds, between two boosted hits that wear the Ocarina, so one swing that hits several times costs only one wear. 0 wears the Ocarina on every boosted hit.");
		}

		/// <summary>When each wielder last paid wear, by avatar instance id (host only). Cleared when the level ends.</summary>
		internal Dictionary<int, float> LastBilled { get; } = new Dictionary<int, float>();

		protected override void OnLevelEnded()
		{
			LastBilled.Clear();
		}

		/// <summary>Damage multiplier for the Sword (1 to 10).</summary>
		internal CharmSetting DamageMultiplier { get; }

		/// <summary>Seconds between two wear charges of the same wielder (0 to 5).</summary>
		internal CharmSetting WearCooldownSeconds { get; }
	}
}
