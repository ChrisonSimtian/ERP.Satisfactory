# `etothepii-test` — Satisfactory save-game parser sidecar (PoC)

Wraps the [`@etothepii/satisfactory-file-parser`](https://github.com/etothepii4/satisfactory-file-parser)
TypeScript parser as a CLI. The chosen parser path per
[ADR 0012](../../docs/adr/0012-live-factory-state-via-node-sidecar.md);
this folder is the proof-of-concept that the ADR turns into an Aspire-hosted
Node sidecar later.

## Scripts

| Script | Purpose |
| --- | --- |
| `test.mjs` | Original PoC. Dumps header, top-20 class counts, miner positions. Referenced by ADR 0012. |
| `stocktake.mjs` | **Sync tool for `.satisfactory/stocktake.md`.** Counts miners (by resource), producers (by recipe), generators (by fuel), water/oil extractors, and belt/lift/pipe tiers. Markdown by default, `--json` for raw. |

## Usage

```powershell
# Auto-detect latest save under the default save dir
node stocktake.mjs

# Explicit save file
node stocktake.mjs "C:\Users\ChrisSimon\AppData\Local\FactoryGame\Saved\SaveGames\76561198103946376\Beta Game_autosave_2.sav"

# JSON output
node stocktake.mjs --json
```

Default save dir is hard-coded to Chris's Steam ID for now; override with
`ERP_SATISFACTORY_SAVE_DIR` (matches the env-var pattern in ADR 0012).

## Why a separate folder

The eventual sidecar lives elsewhere — this folder is throwaway PoC scaffolding
that proved the parser works on v1.2 saves (SaveVersion 60, BuildVersion 489969).
Keep it small. New tooling lives in the proper sidecar once ADR 0012 lands.
