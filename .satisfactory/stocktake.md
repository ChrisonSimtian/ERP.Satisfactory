# Satisfactory Stocktake

Living record of factory state in Chris's Satisfactory v1.2 save. Update whenever
modules are added, relocated, or scaled. Latest snapshot at the top — older
snapshots stay below for progression tracking.

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

### Outstanding flags

- **`[ALERT]` Iron ore extraction short (BLOCKER)** — production line demands
  `420 Iron Ore/min` (full target). Current supply: `120 Iron Ore/min` from
  `4 × MK1` on impure across 2 sites. **Shortfall: `300/min` (~`71%`).**
  Combined ingot output: `120/min` (`60` from Site A via base smelters at `25%`,
  `60` from Site B minehead smelters at `50%`). Production line runs at
  `~29%` effective throughput. Tracked as TODO #3.
- **Screws at 100% utilisation (BLOCKER for Rotor)** — RIP consumes the entire
  `240 screw/min`. Zero screws available for Rotor. Resolve via: 3rd screw
  module (+`120/min`), or scale RIP down to `10/min`, or unlock **Cast Screw**
  alt. Tracked as TODO #2.
- **Smelter shortfall (BLOCKER)** — production line demands `420 ingot/min`.
  Currently `8` Iron Smelters at `240/min` → `6` smelters short. Tracked as
  TODO #1. *(Note: until ore is solved, the smelter expansion is moot — TODO #3
  is the real upstream blocker.)*
- **`[RISK]` Power ceiling at the Tier 4 jump** — `450 MW` supply, ~`250 MW` draw
  today, but Steel Foundries (`+32 MW`), Oil Refineries (`+90 MW`), the smelter
  expansion (`+24 MW`), and Rotor/MF/Smart Plating (`+45–60 MW`) project the
  next-phase draw to `~450–500 MW`. Plan a 3rd coal module (`+225 MW`) or pivot
  to fuel generators when oil lands.

### Pending follow-up stocktakes

- Iron Smelters and Iron Ore miners (Chris assumed "just enough" but the math
  above says he's short).
- Copper Smelters (currently at the minehead, slated for relocation into the
  basement).
- Existing Phase 1 / Phase 2 factories — Smart Plating, Versatile Framework,
  Modular Engine, Automated Wiring (assumed running from prior phase deliveries;
  not yet stocktaken).

---

## How to update this file

- New snapshot → new `## YYYY-MM-DD — <title>` block at the top.
- Don't delete old snapshots; they show factory progression over time.
- Keep the module table format consistent.
- Resource-flow blocks stay inside code fences (terminal-friendly).
- Flags + pending items live in the current snapshot, not history.
