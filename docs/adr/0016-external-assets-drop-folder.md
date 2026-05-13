# 0016. External game-derived assets via the `.assets/` drop folder

- Status: Accepted
- Date: 2026-05-13
- Deciders: Chris
- Extends: [0015](0015-map-backdrops-fair-use.md)

## Context

[ADR 0015](0015-map-backdrops-fair-use.md) accepted shipping three map-backdrop
images in `src/Web/wwwroot/lib/maps/` under a fair-use claim. The rationale
rested on the scope being explicitly limited: three images, used as backdrops
only, with attribution.

Phase 1 of the MudBlazor migration (issue #22) added a searchable item picker
on the Planner page and motivated bundling per-item icons next to each row in
the dropdown. The icons come from the same source as the maps
([satisfactory.wiki.gg](https://satisfactory.wiki.gg/), community-uploaded
content under fair-use-for-fan-tools), but the catalogue contains ~190 items
(~11 MB of PNGs). That is **not** the "limited scope" carve-out ADR-0015 leaned
on, and we don't want to test the fair-use line at that scale.

We need a way to use the icons in the running app without committing them to
the repo.

## Decision

Adopt a **drop-folder convention** for external game-derived assets:

- A folder named `.assets/` at the repo root is **gitignored**, by `.gitignore`
  line `.assets/` with the comment `# Local reference assets (user-only, not for
  redistribution)`. This folder already existed for the map source files Chris
  dropped there manually; we are now formalising its role.
- `src/Web/Program.cs` mounts `.assets/` at the `/assets/*` URL path via
  `UseStaticFiles` + `PhysicalFileProvider`. The Web project walks up from
  `ContentRootPath` to find a sibling `.assets/` directory (so the layout works
  in dev), with `Assets:LocalPath` available as a configuration override.
- Pages reference assets by their local path (e.g.
  `<img src="/assets/icons/items/Desc_IronIngot_C.png">`) and fall back
  gracefully when an asset is missing — for the picker, an `onerror` handler
  hides the `<img>` and the row renders text-only.
- `tools/Update-Assets.ps1` is the reproducible way to populate `.assets/`
  from the wiki. It queries the running ApiService's `/catalog/items` endpoint
  for the current item list, then pulls each icon plus the three map backdrops
  from `https://satisfactory.wiki.gg/images/{Filename}`. The script paces
  requests at ~1 req/s with a User-Agent that identifies the project and
  retries once on HTTP 429.

Icons are keyed by stable in-game class ID (`Desc_IronIngot_C.png`), not by
display name. Display names can change between game patches; class IDs are
stable.

## Why not extend ADR-0015 to icons?

ADR-0015's fair-use claim rests partly on "limited scope (three images, used
as backdrops only)". 190 item icons is the **full game catalogue**, not a
limited subset. Conservative read: ship the three maps under fair use, keep
everything bigger out of distribution. The dropbox-folder approach keeps the
running app's UX identical while removing us from the redistribution
question entirely.

## Why not extract from the user's local game install?

Considered. The Docs.json parser already requires a Satisfactory install
([ADR-0009](0009-runtime-ingestion-of-game-catalogue.md)), so we could assume
the user has the game files. But icons live inside Unreal Engine `.uasset`
files and need UE asset tooling (UModel, FModel) to extract — a much heavier
dependency than a polite HTTP fetch from the wiki, which is also where users
go for icon reference anyway.

If a user wants offline reproducibility without the wiki dependency, they can
populate `.assets/` from any source (a local UModel extraction, a bundle from
another fan tool, etc.) — the runtime mapping doesn't care where the PNGs
came from.

## Consequences

- A fresh clone has no icons or map source files until
  `pwsh tools/Update-Assets.ps1` is run. The Planner picker degrades to
  text-only; the map page keeps working because the three map images are
  separately committed in `wwwroot/lib/maps/` under ADR-0015. **The app
  does not break on a fresh clone**, it just looks plainer until the script
  runs.
- CI does not have icons. UI tests do not assert against them; if a future
  test does, it must seed `.assets/` first (or accept the degraded state).
- Game updates that rename items or change icon filenames require re-running
  the script and possibly adding entries to the `$nameOverrides` table in
  `Update-Assets.ps1` (the wiki occasionally diverges from the in-game
  display name — currently for `Desc_GolfCart_C`, `Desc_GolfCartGold_C`, and
  `Desc_Snow_C`).
- Four FICSMAS event items (Day-N Data Cartridges) have no wiki pages and
  always render text-only. Acceptable miss.
- The three map backdrops in `wwwroot/lib/maps/` (ADR-0015) remain committed
  and shipped. The script also refreshes them in `.assets/` for parity, but
  the committed copies are the source of truth at runtime for the map page.
- Attribution: `satisfactory.wiki.gg` is the source. The User-Agent the
  script sends identifies this project, satisfying minimum courtesy. A
  visible attribution surface (Settings page footer, About panel) is a
  follow-up; not blocking.

## Alternatives considered

- **Commit the icons (extend ADR-0015 to 190 PNGs).** Rejected — past fair
  use's "limited scope" line, and the repo size cost (~11 MB) for assets
  that change with every game patch is not worth it.
- **Hot-link to the wiki at runtime.** Rejected — the user asked for
  offline self-sufficiency. Hot-linking means every page render hits the
  wiki for ~190 images.
- **Build-time extraction from the user's game install** (the
  `greeny/SatisfactoryTools` approach). Rejected — requires UModel as a
  build dependency, heavier than a polite scrape, and the user already runs
  the app against a game install for the catalogue. The HTTP fetch is the
  lighter contract.
- **CDN of pre-extracted icons.** No suitable community CDN exists today
  that we'd want to take a hard dependency on. The wiki is the canonical
  source.
