using CharmCore.Api;

namespace CharmOcarina
{
	/// <summary>Song of Power: the Sword deals triple damage.</summary>
	internal sealed class OcarinaCharm : Charm
	{
		internal const string PrefabName = "Valuable Manor Ocarina";

		internal OcarinaCharm()
			: base("Ocarina", "Ocarina - Song of Power", defaultWearPercent: 8f, defaultCooldownSeconds: null, PrefabName)
		{
			// Extra host-synced numbers for this power go here, for example:
			// Amount = AddSetting("Amount", 30f, 1f, 100f, "Description for the config file.");
		}
	}
}
