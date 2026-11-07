# Changelog

## [Unreleased]

### Fixed
- Runtime hygiene: the wear records are cleared when a level ends, and the debug log line in the hit patch is only built in debug mode.

### Added
- Song of Power: the Sword deals triple damage (`DamageMultiplier`, 1 to 10) to monsters while its wielder carries the Ocarina; boosted hits wear the Ocarina, at most once per `WearCooldownSeconds`.
- `Debug.DebugMode` (off by default): puts the Ocarina on the floor at the start of each level and writes detailed logs.
