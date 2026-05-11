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

### 3. Iron ore extraction shortfall — UPSTREAM BLOCKER (much improved, still blocker)
- **Severity:** `BLOCKER` (closing fast — was `270/min` short, now `90/min`).
- **Source:** 2026-05-11 stocktake (save-file sync — Site D added, +6 MK1 miners)
- **Detail:** Production line demands **`450 Iron Ore/min`** (`15`-smelter full
  target). Current supply: **`360 Iron Ore/min`** from `12 × MK1` miners on
  Impure nodes (assumed) across `4` sites:
  - Site A (loose): `2 × MK1 Impure = 60 ore/min`
  - Site B (former minehead): `2 × MK1 Impure = 60 ore/min`
  - Site C: `2 × MK1 Impure = 60 ore/min`
  - Site D (new cluster Z≈-4400/-4500): `6 × MK1 Impure assumed = 180 ore/min`

  **Shortfall: `90/min` (~`20%`).** Combined ingot output: `360/min`.
  Production line effective throughput: `~80%`. Site D narrowed the gap by
  `180 ore/min`. Smelter bay now at `75%` throughput (4 idle of 16).
- **Fix:** Pick one or combine —
  - **Confirm Site D purities.** Each Normal node closes `+30/min`; three Normals
    zero the shortfall outright. Cheapest possible solve.
  - **Add `3` more MK1 miners** on any Impure iron node to close `+90/min`. Or
    `2 × MK1` if even one ends up Normal.
  - **Buy Tier 4 milestone → MK2 miners.** MK2 on existing `12` Impure nodes
    doubles supply to `720 ore/min` — overshoots target by `+270/min`, opens
    headroom for steel and beyond. Gated on Encased Industrial Beam.
  - **Scale the production line back** to match `360 Iron Ore/min` (~`12`
    smelters' worth of downstream) as an interim.

---

### 5. Iron belt breach at base — PENDING CONFIRMATION (was: ACTIVE NOW)
- **Severity:** `BLOCKER` until topology confirmed (could resolve to nothing).
- **Source:** 2026-05-11 stocktake (save-file sync — Site D added)
- **Detail:** Combined raw iron ore arriving at base is now `360/min` (Site A
  `60` + B `60` + C `60` + D `180`). Single MK2 belt cap `120/min`. **3× over
  cap** on a single lane. However, save shows `+27 MK2` belts and `+4 MK2`
  lifts laid since last snapshot — strong signal Chris ran multi-lane MK2
  from Site D. Topology not yet confirmed.
- **Fix (already in flight?):** Confirm with Chris which is built —
  - `3 × MK2` belts merged at base (`360/min` cap, zero margin) — works, no
    milestone needed.
  - `2 × MK3` belts (`540/min` cap) — clean, future-proofs MK2-miner upgrade.
  - If still single MK2: split immediately, `120/min` is being lost at the merge.
  Post-MK2-miner-upgrade projection: `720/min` iron — would need `≥ 3 × MK3`
  or `2 × MK4` lanes.

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
