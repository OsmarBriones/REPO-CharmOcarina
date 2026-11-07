# QA battery: Song of Power (CharmOcarina)

Deploy: Pockets, CharmCore and CharmOcarina are in the r2modman `Debug` profile (compare hashes with the builds). Restart the game first.

## Quick start with debug mode
Set `DebugMode = true` in the `Debug` section of `BepInEx/config/com.osmar.CharmOcarina.cfg` (off by default). When a level starts, a Ocarina is placed on the floor about 2.5 m in front of you; the log then also shows the detailed lines. Any level works.

## Single player
- [ ] Log shows `Charm registered: Ocarina - Song of Power` and `CharmOcarina 1.0.0 loaded!`.
- [ ] Store the valuable in a slot: the slot shows its picture and value.
- [ ] Without the Ocarina in a pocket, count the hits a monster needs with the Sword (it deals 50 per hit).
- [ ] Store the Ocarina, hit the same kind of monster: it dies in about a third of the hits (triple damage).
- [ ] The Ocarina's value drops by about `WearPercent` (8%) on a hit; a multi-hit swing costs only one step per 0.4 s. There is no cooldown veil on the slot (this charm has no cooldown).
- [ ] Hit with another weapon or throw objects: damage is normal, no wear.
- [ ] Take the Ocarina out mid-fight: the next Sword hit is normal again.
- [ ] With `DebugMode` the log shows `Song of Power: Sword hit boosted 50 -> 150`.
- [ ] Sword damage is normal again in the next fight (no permanent change to the Sword).
- [ ] Take the valuable out: the power stops.
- [ ] Use it until the valuable breaks: the slot empties, the power stops, no errors in the log.

## Settings
- [ ] `DamageMultiplier = 1`: no boost, no wear. `10`: ten times damage. `WearCooldownSeconds = 0`: every hit wears; `5`: one wear per 5 s. `WearPercent = 0`: no wear.
- [ ] Charm `Enabled = false`: no effect and no wear. CharmCore `Enabled = false`: nothing works.

## Multiplayer (host and client)
- [ ] Host carrying the Ocarina swings the Sword: boosted. Client carrying it swings the Sword: boosted, and the Ocarina value drops on both machines.
- [ ] A player without the Ocarina swinging the Sword while another player carries one: no boost.
- [ ] Host settings override the client's.
- [ ] Client leaves and rejoins: no stuck veil.

Log patterns: `Charm registered`, `threw`, `Exception`.
