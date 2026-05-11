# Satisfactory Stocktake

Living record of factory state in Chris's Satisfactory v1.2 save. Update whenever
modules are added, relocated, or scaled. Latest snapshot at the top — older
snapshots stay below for progression tracking.

---

## 2026-05-11 — Save-file sync (Site D / iron-cluster expansion)

- **Tier:** 3/4
- **Phase:** working toward Phase 3 of the Space Elevator
- **Belt tier:** MK2 (MK3 pending Tier 4 logistics milestone)
- **Module philosophy:** sub-factories per product, copy-paste to scale
- **Site:** mega-factory basement + new iron cluster "Site D"
- **Save:** `Beta Game_autosave_1.sav` @ 2026-05-11T08:22:08Z (build 60/489969, 7,326 objects, parse 1002 ms)

### What changed since 2026-05-11 (save-file sync — course-correct)

- **+6 MK1 Iron miners** in a new cluster (provisionally **Site D**) around `(-91K..-99K, 253K..268K, Z≈-4400/-4500)`. Iron miner total `6 → 12` across `4` sites (A/B/C/D).
- **Iron ore supply `180 → 360/min`** (+`180/min`, assuming all 6 new nodes Impure — pending purity check).
- **Smelter bay throughput `37.5% → 75%`** — `12` of `16` smelters now actively fed; `4` still idle.
- **Belts: MK2 `+27`, MK1 `+8`. Lifts: MK2 `+4`.** Consistent with running multi-lane MK2 from Site D to base. Topology unconfirmed.
- No new smelters, constructors, assemblers, generators, or water pumps. Module shape unchanged.

### Iron production line — modules (unchanged shape)

| Module                            | Count × Size           | Per-Module Output | **Total Output**     | Power |
| --------------------------------- | ---------------------- | ----------------- | -------------------- | ----- |
| Iron Plate                        | 2 × 4 Constructors     | `80 plate/min`    | **`160 plate/min`**  | 32 MW |
| Iron Rod (external)               | 2 × 4 Constructors     | `60 rod/min`      | **`120 rod/min`**    | 32 MW |
| Screws (3 screw + 2 internal rod) | 3 × 5 Constructors     | `120 screw/min`   | **`360 screw/min`**  | 60 MW |
| Reinforced Iron Plate (RIP)       | 2 × 2 Assemblers       | `10 RIP/min`      | **`20 RIP/min`**     | 60 MW |

**Total production-line power: `~184 MW`** (unchanged).

### Resource flow

```
Iron Ore supply: 360/min total (was 180 — +180 from Site D, 6 × MK1 Impure assumed)
├── Site A (loose):       60 ore/min   (2 × MK1 Impure)
├── Site B:               60 ore/min   (2 × MK1 Impure)
├── Site C:               60 ore/min   (2 × MK1 Impure)
└── Site D (new):        180 ore/min   (6 × MK1 Impure assumed)

Iron Ingot demand: 450/min total
├── Iron Plate factory:        240 ingot/min  (8 Constructors)
├── External Rod factory:      120 ingot/min  (8 Constructors)
└── Internal rod (in Screws):   90 ingot/min  (6 Constructors across 3 modules)

Iron Smelter bay: 16 smelters at base (4 × 4)
├── Nominal capacity:    480 ingot/min  (16 × 30/min)
├── Ore-fed actual:      360 ingot/min  (75% throughput — 4 smelters idle)
└── Headroom:           +120 ingot/min  unlocks at full 450/min ore (still 90/min short)

Iron Plate flow: 160/min produced (when fully fed) — currently ~128/min ore-limited
Screw flow:      360/min produced (when fully fed) — currently ~288/min ore-limited
Iron Rod flow:   120 external + 90 internal — currently ~80%-fed across the board

⚠ ORE-LIMITED: Iron ingot supply 360/min (80% of 450/min demand). Shortfall 90/min.
```

### Smelter inventory (unchanged, throughput improved)

| Resource     | Site              | Smelter Count  | Effective Output                            | Notes |
| ------------ | ----------------- | -------------- | ------------------------------------------- | ----- |
| Iron Ingot   | Mega factory base | `16` (`4 × 4`) | `360 ingot/min` actual (`480/min` nominal)  | Fed by A+B+C+D = `360 ore/min` → `75%` throughput; `4` smelters idle. `90/min` short of full line target. |
| Copper Ingot | Mega factory base | `4` (`1 × 4`)  | `120 ingot/min` actual (`120/min` nominal)  | Unchanged — stranded downstream. |

