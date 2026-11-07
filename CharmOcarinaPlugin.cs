using BepInEx;
using CharmCore.Api;
using HarmonyLib;
using UnityEngine;

namespace CharmOcarina
{
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	[BepInDependency("com.osmar.Pockets", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("com.osmar.CharmCore", BepInDependency.DependencyFlags.HardDependency)]
	public class CharmOcarinaPlugin : BaseUnityPlugin
	{
		public const string PluginGuid = "com.osmar.CharmOcarina";
		public const string PluginName = "CharmOcarina";
		public const string PluginVersion = "1.0.0";

		internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; } = null!;

		private void Awake()
		{
			Logger = base.Logger;

			// Prevent the plugin from being deleted
			this.gameObject.transform.parent = null;
			this.gameObject.hideFlags = HideFlags.HideAndDontSave;

			// Registering binds Enabled, WearPercent, CooldownSeconds and the Debug section (DebugMode) in this mod's config file.
			OcarinaCharm charm = new OcarinaCharm();
			CharmsApi.Register(charm, Config);
			SongofPower.Charm = charm;
			new Harmony(PluginGuid).PatchAll();

			Logger.LogInfo($"{PluginName} {PluginVersion} loaded!");
		}
	}
}
