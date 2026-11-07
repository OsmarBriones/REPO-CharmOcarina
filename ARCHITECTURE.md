# CharmOcarina Architecture

A Charm mod (layer 3 of the PocketCharms family): one valuable, one power. It depends on **Pockets** (storage) and **CharmCore** (registry, wear, cooldown display, host settings, debug mode).

## The power: Song of Power

While a player carries the Ocarina in a pocket, the Sword that player wields deals `DamageMultiplier` times its damage (default 3) to monsters. Each boosted hit that landed wears the Ocarina (`WearPercent`, default 8%), at most once per `WearCooldownSeconds` per wielder.

This is a **host effect**: monster damage from a melee `HurtCollider` is applied only on the master (or alone), so the patch runs there and needs no network message. Other machines see the monster lose more health through the game's own sync.

- `OcarinaCharm : Charm` registers the prefab `Valuable Manor Ocarina`, wear 8%, no cooldown, and the host-synced settings below.
- `Patches/HurtCollider_EnemyHurt_Patch` patches `HurtCollider.EnemyHurt(Enemy)`. The prefix calls `SongofPower.Begin`, which may raise `enemyDamage` on that collider and stores the original in `__state`. A **finalizer** (runs even if the game code throws) calls `SongofPower.End`, which always restores `enemyDamage`, then bills the wear when the call returned true.
- `SongofPower` (static) holds the logic: sword detection, wielder lookup, carrier check, wear billing. The charm instance is set by the plugin in `Awake`.
- `Domain/DamageBoost.cs` is game-free: `Multiply` (clamp 1..10, round half up, never below the original, overflow-safe) and `CanBill` (the wear pause). Unit tested in `DamageBoostTests`.

### Flow of a hit
1. `HurtCollider.ColliderCheck` finds a monster and (through `EnemyHurtRigidbody` or directly) calls `EnemyHurt`, only on the master. All melee-vs-monster damage passes through `EnemyHurt`, which applies `enemyDamage` with `Health.Hurt`.
2. The prefix skips unless: master/alone, charm enabled, `enemyDamage > 0`, the monster has health, the collider is not a death pit or instant kill.
3. The collider must belong to an `ItemMelee` (searched in the parents) that is the Sword: `ItemAttributes.item.itemName == "Sword"`, or else the object name starts with `Item Melee Sword`.
4. Wielder: the first player in `ItemMelee.physGrabObject.playerGrabbing` who carries the Ocarina (`CharmsApi.ForEachCarrier`, which also honours CharmCore's switch and skips dead players). If nobody holds the Sword (thrown), `HurtCollider.playerCausingHurt`, else `PhysGrabObject.lastPlayerGrabbing`.
5. `enemyDamage` becomes `Multiply(original, DamageMultiplier)`; the game applies it; the finalizer restores the original.
6. If the hit landed and `CanBill` allows it, `Charm.WearValuable(ocarina, WearPercent)`.

### Settings
| Key | Type | Range | Default | Meaning |
|---|---|---|---|---|
| `DamageMultiplier` | float | 1 to 10 | 3 | Sword damage factor. 1 means no boost; below 1 is excluded so the charm never weakens the Sword. |
| `WearCooldownSeconds` | float | 0 to 5 | 0.4 | Pause between two wear charges of the same wielder. 0 wears on every boosted hit. |
| `WearPercent` (CharmCore) | float | 0 to 100 | 8 | Wear per billed hit. |

## Game members used

- `HurtCollider.EnemyHurt(Enemy)` (private; patched), `enemyDamage` (read, written, restored), `enemyKill`, `deathPit`, `playerCausingHurt`
- `Enemy.HasHealth`
- `ItemMelee.physGrabObject` (private), found with `GetComponentInParent<ItemMelee>()` from the collider
- `ItemAttributes.item`, `Item.itemName`
- `PhysGrabObject.playerGrabbing`, `PhysGrabObject.lastPlayerGrabbing`, `PhysGrabber.playerAvatar`
- `SemiFunc.IsMasterClientOrSingleplayer()`
- `ValuableObject.dollarValueCurrent` (wear, done by CharmCore)

All are checked by `CharmOcarina.Tests/GameContractTests.cs`.

## Debug mode

Provided by CharmCore: the `Debug.DebugMode` setting (off by default) places the valuable on the floor at the start of each level and enables `Charm.LogDebug` output. This charm logs each boosted hit (original and boosted damage, wielder, wear billed or skipped).

## Known limits

- Looked up by exact prefab name; a game update that renames the prefab needs a mod update.
- Only the Sword is boosted (not other melee weapons) and only damage to monsters, not to players.
- A thrown Sword still counts for the player who threw it (`playerCausingHurt` / last grabber).
- If the Ocarina was destroyed between the prefix and the finalizer the wear is silently skipped.
- **Not verified in game** (only the build, unit tests and game-member existence checks ran): that `playerCausingHurt` is filled (the code does not depend on it while the Sword is held, it uses the grabber list); that the finalizer binding with `bool __result` and `__state` works on the installed Harmony; the real damage numbers and the behaviour in multiplayer; that monsters without health are ignored as expected.

## Decisions

- **Hook choice:** `EnemyHurt` rather than `Health.Hurt` or `ItemMelee.EnemySwingHit`: it is the single place where the collider's `enemyDamage` is applied, runs only on the host, and knows the collider (so the Sword and its wielder). `EnemySwingHit` was not used because nothing in the decompiled code calls it directly (it is probably wired through a prefab event) and it does not say which hit it was.
- **Temporary field change** (prefix raises, finalizer restores) instead of a transpiler: simpler, and a failed hit can never leave a permanent change.
- **Wear per landed hit, with a pause:** one wear per boosted hit as asked; because one swing can hit several times, `WearCooldownSeconds` (per wielder) limits the billing.
- **No wear when nothing changed:** a multiplier of 1 skips the boost and the wear.
- **Wielder rule:** the current grabber wins; the "blamed" player is only a fallback for a Sword nobody holds.
- **Parameter binding:** the monster is read as `__0` so a renamed parameter cannot break the patch; the contract test checks the method name and parameter count.
- **RepoAPI reuse gate:** nothing here is generic enough for RepoAPI (all of it depends on this charm's settings and the Sword); no promotion candidate.