### Miner inventory

| Resource   | Purity | Miner | Per-Miner Output | Count | **Total**                |
| ---------- | ------ | ----- | ---------------- | ----- | ------------------------ |
| Iron Ore   | Impure (assumed) | MK1 | `30/min`     | `12`  | **`360 Iron Ore/min`**   |
| Copper Ore | Normal | MK1   | `60/min`         | `1`   | **`60 Copper Ore/min`**  |
| Copper Ore | Impure | MK1   | `30/min`         | `2`   | **`60 Copper Ore/min`**  |
| Limestone  | Impure | MK1   | `30/min`         | `1`   | `30 Limestone/min`       |
| Limestone  | Normal | MK1   | `60/min`         | `1`   | `60 Limestone/min`       |
| Limestone  | ?      | MK1   | `30/min` (assumed Impure) | `1` | `30 Limestone/min` (assumed) |
| Coal       | Pure   | MK1   | `120/min`        | `1`   | **`120 Coal/min`**       |

**Iron-node breakdown:**
- **Site A (loose):** `2 × MK1 Impure = 60 Iron Ore/min`
- **Site B (former minehead):** `2 × MK1 Impure = 60 Iron Ore/min`
- **Site C:** `2 × MK1 Impure = 60 Iron Ore/min`
- **Site D (new, cluster Z≈-4400/-4500):** `6 × MK1 Impure assumed = 180 Iron Ore/min`
- **Combined raw ore arriving at base: `360/min`** — needs `≥ 3 × MK2` or `≥ 2 × MK3` carrier lanes (single MK2 caps at `120/min`).

### Logistics delta (belts/lifts)

| Asset | Prev | Now | Delta |
| ----- | ---- | --- | ----- |
| Belt MK1 | `234` | `242` | `+8` |
| Belt MK2 | `171` | `198` | `+27` |
| Lift MK1 | `5`   | `5`   | unchanged |
| Lift MK2 | `33`  | `37`  | `+4` |

`+27 MK2` belts + `+4 MK2` lifts strongly suggest Site D → base is running multi-lane MK2. **Confirm topology** — see Outstanding flags.

### Power infrastructure (unchanged)

| Asset            | Count × Spec               | Per-Unit                                         | **Total**              |
| ---------------- | -------------------------- | ------------------------------------------------ | ---------------------- |
| Coal Miner       | `1 × MK1` on Pure node     | `120 Coal/min`                                   | **`120 Coal/min`**     |
| Coal Generator   | `2 × 3 = 6` generators     | `75 MW`, `15 Coal/min`, `45 m³ water/min`        | **`450 MW`**           |
| Water Extractor  | `3` pumps                  | `120 m³/min` each                                | **`360 m³ water/min`** |

**Estimated current draw: `~300 MW`** (prior `~270 MW` + `+30 MW` for `+6` MK1 miners at `5 MW` each). **Headroom today: `~150 MW`.**

### Outstanding flags

- **`[BLOCKER]` Iron ore shortfall — much improved, still BLOCKER.** Supply `360/min`, demand `450/min`. Shortfall `90/min` (~`20%`). Tracked as TODO #3 (revised).
- **`[BLOCKER → ?]` Iron belt breach — pending topology confirmation.** Combined raw ore now `360/min`. If Chris ran `≥ 3 × MK2` or `≥ 2 × MK3` lanes from Site D to base, **breach is resolved**. If still on a single merged MK2 to bay, breach is `3×` cap. Tracked as TODO #5 (pending course-correct).
- **`[DEGRADED]` Concrete underfed.** Unchanged — `4` Constructors want `180 limestone/min`; supply (assumed) `120/min`.
- **`[RISK]` Power ceiling.** `450 MW` supply, `~300 MW` draw (was `~270 MW`, `+30 MW` miners). Headroom `~150 MW`. Steel + Oil still project `~450–500 MW`.
- **`[RISK]` Coal headroom insufficient for steel module.** Unchanged.
- **`[INFO]` Copper line retired.** Unchanged.
- **`[INFO]` Biomass reclassified as emergency-only.** Unchanged.

### Pending follow-up stocktakes

