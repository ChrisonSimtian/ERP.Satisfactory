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

### 1. Iron smelter count gap (dormant, ore-blocked)
- **Severity:** `DEGRADED` (was `RISK` — gap widened by 3rd screw module's ingot demand)
- **Source:** 2026-05-11 stocktake (post-centralization + 3rd screw module)
- **Detail:** Iron production line full target = **`450 Iron Ingot/min`** (was
  `420` — `+30` from the 3rd screw module's internal rod) → needs **`15`** smelters.
  Current iron smelter inventory: **`8` total** (all at the mega factory base, in
  `2` modules of `4`). **Short `7` smelters.** Dormant until ore is solved — at
  the current `120 ore/min` supply, only `4` smelters are even active.
- **Fix:** Once ore is solved (TODO #3), add `7` more Iron Smelters (`~2` more
  modules) and verify ingot flow reaches all production-line modules. Until ore
  is solved, this TODO is effectively dormant.

### 3. Iron ore extraction shortfall — UPSTREAM BLOCKER (WORSE)
- **Severity:** `BLOCKER` (this remains the real upstream blocker — TODO #1
  is parked behind this).
- **Source:** 2026-05-11 stocktake (revised after 3rd screw module added)
- **Detail:** Production line now demands **`450 Iron Ore/min`** (`15`-smelter
  full target — `+30` from the 3rd screw module's internal rod). Current supply:
  `120 Iron Ore/min` from `4 × MK1` miners on Impure nodes across 2 sites, both
  now belted directly into the basement smelting bay:
  - Site A (loose): `60 ore/min` → merged at base on MK2 belt.
  - Site B (former minehead): `60 ore/min` → merged at base on MK2 belt.

  **Shortfall: `330/min` (~`73%`).** Combined ingot output: `120/min`.
  Production line effective throughput: `~27%`. The 3rd screw module widened
  the gap by `30/min` of ingot demand (`+30 ore/min`).
- **Fix:** Pick one or combine —
  - **Survey for higher-purity iron nodes.** A pair of MK1 miners on Normal
    nodes adds `120/min`; on Pure adds `240/min`. Cheapest immediate fix.
  - **Buy Tier 4 milestone → MK2 miners.** Phase 1 already unlocked Tier 4.
    MK2 on the existing `4` Impure nodes doubles supply to `240 ore/min`
    (still short `210/min`, but unlocks the path). Milestone cost requires
    Encased Industrial Beam — gated on starting Steel.
  - **Scale the production line back** to match `120 Iron Ore/min` until ore is
    solved — `4` smelters' worth of everything downstream.

---

### 5. Belt breach pending MK2 miner upgrade
- **Severity:** `RISK`
- **Source:** 2026-05-11 centralized smelting plan
- **Detail:** Post-MK2 miners (Tier 4 milestone), the merged Iron belt at base
  carries `240/min` and the merged Copper belt carries `240/min`. Both exceed
  MK2 belt cap (`120/min`).
- **Fix:** Unlock **MK3 belts** (Tier 4 logistics milestone — already unlocked
  by Phase 1) *before* the MK2 miner swap, or run `2 × MK2` belts per ore into
  the basement.

### 6. Coal headroom insufficient for steel module
- **Severity:** `RISK` (becomes `BLOCKER` the moment steel is built)
- **Source:** 2026-05-11 steel-pivot analysis
- **Detail:** A minimum-viable steel module for `1 Modular Engine/min`
  (`30 Steel Pipe/min` → `45 Steel Ingot/min`) needs `45 coal/min`. Current
  spare: `30/min`. **Short `15 coal/min`** when steel lands.
- **Fix:** Add a 4th coal generator pair (`+150 MW`, `+30 coal demand`,
  `+90 m³ water demand` — water has exactly `90/min` spare, perfect fit) **before**
  lighting any Steel Foundries. Or pivot to a Steel-friendly alt-coal source.

---

## Resolved

### 2. Screw factory at 100% utilisation — resolved 2026-05-11
- **Resolution:** Built a 3rd screw module (`+120 screw/min`). Total screw
  output now `360/min`. RIP still consumes `240/min`; **`120/min` free for
  Rotor** (`1 Rotor Assembler` at `4 Rotor/min` needs `100 screw/min` → `20/min`
  spare margin).
- **Trade-off taken:** `+30 iron ingot/min` internal-rod demand widened the ore
  shortfall (TODO #3) by `30 ore/min`.

### 4. Copper smelters at Impure site ore-starved — resolved 2026-05-11
- **Resolution:** Dismantled all minehead copper smelters. Built `1` copper
  smelter module (`4` Smelters) in the mega factory basement. All three copper
  miners (`1` Normal + `2` Impure) belted into one MK2 line (`120/min`,
  saturated) feeding the module at `100%` throughput. Output: `120 Copper
  Ingot/min`. No idle smelters, no wasted power.
