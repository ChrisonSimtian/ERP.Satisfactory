# Satisfactory Priority TODO

ASAP-fix list. Items land here when ADA's math shows a resource breach —
power, ingots, ore, belts, water, miner extraction, anything saturated or
short. Resolve top-down before stacking new modules on top of broken
foundations.

**Format per item:**

```
### <one-line title>
- Severity: BLOCKER | DEGRADED | RISK
- Source:   <stocktake date / module name>
- Detail:   <numbers + what's saturated>
- Fix:      <what to build / change>
```

**Severity guide:**

- `BLOCKER` — the build doesn't work / a downstream module is starved.
- `DEGRADED` — runs but underclocked, capped, or wasting capacity.
- `RISK` — fine today, will break at the next phase / scale-up.

---

## Open

### 1. Iron smelter shortfall (now near-resolved on count, ore-blocked)
- **Severity:** `RISK` (was `BLOCKER` — count is close, ore is the real issue)
- **Source:** 2026-05-10 stocktake (revised after base smelters added)
- **Detail:** Iron production line full target = `420 Iron Ingot/min` →
  needs `14` smelters. Current iron smelter inventory: **`12` total**
  (`4` at impure-iron minehead + `8` at mega factory base). Only **`2` smelters
  short** of the full-target count. At full ore feed, `12` smelters would
  produce `360 Iron Ingot/min` (covers the `240/min` plate-factory demand
  comfortably). **Real bottleneck is ore (TODO #3) — not smelter count.**
- **Fix:** Once ore is solved, add `2` more Iron Smelters and verify ingot
  flow reaches all production-line modules. Until ore is solved, this TODO is
  effectively dormant.

### 2. Screw factory at 100% utilisation
- **Severity:** `BLOCKER` (for Rotor)
- **Source:** 2026-05-10 stocktake
- **Detail:** RIP consumes the entire `240 screw/min` output. Zero screws
  available for Rotor when built (Rotor needs `100 screw/min` per Assembler
  for `4 Rotor/min`).
- **Fix:** Pick one —
  - Add a 3rd screw module (`+120 screw/min`).
  - Scale RIP back to `10 RIP/min` (`2` Assemblers, frees `120 screw/min`).
  - Unlock **Cast Screw** alt recipe (Iron Ingot → Screw direct, much
    screw-cheaper, skips rods).

### 3. Iron ore extraction shortfall — UPSTREAM BLOCKER
- **Severity:** `BLOCKER` (this is the real upstream blocker — TODO #1 is
  parked behind this).
- **Source:** 2026-05-10 stocktake (miner inventory, revised after Site A/B
  separation)
- **Detail:** Production line demands `420 Iron Ore/min` (`14`-smelter full
  target). Current supply: `120 Iron Ore/min` from `4 × MK1` miners on Impure
  nodes across 2 sites:
  - Site A (loose): `60 ore/min` belted to mega factory base, feeds `8` base
    smelters at `25%` throughput.
  - Site B (minehead): `60 ore/min` feeds `4` minehead smelters at `50%`
    throughput.
  **Shortfall: `300/min` (~`71%`).** Combined ingot output: `120/min`.
  Production line effective throughput: `~29%`.
- **Fix:** Pick one or combine —
  - **Survey for higher-purity iron nodes.** A pair of MK1 miners on Normal
    nodes adds `120/min`; on Pure adds `240/min`. Cheapest immediate fix.
  - **Push to Tier 4 → MK2 miners.** MK2 on Impure = `60/min` each (already 2×
    MK1), Normal = `120/min`, Pure = `240/min`.
  - **Scale the production line back** to match `120 Iron Ore/min` until ore is
    solved — `4` smelters' worth of everything downstream.

---

### 4. Copper smelters at Impure site ore-starved
- **Severity:** `DEGRADED`
- **Source:** 2026-05-10 stocktake
- **Detail:** `4` Copper Smelters at the impure-copper minehead are fed by
  `2 × MK1` miners on Impure nodes producing `60 Copper Ore/min`. Smelters need
  `120/min` for full → **`50%` throughput**, output `60 Copper Ingot/min`. Two
  smelters' worth of capacity is idle, burning `~8 MW` for nothing. Same starvation
  pattern as the iron smelters at the impure-iron site.
- **Fix:** Pick one —
  - **Decommission `2` Smelters.** Cleanest. Saves `~8 MW`, output unchanged.
  - **Belt in the Normal copper site's `60 ore/min`.** Total feed becomes `120/min`,
    smelters run at `100%`, output doubles to `120 Copper Ingot/min`. Cost: a
    long belt run between sites.
  - **Add more impure copper miners** (only if more impure nodes exist within
    range — check the local survey).

---

## Resolved

*(none yet — move items here with a date stamp when fixed,
 e.g. `### 1. Iron smelter shortfall — resolved 2026-05-15`)*