- **Confirm Site D node purities** (6 nodes) — each Normal closes `30/min` of the remaining `90/min` shortfall.
- **Confirm Site D → base belt topology** — `3 × MK2`, `2 × MK3`, or something else. Determines whether TODO #5 closes.
- **Confirm Site D is one site vs two (Z=-4400 vs Z=-4500 split).**
- **Limestone Site C+1 purity** — still outstanding.
- **Replacement copper module** spec — still outstanding.

---

## 2026-05-11 — Save-file sync (course-correct)

- **Tier:** 3/4
- **Phase:** working toward Phase 3 of the Space Elevator
- **Belt tier:** MK2 (MK3 pending Tier 4 logistics milestone)
- **Module philosophy:** sub-factories per product, copy-paste to scale
- **Site:** mega-factory basement (smelting fully centralized)

### What changed since 2026-05-11 (centralized smelting + 3rd screw module)

- **Iron smelting scaled to `16`** (`4 × 4`) in the basement — previously recorded `8` (`2 × 4`). TODO #1 flips RESOLVED.
- **New iron miner site C** built — `2 × MK1` Impure (assumed). Iron miner total now `6 × MK1` across **3 sites**.
- **Concrete module** clarified — `4` Constructors fed by **all `3` limestone miners** (not one). Limestone is fully committed to concrete.
- **Copper line retired** — `2` Wire + `1` Cable + `1` Copper Sheet Constructors disconnected. Stranded, not dismantled. Replacement planned later.
- **Biomass plant reclassified** — `12` burners + `4` feeders are **fallback / emergency power only**. Not in live baseline.
- Limestone purity for the `+1` node is **unknown** — assumed `Impure (30/min)` conservatively pending next-session check.

### Iron production line — modules (unchanged shape, same throughputs)

| Module                            | Count × Size           | Per-Module Output | **Total Output**     | Power |
| --------------------------------- | ---------------------- | ----------------- | -------------------- | ----- |
| Iron Plate                        | 2 × 4 Constructors     | `80 plate/min`    | **`160 plate/min`**  | 32 MW |
| Iron Rod (external)               | 2 × 4 Constructors     | `60 rod/min`      | **`120 rod/min`**    | 32 MW |
| Screws (3 screw + 2 internal rod) | 3 × 5 Constructors     | `120 screw/min`   | **`360 screw/min`**  | 60 MW |
| Reinforced Iron Plate (RIP)       | 2 × 2 Assemblers       | `10 RIP/min`      | **`20 RIP/min`**     | 60 MW |

