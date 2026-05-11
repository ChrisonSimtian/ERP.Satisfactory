# Satisfactory Stocktake

Living record of factory state in Chris's Satisfactory v1.2 save. Update whenever
modules are added, relocated, or scaled. Latest snapshot at the top — older
snapshots stay below for progression tracking.

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
