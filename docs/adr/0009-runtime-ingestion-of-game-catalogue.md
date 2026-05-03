# 0009. Runtime ingestion of game catalogue from Docs.json

- Status: Accepted
- Date: 2026-05-03
- Deciders: Chris

## Context

The planner needs the full game catalogue (items, buildings, recipes incl.
alternates) to produce useful plans. The current 12-item hand-coded seed in
`Satisfactory.Catalog` proves the architecture but is not viable as a long-term
data source: Satisfactory ships frequent updates and players run different patch
levels (Update 8 vs 1.0 vs Ficsmas, Early Access branches, etc.). We also want
this codebase to potentially extend beyond Satisfactory in the future.

The Satisfactory game install ships with a `Docs.json` file containing the full
machine-readable catalogue. Coffee Stain owns that data, so redistributing it
inside our repo creates a licensing question.

## Decision

The catalogue is **loaded at application startup from the user's installed
`Docs.json`**. Nothing game-specific is checked into the repo; the user points
the app at their game install and the catalogue is parsed dynamically into the
in-memory model.

## Alternatives considered

- **Vendor `Docs.json` (or a derived JSON) into the repo.** Cleanest UX but
  Coffee Stain owns the data and we don't want to host it. Also forces a repo
  update for every game patch.
- **Codegen a `.cs` catalogue** from `Docs.json` at build time. Same licensing
  problem if the source is checked in; if the source is fetched at build, we've
  reinvented runtime ingestion at a worse time.
- **Manual curation** (continue the hand-coded approach). Doesn't scale past
  ~30 recipes; goes stale on every patch.

## Consequences

- The user must provide a path to `Docs.json` ([ADR 0011](0011-catalogue-source-path-configuration.md)).
  First-run UX needs to handle "not configured yet" and "file missing" cases
  cleanly.
- The catalogue is a singleton populated during `IHost.StartAsync`; planner
  queries assume it's loaded.
- We may later ship a default `Docs.json` (e.g. last known stable game version)
  as a fallback for users who don't have the game installed — explicitly out of
  scope for now.
- Multi-game capability falls out: a different game adapter just provides a
  different parser. Catalogue contract lives in the Application layer
  ([ADR 0010](0010-game-agnostic-catalogue-contract.md)).
- Ingesting parts of the catalogue beyond items/buildings/recipes (fluids,
  extractors, schematics) is deferred to dedicated epics — `Docs.json` contains
  them, but each carries domain logic the planner shouldn't conflate with the
  manufacturing core.