**Total production-line power: `~184 MW`** (unchanged — module scale didn't move).

### Resource flow

```
Iron Ore supply: 180/min total (was 120 — +60 from new Site C, 2 × MK1 Impure)
├── Site A (loose):       60 ore/min   (2 × MK1 Impure)
├── Site B:               60 ore/min   (2 × MK1 Impure)
└── Site C (new):         60 ore/min   (2 × MK1 Impure, purity assumed)

Iron Ingot demand: 450/min total
├── Iron Plate factory:        240 ingot/min  (8 Constructors)
├── External Rod factory:      120 ingot/min  (8 Constructors)
└── Internal rod (in Screws):   90 ingot/min  (6 Constructors across 3 modules)

Iron Smelter bay: 16 smelters at base (4 × 4)
├── Nominal capacity:    480 ingot/min  (16 × 30/min)
├── Ore-fed actual:      180 ingot/min  (37.5% throughput — 10 smelters idle)
└── Headroom:           +270 ingot/min  unlocks when ore solves

Iron Plate flow: 160/min produced (when fully fed)
├── To RIP:           120/min  (4 Assemblers × 30 plate/min)
└── Spare to bus:      40/min

Screw flow: 360/min produced (when fully fed)
├── To RIP:           240/min  (4 Assemblers × 60 screw/min)
└── Free for bus:    120/min   — covers 1 Rotor Assembler (100 screw/min); 20/min spare

Iron Rod flow:
├── External rod:    120/min   available to bus
│   ├── Rotor consumes:    20 rod/min  (for 4 Rotor/min)
│   └── Spare:           100 rod/min   (for Modular Frame / Stator)
└── Internal rod:     90/min   consumed entirely inside screw modules

⚠ ORE-LIMITED: Iron ingot supply 180/min (40% of 450/min demand). Shortfall 270/min.
```

### Smelter inventory (fully centralized in basement)

| Resource     | Site              | Smelter Count  | Effective Output                            | Notes |
| ------------ | ----------------- | -------------- | ------------------------------------------- | ----- |
| Iron Ingot   | Mega factory base | `16` (`4 × 4`) | `180 ingot/min` actual (`480/min` nominal)  | Fed by Site A + B + C = `180 ore/min` → `37.5%` throughput; `10` smelters effectively idle. Capacity already in place for full `450/min` line + headroom. |
| Copper Ingot | Mega factory base | `4` (`1 × 4`)  | `120 ingot/min` actual (`120/min` nominal)  | Fed by merged Normal + 2× Impure = `120 ore/min` → `100%` throughput. Currently stranded — see Retired / stranded. |

**Iron smelter total: `16`.** At `450 ingot/min` target, only `15` needed — **`+1` smelter surplus, sized for headroom.** TODO #1 resolved.
**Copper smelter total: `4`.** Producing `120 ingot/min` but downstream (Wire/Cable/Sheet) is parked.

### Miner inventory

| Resource   | Purity | Miner | Per-Miner Output | Count | **Total**                |
| ---------- | ------ | ----- | ---------------- | ----- | ------------------------ |
| Iron Ore   | Impure | MK1   | `30/min`         | `6`   | **`180 Iron Ore/min`**   |
| Copper Ore | Normal | MK1   | `60/min`         | `1`   | **`60 Copper Ore/min`**  |
| Copper Ore | Impure | MK1   | `30/min`         | `2`   | **`60 Copper Ore/min`**  |
| Limestone  | Impure | MK1   | `30/min`         | `1`   | `30 Limestone/min`       |
| Limestone  | Normal | MK1   | `60/min`         | `1`   | `60 Limestone/min`       |
| Limestone  | ?      | MK1   | `30/min` (assumed Impure) | `1` | `30 Limestone/min` (assumed) |
| Coal       | Pure   | MK1   | `120/min`        | `1`   | **`120 Coal/min`**       |

**Iron-node breakdown (all belted to base, no minehead smelters):**
- **Site A (loose):** `2 × MK1 Impure = 60 Iron Ore/min` → merged at base on MK2 belt.
- **Site B (former minehead):** `2 × MK1 Impure = 60 Iron Ore/min` → merged at base on MK2 belt.
- **Site C (new):** `2 × MK1 Impure = 60 Iron Ore/min` → merged at base on MK2 belt.
- **Merged total at base: `180 Iron Ore/min` on MK2 belt (`120/min` cap).** Single MK2 belt **already breached** — see TODO #5 revision.

**Copper-node breakdown (all belted to base):**
- **Normal copper:** `1 × MK1 = 60/min` + **Impure copper:** `2 × MK1 = 60/min` → merged onto one MK2 belt (`120/min`, saturated).

**Limestone-node breakdown:**
- **Normal:** `1 × MK1 = 60/min`
- **Impure:** `1 × MK1 = 30/min`
- **Unknown purity (new):** `1 × MK1 = 30/min` (assumed Impure pending confirmation)
- **Total assumed: `120 Limestone/min`** — entirety fed to Concrete module (`4` Constructors).

### Concrete module

| Module   | Count × Size       | Recipe                              | Throughput                                                            | Power |
| -------- | ------------------ | ----------------------------------- | --------------------------------------------------------------------- | ----- |
| Concrete | `4` Constructors   | Concrete (3 limestone → 1 concrete) | `60 concrete/min` at full feed (`180 limestone/min` demand)           | 16 MW |

**Concrete feed check:** `4` Constructors at full pull need `180 limestone/min`. Supply (assumed) `120/min`. **`33%` underfed → effective output `~40 concrete/min`.** If the new node turns out Normal, supply jumps to `150/min` (still short `30/min`). Flag for next-session purity check.

### Retired / stranded

| Asset                    | Count | Status                        | Notes |
| ------------------------ | ----- | ----------------------------- | ----- |
| Wire Constructors        | `2`   | Stranded (input disconnected) | Parked, not dismantled. To be folded into base modular setup. |
| Cable Constructor        | `1`   | Stranded                      | Same. |
| Copper Sheet Constructor | `1`   | Stranded                      | Same. |
| Biomass Burner           | `12`  | **Fallback / emergency only** | Coal is primary. Burners + `4` feeders held in reserve; not in live power baseline. |
| Biomass Feeder           | `4`   | Fallback / emergency only     | Powers biomass burners on demand only. |

### Power infrastructure (unchanged)

| Asset            | Count × Spec               | Per-Unit                                         | **Total**              |
| ---------------- | -------------------------- | ------------------------------------------------ | ---------------------- |
| Coal Miner       | `1 × MK1` on Pure node     | `120 Coal/min` (Pure × 2 of MK1's `60/min` base) | **`120 Coal/min`**     |
| Coal Generator   | `2 × 3 = 6` generators     | `75 MW`, `15 Coal/min`, `45 m³ water/min`        | **`450 MW`**           |
| Water Extractor  | `3` pumps                  | `120 m³/min` each                                | **`360 m³ water/min`** |

**Balance check:** Coal `120 in − 90 consumed = 30/min spare`. Water `360 in − 270 consumed = 90 m³/min spare`. Power `450 MW` supply.

**Estimated current draw: `~270 MW`** (production line `184 MW` + `16` active smelters with `~6` fed (`~24 MW`) + concrete module `16 MW` + miners + misc `~46 MW`). **Headroom today: `~180 MW`.**

### Outstanding flags

- **`[BLOCKER]` Iron ore shortfall — improved but still BLOCKER.** Target ingot demand `450/min` → ore demand `450/min`. Supply now `180/min` (Site C added `+60`). **Shortfall `270/min` (~`60%`).** Tracked as TODO #3 (revised).
- **`[BLOCKER]` Iron belt breach at base — ACTIVE NOW.** Merged Iron belt carries `180/min` into base, MK2 cap `120/min`. **Belt overflows today, not "pending MK2 miner upgrade".** Tracked as TODO #5 (revised — severity escalation).
- **`[DEGRADED]` Concrete underfed.** `4` Constructors want `180 limestone/min`; supply (assumed) `120/min` → effective output `~40/60 concrete/min`. Resolves partially if new node is Normal (`150/min` supply). **Confirm purity next session.**
- **`[RISK]` Power ceiling at Tier 4 jump.** `450 MW` supply, `~270 MW` draw today (concrete added `~16 MW`). Steel + Oil + smelter scale-up still project `~450–500 MW`. Plan a 3rd coal pair (`+150 MW`) or fuel gens.
- **`[RISK]` Coal headroom insufficient for steel module.** Unchanged — `30/min` spare vs `45/min` needed. TODO #6.
- **`[INFO]` Copper line retired.** `4` copper Constructors stranded. `4` copper Smelters producing `120/min` with no live consumer — copper ingot will back up. Plan replacement module before re-enabling.
- **`[INFO]` Biomass reclassified as emergency-only.** Not in live baseline; available if coal trips.

### Pending follow-up stocktakes

- **Confirm Limestone Site C+1 purity** next session — flips Concrete from `DEGRADED` to either healthy (Normal) or worse-than-assumed (Pure unlikely on the third limestone).
- **Confirm Site C iron-node purity** — assumed all Impure; if any are Normal, the BLOCKER shortfall narrows.
- **Replacement copper module** spec — Wire/Cable/Sheet sizing for current bus demand before re-energising the stranded Constructors.

---

## 2026-05-11 — Centralized smelting + 3rd screw module

- **Tier:** 3/4
- **Phase:** working toward Phase 3 of the Space Elevator
- **Belt tier:** MK2 (MK3 pending Tier 4 logistics milestone)
- **Module philosophy:** sub-factories per product, copy-paste to scale
- **Site:** mega-factory basement (smelting now centralized — all minehead smelters dismantled)

### What changed since 2026-05-10

- Dismantled all minehead smelters (`4` iron at Site B + `~4` copper across both copper mineheads). Raw ore now belted to the basement.
- Built `1` copper smelter module (`4` Smelters) in the basement.
- Built a **3rd screw module** (`5` Constructors → **`+120 screw/min`**).

### Iron production line — modules

| Module                            | Count × Size           | Per-Module Output | **Total Output**     | Power |
| --------------------------------- | ---------------------- | ----------------- | -------------------- | ----- |
| Iron Plate                        | 2 × 4 Constructors     | `80 plate/min`    | **`160 plate/min`**  | 32 MW |
| Iron Rod (external)               | 2 × 4 Constructors     | `60 rod/min`      | **`120 rod/min`**    | 32 MW |
| Screws (3 screw + 2 internal rod) | **3 × 5 Constructors** | `120 screw/min`   | **`360 screw/min`**  | **60 MW** |
| Reinforced Iron Plate (RIP)       | 2 × 2 Assemblers       | `10 RIP/min`      | **`20 RIP/min`**     | 60 MW |

**Total production-line power: `~184 MW`** (was `~164 MW` — `+20 MW` for 3rd screw module)

### Resource flow

```
Iron Ingot demand: 450/min total (was 420 — +30 from 3rd screw module's internal rod)
├── Iron Plate factory:        240 ingot/min  (8 Constructors)
├── External Rod factory:      120 ingot/min  (8 Constructors)
└── Internal rod (in Screws):   90 ingot/min  (6 Constructors across 3 modules)

Iron Plate flow: 160/min produced
├── To RIP:           120/min  (4 Assemblers × 30 plate/min)
└── Spare to bus:      40/min

Screw flow: 360/min produced (was 240 — 3rd module added)
├── To RIP:           240/min  (4 Assemblers × 60 screw/min)
└── Free for bus:    120/min   — covers 1 Rotor Assembler (100 screw/min); 20/min spare

Iron Rod flow:
├── External rod:    120/min   available to bus
│   ├── Rotor consumes:    20 rod/min  (for 4 Rotor/min)
│   └── Spare:           100 rod/min   (for Modular Frame / Stator)
└── Internal rod:     90/min   consumed entirely inside screw modules

Output to factory bus:
├── RIP:               20/min  (gateway for Modular Frame, Smart Plating)
├── Iron Plate spare:  40/min  (buffer or future RIP module #3)
├── Iron Rod spare:   100/min  (next consumers: Modular Frame, Stator)
└── Screws spare:     120/min  (covers Rotor + 20/min margin)

⚠ ORE-LIMITED: Iron supply still 120 ingot/min (26.7% of demand). Shortfall 330/min.
```

### Smelter inventory (fully centralized in basement)

| Resource     | Site              | Smelter Count | Effective Output                          | Notes |
| ------------ | ----------------- | ------------- | ----------------------------------------- | ----- |
| Iron Ingot   | Mega factory base | `8` (`2 × 4`) | `120 ingot/min` actual (`240/min` nominal) | Fed by merged Site A + Site B = `120 ore/min` → `50%` throughput, `4` smelters effectively idle. |
| Copper Ingot | Mega factory base | `4` (`1 × 4`) | `120 ingot/min` actual (`120/min` nominal) | Fed by merged Normal + 2× Impure = `120 ore/min` → `100%` throughput. |

**Iron smelter total: `8` (all at base).** Once ore lands at `450 ore/min` target, need `15` — short `7`.
**Copper smelter total: `4` (all at base).** Saturated at current ore supply.

### Miner inventory (unchanged from 2026-05-10)

| Resource   | Purity | Miner | Per-Miner Output | Count | **Total**                |
| ---------- | ------ | ----- | ---------------- | ----- | ------------------------ |
| Iron Ore   | Impure | MK1   | `30/min`         | `4`   | **`120 Iron Ore/min`**   |
| Copper Ore | Normal | MK1   | `60/min`         | `1`   | **`60 Copper Ore/min`**  |
| Copper Ore | Impure | MK1   | `30/min`         | `2`   | **`60 Copper Ore/min`**  |
| Limestone  | Impure | MK1   | `30/min`         | `1`   | `30 Limestone/min`       |
| Limestone  | Normal | MK1   | `60/min`         | `1`   | `60 Limestone/min`       |
| Coal       | Pure   | MK1   | `120/min`        | `1`   | **`120 Coal/min`**       |

**Iron-node breakdown (both belted to base, no minehead smelters):**
- **Site A (loose):** `2 × MK1 Impure = 60 Iron Ore/min` → merged at base on MK2 belt.
- **Site B (former minehead):** `2 × MK1 Impure = 60 Iron Ore/min` → merged at base on MK2 belt.

**Copper-node breakdown (all belted to base):**
- **Normal copper:** `1 × MK1 = 60/min` + **Impure copper:** `2 × MK1 = 60/min` → merged onto one MK2 belt (`120/min`, saturated).

### Power infrastructure (unchanged from 2026-05-10)

| Asset            | Count × Spec               | Per-Unit                                         | **Total**              |
| ---------------- | -------------------------- | ------------------------------------------------ | ---------------------- |
| Coal Miner       | `1 × MK1` on Pure node     | `120 Coal/min` (Pure × 2 of MK1's `60/min` base) | **`120 Coal/min`**     |
| Coal Generator   | `2 × 3 = 6` generators     | `75 MW`, `15 Coal/min`, `45 m³ water/min`        | **`450 MW`**           |
| Water Extractor  | `3` pumps                  | `120 m³/min` each                                | **`360 m³ water/min`** |

**Balance check (unchanged):** Coal `120 in − 90 consumed = 30/min spare`. Water `360 in − 270 consumed = 90 m³/min spare`. Power `450 MW` supply.

**Estimated current draw: `~254 MW`** (production line `184 MW` + active smelters `~32 MW` + miners + misc `~38 MW`). **Headroom today: `~196 MW`.**

### Outstanding flags

- **`[BLOCKER]` Iron ore shortfall — WORSE.** Target ingot demand now `450/min` → ore demand `450/min`. Supply `120/min`. **Shortfall `330/min` (~`73%`).** The 3rd screw module widened the gap by `30/min` of internal-rod ingot demand. Tracked as TODO #3.
- **`[DEGRADED]` Iron smelter count gap.** At `450/min` target, need `15`; have `8`. Short `7`. Dormant until ore solves. Tracked as TODO #1.
- **`[RISK]` Power ceiling at Tier 4 jump.** `450 MW` supply, `~254 MW` draw today. Steel Foundries, Oil Refineries, smelter scale-up, and Rotor/MF/Smart Plating still project the next-phase draw to `~450–500 MW`. Plan a 3rd coal module (`+225 MW`) or pivot to fuel generators when oil lands.
- **`[RISK]` Belt breach pending MK2 miner upgrade.** Post-MK2 miners, Iron `240/min` and Copper `240/min` each exceed MK2 belt cap (`120/min`). Need MK3 belts (Tier 4) or `2 × MK2` per ore. Tracked as TODO #5.
- **`[RISK]` Coal headroom insufficient for steel module.** When steel lands, demand `+45 coal/min`, spare `30/min`, short `15/min`. Tracked as TODO #6.

### Pending follow-up stocktakes

- **Copper-line stocktake** (Cable, Wire, downstream consumers) before Tier 4 Caterium / Heavy Modular Frame demand lands.
- Existing Phase 1 / Phase 2 factories — Smart Plating, Versatile Framework, Modular Engine, Automated Wiring (assumed running from prior phase deliveries; not yet stocktaken).

---

## 2026-05-10 — Initial iron production-line stocktake

- **Tier:** 3/4
- **Phase:** working toward Phase 3 of the Space Elevator
- **Belt tier:** MK2 (MK3 pending Tier 4 logistics milestone)
- **Module philosophy:** sub-factories per product, copy-paste to scale
- **Site:** mega-factory basement

### Iron production line — modules

| Module                            | Count × Size           | Per-Module Output | **Total Output**     | Power |
| --------------------------------- | ---------------------- | ----------------- | -------------------- | ----- |
| Iron Plate                        | 2 × 4 Constructors     | `80 plate/min`    | **`160 plate/min`**  | 32 MW |
| Iron Rod (external)               | 2 × 4 Constructors     | `60 rod/min`      | **`120 rod/min`**    | 32 MW |
| Screws (3 screw + 2 internal rod) | 2 × 5 Constructors     | `120 screw/min`   | **`240 screw/min`**  | 40 MW |
| Reinforced Iron Plate (RIP)       | 2 × 2 Assemblers       | `10 RIP/min`      | **`20 RIP/min`**     | 60 MW |

**Total production-line power: `~164 MW`**

### Resource flow

```
Iron Ingot demand: 420/min total
├── Iron Plate factory:        240 ingot/min  (8 Constructors)
├── External Rod factory:      120 ingot/min  (8 Constructors)
└── Internal rod (in Screws):   60 ingot/min  (4 Constructors across 2 modules)

Iron Plate flow: 160/min produced
├── To RIP:           120/min  (4 Assemblers x 30 plate/min)
└── Spare to bus:      40/min

Screw flow: 240/min produced
└── To RIP:           240/min  (4 Assemblers x 60 screw/min) — 100% consumed

Iron Rod flow:
├── External rod:     120/min  available for Rotor, Modular Frame, etc.
└── Internal rod:      60/min  consumed entirely inside screw modules

Output to factory bus:
├── RIP:               20/min  (gateway for Modular Frame, Smart Plating)
├── Iron Plate spare:  40/min  (buffer or future RIP module #3)
└── Iron Rod:         120/min  (next consumer: Rotor)
```

### Power infrastructure

| Asset            | Count × Spec               | Per-Unit                                         | **Total**              |
| ---------------- | -------------------------- | ------------------------------------------------ | ---------------------- |
| Coal Miner       | `1 × MK1` on Pure node     | `120 Coal/min` (Pure × 2 of MK1's `60/min` base) | **`120 Coal/min`**     |
| Coal Generator   | `2 × 3 = 6` generators     | `75 MW`, `15 Coal/min`, `45 m³ water/min`        | **`450 MW`**           |
| Water Extractor  | `3` pumps                  | `120 m³/min` each                                | **`360 m³ water/min`** |

**Balance check:**

- Coal: `120 in − 90 consumed = 30/min spare` (room for `2` more generators).
- Water: `360 in − 270 consumed = 90 m³/min spare` (room for `2` more generators).
- Power output: **`450 MW`** total.

**Estimated current draw: `~250 MW`** (production line `164 MW` + Iron Smelters `32 MW`
+ Copper Smelters + miners + misc `~50 MW`). **Headroom today: `~200 MW`.**

### Miner inventory

| Resource   | Purity | Miner | Per-Miner Output | Count | **Total**                |
| ---------- | ------ | ----- | ---------------- | ----- | ------------------------ |
| Iron Ore   | Impure | MK1   | `30/min`         | `4`   | **`120 Iron Ore/min`**   |
| Copper Ore | Normal | MK1   | `60/min`         | `1`   | **`60 Copper Ore/min`**  |
| Copper Ore | Impure | MK1   | `30/min`         | `2`   | **`60 Copper Ore/min`**  |
| Limestone  | Impure | MK1   | `30/min`         | `1`   | `30 Limestone/min`       |
| Limestone  | Normal | MK1   | `60/min`         | `1`   | `60 Limestone/min`       |
| Coal       | Pure   | MK1   | `120/min`        | `1`   | **`120 Coal/min`**       |

**Iron-node breakdown:**
- **Site A (loose, no minehead smelters):** `2 × MK1 Impure = 60 Iron Ore/min` → belted to mega factory base.
- **Site B (minehead with smelters):** `2 × MK1 Impure = 60 Iron Ore/min` → fed into the `4` minehead smelters on-site.

**Totals:**
- **Iron Ore: `120/min`** (4 × Impure across 2 sites)
- **Copper Ore: `120/min`** (1 × Normal + 2 × Impure)
- **Limestone: `90/min`** (1 × Impure + 1 × Normal)
- **Coal: `120/min`** (1 × Pure)

MK1 base = `60/min` on Normal; Impure = `0.5×`, Pure = `2×`.

### Smelter inventory

| Resource     | Site                      | Smelter Count | Effective Output      | Notes |
| ------------ | ------------------------- | ------------- | --------------------- | ----- |
| Iron Ingot   | At Impure iron minehead (Site B) | `4`    | `60 ingot/min` actual (`120/min` nominal) | Fed by Site B's `60 ore/min` → `50%` throughput, 2 smelters effectively idle. Ingots belted to mega factory. |
| Iron Ingot   | At mega factory base      | `8` (`2 × 4`) | `60 ingot/min` actual (`240/min` nominal) | Fed by Site A's `60 ore/min` belted from the loose miners → `25%` throughput, 6 smelters effectively idle. |
| Copper Ingot | At Impure copper minehead | `4`           | `60 ingot/min` actual (`120/min` nominal) | Ore-limited at `60/min` → `50%` throughput, 2 smelters effectively idle |
| Copper Ingot | At Normal copper minehead | unknown       | up to `60 ingot/min`  | Smelter count not yet recorded |

**Iron smelter total: `12` (`4` minehead + `8` base).** At full ore feed they'd
produce `360 ingot/min` — only `2` smelters short of the `420/min` full target.
Actual combined output: **`120 Iron Ingot/min`** (Site B: `60` + Site A via base:
`60`). Ore is the binding constraint (see TODO #3); smelter count is fine once
ore is solved.

*(Flags and pending follow-ups for this snapshot have moved to the current
2026-05-11 snapshot above, per file convention.)*

---

## How to update this file

- New snapshot → new `## YYYY-MM-DD — <title>` block at the top.
- Don't delete old snapshots; they show factory progression over time.
- Keep the module table format consistent.
- Resource-flow blocks stay inside code fences (terminal-friendly).
- Flags + pending items live in the current snapshot, not history.
