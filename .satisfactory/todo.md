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

### 3. Iron ore extraction shortfall — UPSTREAM BLOCKER (improved, still blocker)
- **Severity:** `BLOCKER` (smelter count now resolved — this is the sole upstream blocker).
- **Source:** 2026-05-11 stocktake (save-file sync — Site C added)
- **Detail:** Production line demands **`450 Iron Ore/min`** (`15`-smelter full
  target). Current supply: **`180 Iron Ore/min`** from `6 × MK1` miners on Impure
  nodes across `3` sites, all belted directly into the basement smelting bay:
  - Site A (loose): `2 × MK1 Impure = 60 ore/min`
  - Site B (former minehead): `2 × MK1 Impure = 60 ore/min`
  - Site C (new): `2 × MK1 Impure = 60 ore/min` (purity assumed — confirm next session)

  **Shortfall: `270/min` (~`60%`).** Combined ingot output: `180/min`.
  Production line effective throughput: `~40%`. Site C narrowed the gap by
  `60 ore/min` vs the previous snapshot.
- **Fix:** Pick one or combine —
  - **Confirm Site C purity.** If any of the two new nodes are Normal, supply
    jumps `+30/min` per node — cheap freebie.
  - **Survey for higher-purity iron nodes** elsewhere. A pair of MK1 miners on
    Normal nodes adds `120/min`; on Pure adds `240/min`.
  - **Buy Tier 4 milestone → MK2 miners.** MK2 on the existing `6` Impure nodes
    doubles supply to `360 ore/min` (still short `90/min`, but a big jump).
    Gated on Encased Industrial Beam (Steel).
  - **Scale the production line back** to match `180 Iron Ore/min` until ore is
    solved — `6` smelters' worth of everything downstream.

---

### 5. Iron belt breach at base — ACTIVE NOW (was: pending MK2 upgrade)
- **Severity:** `BLOCKER` (escalated from `RISK` — Site C pushed merged iron belt past MK2 cap today)
- **Source:** 2026-05-11 stocktake (save-file sync — Site C added)
- **Detail:** Merged Iron belt at base now carries `180 ore/min` (Site A `60` +
  Site B `60` + Site C `60`). MK2 belt cap is `120/min`. **Belt overflows by
  `60/min` today** — Site C's contribution is being lost at the merge or
  backing up upstream. Copper merged belt unchanged (`120/min`, at cap).
  Post-MK2-miner-upgrade projection is worse: `360 iron/min` and `240 copper/min`.
- **Fix:** Pick one immediately —
  - **Split the iron merge into `2 × MK2` belts** into the smelter bay (one
    carrying `120/min`, the other `60/min`) — zero-milestone fix, works today.
  - **Unlock MK3 belts** (Tier 4 logistics — already unlocked by Phase 1) and
    swap the merged belt to a single MK3 (`270/min` cap) — clean and
    future-proof through the next miner upgrade as well.

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

### 1. Iron smelter count gap — resolved 2026-05-11 (save-file sync)
- **Resolution:** Save-file confirms `16` Iron Smelters built at the mega factory
  base (`4 × 4`), not `8`. At the `450 ingot/min` production-line target only
  `15` are required — **`+1` smelter surplus, sized for headroom.** Smelter
  count is no longer a constraint; iron ore (TODO #3) remains the upstream
  blocker.
- **Note:** At today's `180 ore/min` supply, only `~6` of the `16` smelters are
  actively fed (`~37.5%` bay throughput); the rest sit idle until ore scales.
