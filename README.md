# Charm Ocarina

Song of Power: the Sword deals triple damage.

> **Everyone in the lobby must install this mod, together with Pockets and CharmCore.** The power changes what your own character can do and its state shows on your inventory slot, so friends without the mods would see an empty slot and be unable to use the pocket.

## How to Use

1. Find the **Ocarina** valuable and store it in a free inventory slot with [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/) (grab it, press the slot key).
2. Pick up the **Sword** and fight monsters. Every hit you land with it deals triple damage while the Ocarina is in your pocket.
3. Take the valuable out of the pocket and the power stops.

## Features

- **Song of Power:** the Sword you swing deals triple damage to monsters while you carry the Ocarina. Other players' swords are not affected unless they carry an Ocarina too.
- **It costs the valuable:** landing boosted hits wears away a share of the Ocarina's value (one charge per swing, even if it hits several times), and a worn-out Ocarina breaks.
- **Everything is configurable:** how strong the boost is, the wear, and the pause between wear charges. The host's choices apply to everyone.

## Requirements
- [BepInEx Pack for R.E.P.O.](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/)
- [Pockets](https://thunderstore.io/c/repo/p/OsmarBriones/Pockets/)
- [CharmCore](https://thunderstore.io/c/repo/p/OsmarBriones/CharmCore/)

## Installation
1. Install through r2modman / Thunderstore Mod Manager (Pockets and CharmCore come with it), or place `CharmOcarina.dll` next to `Pockets.dll` and `CharmCore.dll` in `BepInEx/plugins`. **All players need it.**
2. Launch the game once; the configuration file is created inside `BepInEx/config`.

## Configuration
Settings are in `com.osmar.CharmOcarina.cfg` inside `BepInEx/config`, section `Charm.Ocarina`. The host's choices apply to everyone.

- `Enabled` (default `true`): turn this charm on or off.
- `WearPercent` (default `8`): share of the valuable's original value lost on each billed boosted hit.
- `DamageMultiplier` (default `3`, from 1 to 10): how many times more damage the Sword deals to monsters.
- `WearCooldownSeconds` (default `0.4`, from 0 to 5): pause between two wear charges for the same player, so one swing that hits several times costs only one. 0 wears on every boosted hit.

Section `Debug`: `DebugMode` (default `false`) places a Ocarina on the floor in front of you when a level starts (host or singleplayer) and writes extra log lines. For testing.

## Issues & Bug Reports
The official way to report issues, suggest improvements, or submit feedback is by opening an issue on the official GitHub repository:
👉 [GitHub Issues](https://github.com/OsmarBriones/REPO-CharmOcarina/issues)

## Credits
Developed by **Osmar Briones**
